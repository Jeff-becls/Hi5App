using Hi5App.Models;
using Hi5App.ViewModels;
using Hi5App.ViewModels.Interface;
using Hi5App.Views;
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

namespace Hi5App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IMainWindowViewModel ViewModel;
        public MainWindow()
        {
            InitializeComponent();

            // ViewModel = Hi5AppServiceProvider.GetService<IMainWindowViewModel>();
            DataContext = new MainWindowViewModel();

            MainFrame.NavigationUIVisibility = NavigationUIVisibility.Hidden;
            MainFrame.Navigate(new MainPage());
        }

        private void MenuListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems[0] is Hi5MenuItem menuItem)
            {
                var uriString = string.Format(Hi5Constants.URIStringTemplate, menuItem.Tag);
                MainFrame.Navigate(new Uri(uriString, UriKind.Relative));
            }
        }
    }
}
