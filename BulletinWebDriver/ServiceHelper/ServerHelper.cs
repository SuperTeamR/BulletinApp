using BulletinBridge.Data;
using BulletinWebDriver.Service;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;


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
                    client.SendQueryCollection("Save", objects: tasks);
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
                    client.SendQueryCollection("Load", callback);
                }
            });
        }
    }
}