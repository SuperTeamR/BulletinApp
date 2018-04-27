using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinWebDriver.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class DriverTaskHelper
    {
        public static TaskCache Next()
        {
            return DCT.Execute(c => c.HubClient.RSendQueryObject<TaskCache>("Next"));
        }

        public static void Complete(TaskCache task)
        {
            DCT.Execute(c => c.HubClient.RSendQueryObject("Complete", obj: task));
        }
        public static void Error(TaskCache task, string error)
        {
            task.Error = error;
            DCT.Execute(c => c.HubClient.RSendQueryObject("Error", obj: task));
        }
        public static T GetTask<T>(TaskCache task)
            where T : CacheObject, new()
        {
            var t = new T();
            t.SetClone(task.Id, task.CreateDate);
           return DCT.Execute(c => c.HubClient.RSendQueryObject("GetTask", obj: t));
        }
    }
}
