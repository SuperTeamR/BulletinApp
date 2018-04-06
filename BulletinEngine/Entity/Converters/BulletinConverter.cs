using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Entity.Converters
{
    /// <summary>
    /// Конвертер буллетинов
    /// </summary>
    static class BulletinConverter
    {
        public static AggregateBulletinPackage Convert(Bulletin obj)
        {
            AggregateBulletinPackage result = null;
            BCT.Execute(d =>
            {
                var accesses = AccessHelper.GetUnusedAccesses(obj.Id).Where(q => q.State == (int)BulletinEngine.Entity.Data.AccessState.Cloning).ToArray();
                if(accesses != null && accesses.Count() > 0)
                {
                    var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.BulletinId == obj.Id);
                    var groupSignature = GroupHelper.GetGroupSignature(dbInstance.Id);
                    var valueFields = ValueFieldHelper.GetValueFields(dbInstance.Id);
                    var accessFields = AccessFieldHelper.GetAccessFields(dbInstance.Id);

                    var package = new BulletinPackage
                    {
                        BulletinId = obj.Id,
                        State = dbInstance.State,
                        Signature = groupSignature,
                        ValueFields = valueFields,
                        AccessFields = accessFields,
                    };
                    result = new AggregateBulletinPackage
                    {
                        Accesses = accesses,
                        Bulletin = package
                    };

                    // Костыль, но где-то надо проставить, что учетка меняет статус с "Cloning"
                    var ids = accesses.Select(qq => qq.Id).ToArray();
                    var takedAccesses = d.Db1.Accesses.Where(q => ids.Contains(q.Id)).ToArray();
                    foreach(var t in takedAccesses)
                    {
                        t.StateEnum = BulletinEngine.Entity.Data.AccessState.Created;
                    }

                    d.Db1.SaveChanges();
                }
            });
            return result;
        }

        public static Bulletin Convert(AggregateBulletinPackage obj)
        {
            Bulletin result = null;
            BCT.Execute(d =>
            {
                result = new Bulletin();
            });
            return result;
        }

    }
}
