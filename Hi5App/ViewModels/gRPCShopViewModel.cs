using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcShopClient;
using Hi5App.ViewModels.Interface;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hi5App.ViewModels
{
    public class gRPCShopViewModel : BindableBase, IgRPCShopViewModel
    {
        private const string DefaultChannelAddress = @"https://localhost:5001";

        private string resultMsg;
        private double packageSize;
        private int sendTimes;

        private ICommand _gRPCStartCmd;
        private ICommand _gRPCUnaryCmd;
        private ICommand _gRPCClientStreamCmd;
        private ICommand _gRPCServerStreamCmd;
        private ICommand _gRPCBothStreamCmd;
        private ICommand _gRPCStartIPCCmd;
        private ICommand _gRPCIPCBigDataBothStreamCmd;
        private ICommand _gRPCBigDataBothStreamCmd;

        private ICommand clearResultCmd;

        public gRPCShopViewModel()
        {
            //default values
            packageSize = 1;
            SendTimes = 10;
        }

        public string ResultMsg
        {
            get { return resultMsg; }
            set { SetProperty(ref resultMsg, value); }
        }

        public double PackageSize
        {
            get { return packageSize; }
            set 
            {
                if (SetProperty(ref packageSize, value))
                {
                    RaisePropertyChanged(nameof(ConfigString));
                }
            }
        }

        public int SendTimes
        {
            get { return sendTimes; }
            set 
            {
                if (SetProperty(ref sendTimes, value))
                {
                    RaisePropertyChanged(nameof(ConfigString));
                }
            }
        }

        public string ConfigString => $"{PackageSize}M/Package, {SendTimes} times ";


        public ICommand gRPCStartCmd => _gRPCStartCmd ?? (_gRPCStartCmd = new DelegateCommand(gRPCStartTry));
        public ICommand gRPCUnaryCmd => _gRPCUnaryCmd ?? (_gRPCUnaryCmd = new DelegateCommand(gRPCUnaryTry));
        public ICommand gRPCClientStreamCmd => _gRPCClientStreamCmd ?? (_gRPCClientStreamCmd = new DelegateCommand(gRPCClientStreamTry));
        public ICommand gRPCServerStreamCmd => _gRPCServerStreamCmd ?? (_gRPCServerStreamCmd = new DelegateCommand(gRPCServerStreamTry));
        public ICommand gRPCBothStreamCmd => _gRPCBothStreamCmd ?? (_gRPCBothStreamCmd = new DelegateCommand(gRPCBothStreamTry));
        public ICommand gRPCStartIPCCmd => _gRPCStartIPCCmd ?? (_gRPCStartIPCCmd = new DelegateCommand(gRPCStartIPCTry));
        public ICommand gRPCIPCBigDataBothStreamCmd => _gRPCIPCBigDataBothStreamCmd ?? (_gRPCIPCBigDataBothStreamCmd = new DelegateCommand(gRPCIPCBigDataBothStreamTry));
        public ICommand gRPCBigDataBothStreamCmd => _gRPCBigDataBothStreamCmd ?? (_gRPCBigDataBothStreamCmd = new DelegateCommand(gRPCBigDataBothStreamTry));

        public ICommand ClearResultCmd => clearResultCmd ?? (clearResultCmd = new DelegateCommand(ClearResultAction));

        #region Private part
        private async void gRPCStartTry()
        {
            var channel = GrpcChannel.ForAddress(DefaultChannelAddress);
            var client = new Greeter.GreeterClient(channel);

            var response = await client.SayHelloAsync(new HelloRequest { Name = "First gRPC" });

            AppendText(response.Message);
        }

        private async void gRPCUnaryTry()
        {
            AppendText("starting unary calling...");
            var channel = GrpcChannel.ForAddress(DefaultChannelAddress);
            var client = new StreamShoper.StreamShoperClient(channel);

            var request = new ExampleRequest { PageIndex = 2, PageSize = 10, IsDescending = true };

            var response = await client.UnaryCallAsync(request);
            AppendText(response.Message);
        }

        private async void gRPCClientStreamTry()
        {
            try
            {
                AppendText("starting Client Steam Calling...");
                var channel = GrpcChannel.ForAddress(DefaultChannelAddress);
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

        private async void gRPCServerStreamTry()
        {
            AppendText("starting server stream...");
            var channel = GrpcChannel.ForAddress(@"https://localhost:5001");
            var client = new StreamShoper.StreamShoperClient(channel);

            var request = new ExampleRequest { PageIndex = 2, PageSize = 10, IsDescending = true };
            var cancelSource = new CancellationTokenSource();
            var token = cancelSource.Token;

            using var call = client.StreamingFromServer(request, null, null, token);

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

        private async void gRPCBothStreamTry()
        {
            try
            {
                AppendText("starting Both Steam Calling...");
                var channel = GrpcChannel.ForAddress(DefaultChannelAddress);
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

        private async void gRPCStartIPCTry()
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

        private async void gRPCIPCBigDataBothStreamTry()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw1.Start();

            try
            {
                AppendText("starting  IPC big data Both Steam Calling...");
                var channel = GrpcConnectionFactory.CreateUdsChannel();
                var client = new StreamShoper.StreamShoperClient(channel);

                var cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                var call = client.SendingBigDataPackage(null, null, token);
                AppendText("Start to send data...");


                sw2.Start();

                AppendText($"Parameters: Send Times is {SendTimes}, Package size is {packageSize}M");

                for (int i = SendTimes; i >= 0; i--)
                {
                    AppendText($"#From Client# the index is {i}");
                    var request = new BigDataRequest { DataType = $"index{i}" };

                    var strTmp = new string('A', (int)(1024 * 1024 * packageSize));

                    var size = ((double)Encoding.UTF8.GetByteCount(strTmp) / (1024 * 1024)).ToString("f3");
                    AppendText($"Per item size is {size}M");

                    var times = 1; // total ~1M
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
                AppendText("Cancelled");
                sw2.Stop();

            }
            catch (Exception ex)
            {
                AppendText(ex.Message);
                AppendText("Exception, Cancelling...");
            }

            sw1.Stop();

            AppendText($"Sending data time cost:{sw2.Elapsed.TotalSeconds}");
            AppendText($"Total time cost:{sw1.Elapsed.TotalSeconds}");
        }


        private async void gRPCBigDataBothStreamTry()
        {
            Stopwatch sw1 = new Stopwatch();
            Stopwatch sw2 = new Stopwatch();

            sw1.Start();

            try
            {
                AppendText("starting Both Steam Calling...");
                var channel = GrpcChannel.ForAddress(DefaultChannelAddress);
                var client = new StreamShoper.StreamShoperClient(channel);

                var cancelSource = new CancellationTokenSource();
                var token = cancelSource.Token;

                var call = client.SendingBigDataPackage(null, null, token);
                AppendText("Start to send data...");


                sw2.Start();

                AppendText($"Parameters: Send Times is {SendTimes}, Package size is {packageSize}M");

                for (int i = SendTimes; i >= 0; i--)
                {
                    AppendText($"#From Client# the index is {i}");
                    var request = new BigDataRequest { DataType = $"index{i}" };

                    var strTmp = new string('A', (int)(1024 * 1024 * packageSize));

                    var size = ((double)Encoding.UTF8.GetByteCount(strTmp)/(1024*1024)).ToString("f3");
                    AppendText($"Per item size is {size}M");

                    var times = 1; // total ~1M
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
                AppendText("Cancelled");
                sw2.Stop();

            }
            catch (Exception ex)
            {
                AppendText(ex.Message);
                AppendText("Exception, Cancelling...");
            }

            sw1.Stop();

            AppendText($"Sending data time cost:{sw2.Elapsed.TotalSeconds}");
            AppendText($"Total time cost:{sw1.Elapsed.TotalSeconds}");
        }

        private void ClearResultAction()
        {
            ResultMsg = string.Empty;
        }

        private void AppendText(string text)
        {
            resultMsg += (Environment.NewLine + text);
            RaisePropertyChanged(nameof(ResultMsg));
        }

        #endregion


    }
}
