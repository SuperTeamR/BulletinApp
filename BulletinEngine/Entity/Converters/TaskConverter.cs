using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using System.Linq;

namespace BulletinHub.Entity.Converters
{
    class TaskConverter
    {
        public static TaskCache Convert(Data.Task obj)
        {
            TaskCache result = null;
            BCT.Execute(d =>
            {
                var bulletinPackage = new BulletinPackage();
                var accessPackage = new AccessPackage();

                if (obj.TargetType == typeof(BulletinInstance).ToString())
                {
                    var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == obj.BulletinId);
                    var dbAccess = d.BulletinDb.Accesses.FirstOrDefault(q => q.Id == obj.AccessId);
                    var groupSignature = GroupHelper.GetGroupSignature2(dbInstance.BulletinId);
                    var valueFields = ValueFieldHelper.GetValueFields2(dbInstance.BulletinId);
                    var accessFields = AccessFieldHelper.GetAccessFields2(dbInstance.BulletinId);

                    bulletinPackage.Signature = groupSignature;
                    bulletinPackage.ValueFields = valueFields;
                    bulletinPackage.AccessFields = accessFields;
                    bulletinPackage.Access = Access.ToCache(dbAccess);
                    bulletinPackage.State = dbInstance.State;
                }
                else if(obj.TargetType == typeof(Access).ToString())
                {

                }
                result = new TaskCache
                {
                    BulletinId = obj.BulletinId,
                    AccessId = obj.AccessId,
                    TargetType = obj.TargetType,
                    TargetTime = obj.TargetDate,
                    Command = obj.Command,
                    BulletinPackage = bulletinPackage,
                    AccessPackage = accessPackage,
                };
            });
            return result;
        }
        public static Data.Task Convert(TaskCache obj, Data.Task entity)
        {
            var result = default(Data.Task);
            BCT.Execute(d =>
            {
                if(obj.BulletinPackage != null)
                {
                    var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == obj.BulletinId);
                    dbInstance.State = obj.BulletinPackage.State;
                    dbInstance.Url = obj.BulletinPackage.Url;
                    d.SaveChanges();
                }

                entity.BulletinId = obj.BulletinId;
                entity.AccessId = obj.AccessId;
                entity.TargetType = obj.TargetType;
                entity.TargetDate = obj.TargetTime;
                entity.State = obj.State;

                result = entity;
            });
            return entity;
        }
    }
}