using Grpc.Net.Client;
using Grpc.Net.Client.Configuration;
using GrpcShopClient;
using Hi5App.ViewModels.Interface;
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
        public IgRPCShopViewModel ViewModel;

        public gRPCShop()
        {
            InitializeComponent();

            ViewModel = Hi5AppServiceProvider.GetService<IgRPCShopViewModel>();
            DataContext = ViewModel;
        }

    }
}
