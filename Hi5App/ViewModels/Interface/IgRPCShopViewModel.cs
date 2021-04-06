using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hi5App.ViewModels.Interface
{
    public interface IgRPCShopViewModel
    {
        string ResultMsg { get; set; }
        double PackageSize { get; set; }
        int SendTimes { get; set; }

        ICommand gRPCStartCmd { get; }
        ICommand gRPCUnaryCmd { get; }
        ICommand gRPCClientStreamCmd { get; }
        ICommand gRPCServerStreamCmd { get; }
        ICommand gRPCBothStreamCmd { get; }
        ICommand gRPCStartIPCCmd { get; }
        ICommand gRPCBigDataBothStreamCmd { get; }

        string ConfigString { get; }
    }
}
