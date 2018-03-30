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
                    var dbUser = d.Db1.Users.FirstOrDefault(q => q.Id == dbAccess.UserId);
                    var dbBulletin = new Bulletin
                    {
                        UserId = dbUser.Id,
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
                        if (dbInstance == null)
                        {
                            var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == bulletin.Access.Login
                                    && q.Password == bulletin.Access.Password);
                            var dbUser = d.Db1.Users.FirstOrDefault(q => q.Id == dbAccess.UserId);
                            var board = d.Db1.Boards.FirstOrDefault(q => q.Name == "Avito");


                            Bulletin dbBulletin = null;
                            if (bulletin.BulletinId == Guid.Empty)
                            {
                                dbBulletin = new Bulletin
                                {
                                    UserId = dbUser.Id,
                                };
                                dbBulletin.StateEnum = BulletinEngine.Entity.Data.BulletinState.Created;
                            }
                            else
                            {
                                dbBulletin = d.Db1.Bulletins.FirstOrDefault(q => q.Id == bulletin.BulletinId);
                                dbBulletin.StateEnum = BulletinEngine.Entity.Data.BulletinState.Created;
                            }
                            var hash = bulletin.Signature.GetHash();
                            var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                            dbInstance = new BulletinInstance
                            {
                                Url = bulletin.Url,
                                GroupId = dbGroup.Id,
                                AccessId = dbAccess == null ? Guid.Empty : dbAccess.Id,
                                BulletinId = dbBulletin.Id,
                                BoardId = board.Id
                            };
                            dbInstance.StateEnum = BulletinEngine.Entity.Data.BulletinInstanceState.Unchecked;
                            
                        }
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
