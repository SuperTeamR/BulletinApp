using BulletinBridge.Data;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Service
{
    static class SendResultRouter
    {
        public static void BulletinWorkResult(IEnumerable<BulletinPackage> bulletins)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection((a) => { }, "AssignBulletinWork", objects: bulletins);
                }
            });
        }
        public static void AccessWorkResult(IEnumerable<AccessPackage> accesses)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection((a) => { }, "CheckAccess", objects: accesses);
                }
            });
        }
    }
}
