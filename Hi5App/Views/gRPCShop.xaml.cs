using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcShopClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hi5App.Views
{
    /// <summary>
    /// Interaction logic for gRPCShop.xaml
    /// </summary>
    public partial class gRPCShop : Page
    {
        public gRPCShop()
        {
            InitializeComponent();
        }

        private async void gRPCStartButton_Click(object sender, RoutedEventArgs e)
        {
            var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var response = await client.SayHelloAsync(new HelloRequest { Name = "First gRPC" });

            AppendText(response.Message);

        }

        private async void gRPCUnaryButton_Click(object sender, RoutedEventArgs e)
        {
            AppendText("starting unary calling...");
            var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
            var client = new StreamShoper.StreamShoperClient(channel);

            var request = new ExampleRequest { PageIndex = 2, PageSize = 10, IsDescending = true };

            var response = await client.UnaryCallAsync(request);
            AppendText(response.Message);
        }

        private async void gRPCClientStreamButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendText("starting Client Steam Calling...");
                var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
                var client = new StreamShoper.StreamShoperClient(channel);

                var cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                using var call = client.StreamingFromClient(null, null, token);
                for (int i = 8; i >= 0; i--)
                {
                    AppendText($"#From Client# the index is {i}");
                    var request = new ExampleRequest { PageIndex = i, PageSize = 10, IsDescending = true };
                    await call.RequestStream.WriteAsync(request);

                    //await Task.Delay(100);
                }
                // cancelSource.Cancel();

                var cancelRequest = new ExampleRequest { PageIndex = 0, PageSize = 10, IsDescending = true };
                await call.RequestStream.WriteAsync(cancelRequest);

                var response = await call.ResponseAsync;
                AppendText(response.Message);
            }
            catch (Exception ex)
            {
                AppendText("Cancell...");
            }
        }

        private async void gRPCServerStreamButton_Click(object sender, RoutedEventArgs e)
        {
            AppendText("starting server stream...");
            var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
            var client = new StreamShoper.StreamShoperClient(channel);

            var request = new ExampleRequest { PageIndex = 2, PageSize = 10, IsDescending = true };
            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;

            using var call = client.StreamingFromServer(request,null,null,token);

            int maxIndex = 10;
            while (true)
            {
                try
                {
                    await call.ResponseStream.MoveNext(token);

                    var response = call.ResponseStream.Current;
                    AppendText(response.Message);
                    if (response.TotalCount >= 10)
                    {
                        AppendText("Meet max, cancelling...");
                        cancelSource.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    AppendText("Exit...");
                    break;
                }
            } 
        }

        private async void gRPCBothStreamButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppendText("starting Both Steam Calling...");
                var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
                var client = new StreamShoper.StreamShoperClient(channel);

                var cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                var call = client.StreamingBothWays(null, null, token);

                for (int i = 8; i >= 0; i--)
                {
                    AppendText($"#From Client# the index is {i}");
                    var request = new ExampleRequest { PageIndex = i, PageSize = 10, IsDescending = true };
                    await call.RequestStream.WriteAsync(request);

                    await call.ResponseStream.MoveNext(token);
                    var response = call.ResponseStream.Current;
                    AppendText(response.Message);

                    AppendText("Waiting 2 seconds...");
                    await Task.Delay(2000);

                    //await Task.Delay(100);
                }
                AppendText("Starting cancelling...");
                cancelSource.Cancel();

            }
            catch (Exception ex)
            {
                AppendText("Cancelling...");
            }
        }

        private async void gRPCStartIPCButton_Click(object sender, RoutedEventArgs e)
        {
            //Config retry 
            var methodConfig = new MethodConfig
            {
                Names = { MethodName.Default },
                RetryPolicy = new RetryPolicy
                {
                    MaxAttempts = 10,
                    InitialBackoff = TimeSpan.FromSeconds(10),
                    MaxBackoff = TimeSpan.FromSeconds(100),
                    BackoffMultiplier = 1.5,
                    RetryableStatusCodes = { Grpc.Core.StatusCode.Unavailable }
                }
            };

            AppendText("Starting IPC gRPC");
            var SocketPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "socket.tmp");
            AppendText($"Tmp path is {SocketPath}");

            var channel = GrpcConnectionFactory.CreateUdsChannel(methodConfig);
            var client = new Greeter.GreeterClient(channel);

            var response = await client.SayHelloAsync(new HelloRequest { Name = "First IPC gRPC" });

            AppendText(response.Message);
        }

        private async void gRPCBigDataBothStreamButton_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw1.Start();

            try
            {
                AppendText("starting Both Steam Calling...");
                var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
                var client = new StreamShoper.StreamShoperClient(channel);

                var cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                var call = client.SendingBigDataPackage(null, null, token);
                AppendText("Start to send data...");

                
                sw2.Start();

                for (int i = 10; i >= 0; i--)
                {
                    AppendText($"#From Client# the index is {i}");
                    var request = new BigDataRequest { DataType= $"index{i}" };

                    var strTmp = new string('A', 1024);

                    var times = 5000;
                    while (times > 0)
                    {
                        request.Content.Add(strTmp);
                        times--;
                    }

                    await call.RequestStream.WriteAsync(request);

                    await call.ResponseStream.MoveNext(token);
                    var response = call.ResponseStream.Current;
                    AppendText(response.Message);

                    //AppendText("Waiting 2 seconds...");
                    //await Task.Delay(2000);

                    //await Task.Delay(100);
                }
                AppendText("Starting cancelling...");
                cancelSource.Cancel();

                sw2.Stop();

            }
            catch (Exception ex)
            {
                AppendText(ex.Message);
                AppendText("Cancelling...");
            }

            sw1.Stop();

            AppendText($"Sending data time cost:{sw2.Elapsed.TotalSeconds}");
            AppendText($"Total time cost:{sw1.Elapsed.TotalSeconds}");
        }

        private void AppendText(string text)
        {
            ResultTextBox.Text += (Environment.NewLine + text);
        }

    }
}
