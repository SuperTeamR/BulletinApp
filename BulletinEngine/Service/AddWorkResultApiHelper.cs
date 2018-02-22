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
        public static ResponseAddBulletinListWorkModel AddWorkResult(RequestAddBulletinListWorkModel request)
        {
            ResponseAddBulletinListWorkModel result = null;
            BCT.Execute(d =>
            {
                var bulletins = request.Objects;
                foreach (var bulletin in bulletins)
                {
                    //Еще не создано или не добавлено в БД
                    if (bulletin.BulletinInstanceId == Guid.Empty
                    && d.Db1.BulletinInstances.FirstOrDefault(q => q.Url == bulletin.Url) == null)
                    {
                        //Получение группы
                        var hash = bulletin.Signature.GetHash();
                        var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);

                        var access = bulletin.Access;
                        var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == access.Login && q.Password == access.Password);

                        var dbBoard = d.Db1.Boards.FirstOrDefault(q => q.Name == "Avito");

                        var dbBulletin = new Bulletin
                        {
                            UserId = dbAccess.UserId,
                        };
                        d.Db1.Bulletins.Add(dbBulletin);
                        d.Db1.SaveChanges();

                        var dbInstance = new BulletinInstance
                        {
                            BoardId = dbBoard.Id,
                            AccessId = dbAccess.Id,
                            BulletinId = dbBulletin.Id,
                            GroupId = dbGroup.Id,
                            Url = bulletin.Url,
                            State = bulletin.State
                        };
                        d.Db1.BulletinInstances.Add(dbInstance);
                        d.Db1.SaveChanges();


                        if(bulletin.ValueFields == null)
                        {
                            dbInstance.State = (int)BulletinInstanceState.Unchecked;
                            d.Db1.SaveChanges();
                        }
                        else
                        {
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
                                    dbBulletinField.StateEnum = BulletinFieldState.Filled;
                                    d.Db1.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Id == bulletin.BulletinInstanceId);
                        if(dbInstance == null)
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
                }
            });
            return result;
        }
    }
}
