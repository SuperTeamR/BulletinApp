﻿using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System.Collections.Generic;
using System.Linq;
using BulletinHub.Tools;

namespace BulletinHub.Helpers
{
    static class BulletinHelper
    {
        public static void GenerateTask()
        {
            BCT.Execute(c =>
            {
                TaskGenerator.GenerateTasks(c.UserId);
            });
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
    }
}