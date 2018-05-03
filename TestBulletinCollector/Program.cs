using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletinCommand.Helpers;

namespace TestBulletinCollector
{
    class Program
    {
        static void Main(string[] args)
        {
            var caches = BulletinCollector.GetBulletinsByQuery(@"http://avito.ru", "iPhone");
            caches = BulletinCollector.GetBulletinsBySheets(caches);
        }
    }
}
