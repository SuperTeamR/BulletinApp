using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System.Collections.Generic;
using System.Linq;
using BulletinHub.Tools;
using System;
using BulletinEngine.Helpers;

namespace BulletinHub.Helpers
{
    public static class BulletinHelper
    {
        public static void BulletinPublicate(Bulletin bulletin)
        {
            BCT.Execute(c =>
            {
                var tasks = c.TempDB.Tasks.Where(q => (q.State == (int)BulletinHub.Entity.Data.TaskState.Created || q.State == (int)BulletinHub.Entity.Data.TaskState.Enabled) && q.BulletinId == bulletin.Id && q.Command == (int)BulletinHub.Entity.Data.TaskCommand.InstancePublication).ToArray();
                if (tasks != null && tasks.Any())
                    TaskHelper.Remove(tasks);
                var instances = BulletinHelper.CreateInstance(bulletin);
                if (instances.Any())
                {
                    bulletin.SetGenerationCheck();
                    var datePublishs = c.TempDB.Tasks.Where(q => q.Command == (int)BulletinHub.Entity.Data.TaskCommand.InstancePublication).Select(q => q.TargetDate).ToArray();
                    datePublishs = datePublishs.Where(q => q.HasValue).ToArray();
                    var datePublish = DateTime.Now;
                    if (datePublishs.Any())
                    {
                        var max = datePublishs.Max().Value;
                        if (datePublish > DateTime.Now)
                            datePublish = max;
                    }
                    datePublish = datePublish.AddMinutes(5);
                    foreach (var instance in instances)
                    {
                        var access = AccessHelper.GetNextAccess(bulletin.UserId, instance.BoardId, instance.BulletinId);
                        if (access == null)
                            continue;
                        instance.AccessId = access.Id;
                        TaskHelper.CreateInstancePublication(bulletin.UserId, instance, datePublish);
                    }
                }
            });
        }
        /// <summary>
        /// Создаёт базовую инстанцию без распределения по доступу
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<BulletinInstance> CreateInstance(Bulletin bulletin)
        {
            var result = new List<BulletinInstance>();
            BCT.Execute(c =>
            {
                var boards = c.BulletinDb.Boards.ToArray();
                foreach (var board in boards)
                {
                    var instance = new BulletinInstance();
                    instance.BoardId = board.Id;
                    instance.BulletinId = bulletin.Id;
                    instance.GroupId = bulletin.GroupId.Value;
                    instance.StateEnum = BulletinInstanceState.Created;
                    result.Add(instance);
                }
                c.SaveChanges();
            });
            return result;
        }


        public static IEnumerable<Bulletin> Create(IEnumerable<Bulletin> objs)
        {
            var result = Enumerable.Empty<Bulletin>().ToList();
            BCT.Execute(d =>
            {
                var bulletins = objs.ToArray();
                for (int i = 0; i < objs.Count(); i++)
                {
                    var currentBulletin = bulletins[i];
                    currentBulletin.StateEnum = BulletinState.Created;
                    result.Add(currentBulletin);
                }
                d.BulletinDb.SaveChanges();
            });
            return result;
        }

        public static IEnumerable<Bulletin> All()
        {
            var result = Enumerable.Empty<Bulletin>();
            BCT.Execute(c =>
            {
                result = c.BulletinDb.Bulletins.Where(q => q.UserId == c.UserId).ToArray();
            });
            return result;
        }

        public static Bulletin Edit(Bulletin bulletin)
        {
            BCT.Execute(d =>
            {
                var instance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.BulletinId == bulletin.Id);
                instance.StateEnum = BulletinInstanceState.Edited;
                d.SaveChanges();
            });
            return bulletin;
        }
        public static Bulletin AddAvito(Bulletin model)
        {
            BCT.Execute(c =>
            {
                model.UserId = c.UserId;
                model.StateEnum = BulletinEngine.Entity.Data.BulletinState.Created;
                c.SaveChanges();
            });
            return model;
        }
        public static void Remove(IEnumerable<Bulletin> entities)
        {
            BCT.Execute(c =>
            {
                foreach (var entity in entities)
                    c.BulletinDb.Bulletins.Remove(entity);
                c.SaveChanges();
            });
        }

        public static Bulletin Publicate(Bulletin model)
        {
            BCT.Execute(d =>
            {


            });
            return model;
        }
    }
}