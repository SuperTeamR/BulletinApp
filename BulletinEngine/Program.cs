using BulletinBridge;
using BulletinEngine.Core;
using BulletinEngine.Service;
using FessooFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper<BulletinEngine.Bootstrapper>.Current.Run();

            UdpManager.Set("127.0.0.1", 5052, 5051);
            BCT.ExecuteAsync(d =>
            {
                UdpManager.Receive(ServiceRouter.ExecuteRouting);
            });
           


            Console.Read();
        }
    }
}
