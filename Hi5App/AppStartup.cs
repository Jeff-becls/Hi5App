using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hi5App
{
    public static class AppStartup
    {
        public static void Initial()
        {
            Hi5AppServiceProvider serviceProvide = new Hi5AppServiceProvider();


            serviceProvide.Configure();
        
        }
    }
}
