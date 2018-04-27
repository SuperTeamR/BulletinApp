using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Entity.Data;
using BulletinHub.Helpers;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinHub.Tools
{
    public static class BackTaskHelper
    {
        #region Task managment
        public static BulletinHub.Entity.Data.Task Next()
        {
            BulletinHub.Entity.Data.Task task = null;
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                task = c.TempDB.Tasks.OrderBy(q => q.TargetDate).FirstOrDefault(q => (q.State == (int)TaskState.Created || q.State == (int)TaskState.Enabled) && q.TargetDate < date);
                if (task.BulletinId == Guid.Empty)
                {
                    task.SetError("Task BoardId is empty");
                    task = Next();
                }
            });
            return task;
        }
        public static void Complite(IEnumerable<BulletinHub.Entity.Data.Task> tasks)
        {
            BCT.Execute(c =>
            {
                foreach (var task in tasks)
                {
                    var t = c.TempDB.Tasks.Find(task.Id);
                    t.SetComplete();
                }
                c.SaveChanges();
            });
        }
        public static void Error(IEnumerable<BulletinHub.Entity.Data.Task> tasks)
        {
            BCT.Execute(c =>
            {
                foreach (var task in tasks)
                    task.SetError(task.ErrorDescription);
                c.SaveChanges();
            });
        }
        #endregion
        #region Task variations

        #endregion
    }
}