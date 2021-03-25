using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var channl = GrpcChannel.ForAddress(@"https://localhost:5001");
            var client = new Greeter.GreeterClient(channl);

            var response = await client.SayHelloAsync(new HelloRequest { Name = "First gRPC" });

            ResultTextBox.Text = response.Message;


        }
    }
}
