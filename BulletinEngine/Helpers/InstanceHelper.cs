using System;
using System.Collections.Generic;
using System.Linq;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Entity.Data;

namespace BulletinHub.Helpers
{
    public class InstanceHelper
    {
        public static void GetInstanceStatistics(BulletinInstance instance)
        {
            BCT.Execute(d =>
            {
                var bulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == instance.BulletinId);
                TaskHelper.CreateInstanceStatistics(bulletin.UserId, instance);
            });
        }

        public static IEnumerable<BulletinInstance> All()
        {
            var result = Enumerable.Empty<BulletinInstance>();
            BCT.Execute(d => { result = d.BulletinDb.BulletinInstances.ToArray(); });
            return result;
        }
    }
}