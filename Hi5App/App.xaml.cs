using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Hi5App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppStartup.Initial();
            // LoadCompleted += App_LoadCompleted;
        }

        //private void App_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        //{
        //    AppStartup.Initial();
        //}
    }
}
