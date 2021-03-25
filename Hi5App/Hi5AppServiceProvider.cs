using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hi5App.ViewModels.Interface;
using Hi5App.ViewModels;

namespace Hi5App
{
    internal class Hi5AppServiceProvider
    {
        private static ServiceProvider serviceProvider;

        public Hi5AppServiceProvider()
        {
            Configure();
        }

        public static ServiceProvider ServiceProvider
        {
            get => serviceProvider;
            set => serviceProvider = value;
        }

        private readonly ServiceCollection Services = new ServiceCollection();

        public void Configure()
        {
            ConfigureServices();
        }

        public static T GetService<T>()
        {
            return serviceProvider.CreateScope().ServiceProvider.GetService<T>();
        }

        private void ConfigureServices()
        {
            Services.Clear();

            Services.AddSingleton<IMainWindowViewModel, MainWindowViewModel>();

            serviceProvider = Services.BuildServiceProvider();

        }
    }
}
