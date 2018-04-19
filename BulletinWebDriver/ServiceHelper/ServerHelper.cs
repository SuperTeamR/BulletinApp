using BulletinBridge.Data;
using BulletinWebDriver.Service;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.ServiceHelper
{
    static class ServerHelper
    {

        public static void SendDoneTasks(IEnumerable<TaskCache> tasks)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new EngineService())
                {
                    var r = client.Ping();
                    Console.WriteLine($"Ping = {r}");
                    client.SendQueryCollection((a) => { }, "Save", objects: tasks, sessionUID: d2._SessionInfo.SessionUID, hashUID: d2._SessionInfo.HashUID);
                }
            });
        }

        public static void GetNewTasks(Action<IEnumerable<TaskCache>> callback)
        {
            DCT.Execute(d =>
            {
                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");

                    client.SendQueryCollection<TaskCache>(callback, "Load", objects: Enumerable.Empty<TaskCache>(), sessionUID: d._SessionInfo.SessionUID, hashUID: d._SessionInfo.HashUID);
                }
            });
          
        }
    }
}
