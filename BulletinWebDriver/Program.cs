using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            DCT.Execute(d =>
            {
                Bootstrapper.Current.Run();
                WebWorkerManager.Execute();
                Console.ReadLine();
            });
        }
    }
}
