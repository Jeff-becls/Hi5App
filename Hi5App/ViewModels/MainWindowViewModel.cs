using Hi5App.Models;
using Hi5App.ViewModels.Interface;
using Hi5App.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi5App.ViewModels
{
   public class MainWindowViewModel : IMainWindowViewModel
    {
        public MainWindowViewModel()
        {
            InitialMenuItems();
        }

        public ObservableCollection<Hi5MenuItem> MenuItems { get; set; }

        private void InitialMenuItems()
        {
            if (MenuItems == null)
            {
                MenuItems = new ObservableCollection<Hi5MenuItem>();
            }

            MenuItems.Clear();

            MenuItems.Add(new Hi5MenuItem { Name = "Main", Text = "Main Page", Tag = nameof(MainPage) });
            MenuItems.Add(new Hi5MenuItem { Name = "gRPC", Text = "gRPC Page", Tag = nameof(gRPCShop) });




        }
    }
}
