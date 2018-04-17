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
        public static void BulletinWorkDone(IEnumerable<BulletinPackage> bulletins)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    //client.Save((a) => { }, bulletins);
                    client.SendQueryCollection((a) => { }, "Save", objects: bulletins, sessionUID: d2._SessionInfo.SessionUID, hashUID: d2._SessionInfo.HashUID);
                }
            });
        }

        public static void TaskWorkDone(IEnumerable<TaskCache> tasks)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection((a) => { }, "Save", objects: tasks, sessionUID: d2._SessionInfo.SessionUID, hashUID: d2._SessionInfo.HashUID);
                    //client.Save((a) => { }, tasks);
                }
            });
        }


        [Obsolete]
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
        [Obsolete]
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
