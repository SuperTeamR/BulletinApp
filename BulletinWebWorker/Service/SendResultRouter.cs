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
                    client.SendQueryCollection("Save", objects: bulletins);
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
                    client.SendQueryCollection("Save", objects: tasks);
                }
            });
        }

        public static void AccessBulletinListWorkDone(IEnumerable<TaskCache> tasks)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection("Save", objects: tasks);
                }
            });
        }

        public static void AccessBulletinDetailsWorkDone(IEnumerable<TaskCache> tasks)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection("Save", objects: tasks);
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

                    client.SendQueryCollection("AssignBulletinWork", objects: bulletins);
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
                    client.SendQueryCollection("CheckAccess", objects: accesses);
                }
            });
        }
    }
}
