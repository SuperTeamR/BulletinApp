using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Helpers
{
    public static class TaskHelper
    {
        #region Back logic
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
        public static void CreateAccessCheck(Access access)
        {
            Create(TaskCommand.AccessCheck, a =>
            {
                //Обязательные
                a.BoardId = access.BoardId;
                a.UserId = access.UserId;

                a.AccessId = access.Id;
            });
        }

        public static void CreateActivateAccess(Access access)
        {
            Create(TaskCommand.ActivateAccess, a =>
            {  
                //Обязательные
                a.BoardId = access.BoardId;
                a.UserId = access.UserId;

                a.AccessId = access.Id;
            });
        }
        public static void CreateActivateInstance(Guid userId, BulletinInstance instance, DateTime publicationDate)
        {
            Create(TaskCommand.ActivateInstance, a =>
            {
                //Обязательные
                a.BoardId = instance.BoardId;
                a.UserId = userId;

                a.AccessId = instance.AccessId;
                a.InstanceId = instance.Id;
                a.TargetDate = publicationDate;
            });
        }

        public static void CreateAccessStatistics(Access access)
        {
            Create(TaskCommand.AccessStatistics, a =>
            {
                //Обязательные
                a.BoardId = access.BoardId;
                a.UserId = access.UserId;

                a.AccessId = access.Id;
            });
        }

        public static void CreateInstanceStatistics(Guid userId, BulletinInstance instance)
        {
            Create(TaskCommand.InstanceStatistics, a =>
            {
                //Обязательные
                a.BoardId = instance.BoardId;
                a.UserId = userId;

                a.InstanceId = instance.Id;
            });
        }

        public static void CreateInstancePublication(Guid userId, BulletinInstance instance, DateTime publicationDate)
        {
            Create(TaskCommand.InstancePublication, a =>
            {
                //Обязательные
                a.UserId = userId;
                a.BoardId = instance.BoardId;

                a.AccessId = instance.AccessId;
                a.BulletinId = instance.BulletinId;
                a.InstanceId = instance.Id;
                a.TargetDate = publicationDate;
            });
        }

        public static void CreateAccessRegistration(Access access)
        {
            Create(TaskCommand.Registration, a =>
            {
                //Обязательные
                a.UserId = access.UserId;
                a.AccessId = access.Id;
                a.BoardId = access.BoardId;
                a.TargetDate = DateTime.Now;
            });
        }

        public static void CreateBulletinTemplateCollector(Guid userId, Guid boardId, Bulletin instance, DateTime publicationDate)
        {
            Create(TaskCommand.BulletinTemplateCollector, a =>
            {
                //Обязательные
                a.UserId = userId;
                a.BulletinId = instance.Id;
                a.TargetDate = publicationDate;
                a.BoardId = boardId;
            });
        }
        #endregion
    }
}
