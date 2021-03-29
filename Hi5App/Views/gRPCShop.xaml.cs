using Grpc.Net.Client;
using GrpcShopClient;
using System;
using System.Collections.Generic;
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

            ResultTextBox.Text = response.Message;

        }

        private async void gRPCServerStreamButton_Click(object sender, RoutedEventArgs e)
        {
            ResultTextBox.Text += (Environment.NewLine + "starting server stream...");
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
                    ResultTextBox.Text += (Environment.NewLine + response.Message);
                    if (response.TotalCount >= 10)
                    {
                        ResultTextBox.Text += (Environment.NewLine + "Meet max, cancelling...");
                        cancelSource.Cancel();
                    }
                }
                catch (Exception ex)
                {
                    ResultTextBox.Text += (Environment.NewLine + "Exit...");
                    break;
                }
            }
            

        }
    }
}
