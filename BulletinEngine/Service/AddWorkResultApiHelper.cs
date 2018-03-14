using BulletinBridge.Data;
using BulletinBridge.Messages.InternalApi;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Service
{
    public static class AddWorkResultApiHelper
    {
        public static IEnumerable<BulletinPackage> AddWorkResult(IEnumerable<BulletinPackage> bulletins)
        {
            var result = Enumerable.Empty<BulletinPackage>();
            BCT.Execute(d =>
            {
                foreach (var bulletin in bulletins)
                {
                    var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Id == bulletin.BulletinInstanceId);
                    if (dbInstance == null)
                    {
                        dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Url == bulletin.Url);
                        if (dbInstance == null) continue;
                    }
                    dbInstance.State = bulletin.State;
                    dbInstance.Url = bulletin.Url;
                    d.Db1.SaveChanges();

                    if (bulletin.ValueFields == null) continue;
                    foreach (var field in bulletin.ValueFields)
                    {
                        var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                        if (dbField != null)
                        {
                            var dbBulletinField = d.Db1.BulletinFields.FirstOrDefault(q => q.BulletinInstanceId == dbInstance.Id
                                && q.FieldId == dbField.Id);
                            if (dbBulletinField == null)
                            {
                                dbBulletinField = new BulletinField
                                {
                                    BulletinInstanceId = dbInstance.Id,
                                    FieldId = dbField.Id,
                                };
                                d.Db1.BulletinFields.Add(dbBulletinField);
                                d.Db1.SaveChanges();
                            }
                            dbBulletinField.Value = field.Value;
                            dbBulletinField.StateEnum = bulletin.State == (int)BulletinInstanceState.Edited
                            ? BulletinFieldState.Edited
                            : BulletinFieldState.Filled;

                            d.Db1.SaveChanges();
                        }
                    }
                }
            });
            return result;
        }
    }
}
