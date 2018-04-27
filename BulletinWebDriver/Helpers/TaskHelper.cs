using BulletinBridge.Data;
using BulletinWebDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class TaskHelper
    {
        public static void Execute()
        {
            DCT.Execute(c =>
            {

            });
        }

        public static void Complete(IEnumerable<TaskCache> tasks)
        {
            DCT.ExecuteAsync(d2 =>
            {
                d2.HubClient.SendQueryCollection("Save", objects: tasks);
            });
        }
        static void Next(Action<IEnumerable<TaskCache>> callback)
        {
            DCT.Execute(d =>
            {
                d.HubClient.SendQueryCollection("Load", callback);
            });
        }
    }
}
