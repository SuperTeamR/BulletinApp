using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Helpers
{
    public static class BulletinHelper
    {
        public static IEnumerable<BulletinPackage> CreateBulletins(IEnumerable<BulletinPackage> objs)
        {
            var result = objs;
            BCT.Execute(d=> {
                foreach (var obj in objs)
                {
                    var state = obj.State;
                    var hash = obj.Signature.GetHash();
                    var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                    var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == obj.Access.Login
                          && q.Password == obj.Access.Password);
                    var dbBulletin = new Bulletin
                    {
                        UserId = Guid.Empty,
                    };
                    dbBulletin.StateEnum = BulletinEngine.Entity.Data.BulletinState.WaitPublication;
                    var board = d.Db1.Boards.FirstOrDefault(q => q.Name == "Avito");
                    var model = new BulletinInstance
                    {
                        Url = obj.Url,
                        State = state,
                        GroupId = dbGroup.Id,
                        AccessId = dbAccess == null ? Guid.Empty : dbAccess.Id,
                        BulletinId = dbBulletin.Id,
                        BoardId = board.Id
                    };
                    model.StateEnum = BulletinEngine.Entity.Data.BulletinInstanceState.WaitPublication;
                    foreach (var field in obj.ValueFields)
                    {
                        var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                        if (dbField != null)
                        {
                            var dbBulletinField = new BulletinField
                            {
                                BulletinInstanceId = model.Id,
                                FieldId = dbField.Id,
                                Value = field.Value,
                            };
                            dbBulletinField.StateEnum = BulletinFieldState.Filled;
                        }
                    }
                }
                d.SaveChanges();
            });
            return result;
        }
    }
}
