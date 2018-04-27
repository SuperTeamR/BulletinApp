using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinCommand.Helpers
{
    public static class CommandTaskHelper
    {
        public static void Remove(IEnumerable<BulletinHub.Entity.Data.Task> entities)
        {
            BCT.Execute(c => 
            {
                foreach (var task in entities)
                    task.StateEnum = BulletinHub.Entity.Data.TaskState.Disabled;
                c.SaveChanges();
            });
        }
        public static BulletinHub.Entity.Data.Task Create(TaskCommand command, Action<BulletinHub.Entity.Data.Task> action)
        {
            var result = new BulletinHub.Entity.Data.Task();
            BCT.Execute(c =>
            {
                result.TargetDate = DateTime.Now;
                result.CommandEnum = command;
                action?.Invoke(result);
                result.StateEnum = BulletinHub.Entity.Data.TaskState.Enabled;
                c.SaveChanges();
            });
            return result;
        }
        public static void CreateAccessCheck(Guid userId, Guid accessId)
        {
            Create(TaskCommand.AccessCheck, a => 
            {
                a.AccessId = accessId;
                a.UserId = userId;
            });
        }

        public static void CreateInstancePublication(Guid userId, BulletinInstance instance, DateTime publicationDate)
        {
            Create(TaskCommand.InstancePublication, a =>
            {
                a.UserId = userId;
                a.AccessId = instance.AccessId;
                a.BulletinId = instance.BulletinId;
                a.InstanceId = instance.Id;
                a.TargetDate = publicationDate;
            });
        }
    }
}
