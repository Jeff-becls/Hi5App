using Hi5App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi5App.ViewModels.Interface
{
    public interface IMainWindowViewModel
    {
        ObservableCollection<Hi5MenuItem> MenuItems { get; set; }
    }
}
