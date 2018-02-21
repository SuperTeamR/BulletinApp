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
                    if (string.IsNullOrEmpty(bulletin.Url))
                    {
                        //Получение группы
                        var hash = bulletin.Signature.GetHash();
                        var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);

                        //Сохранение контейнера буллетинов
                        var dbBulletin = new Bulletin
                        {
                            UserId = Guid.Empty,//d.Objects.CurrentUser.Id,
                        };
                        d.Db1.SaveChanges();

                        var access = bulletin.Access;
                        var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == access.Login && q.Password == access.Password);

                        var dbBoard = d.Db1.Boards.FirstOrDefault(q => q.Name == "Avito");

                        var dbInstance = new BulletinInstance
                        {
                            BoardId = dbBoard.Id,
                            AccessId = dbAccess.Id,
                            BulletinId = dbBulletin.Id,
                            GroupId = dbGroup.Id,
                            State = bulletin.State,
                        };
                        d.Db1.SaveChanges();
                        if(bulletin.ValueFields == null)
                        {
                            //d.Queue.Bulletins.Enqueue(dbInstance.Id);
                        }
                        else
                        {
                            foreach (var field in bulletin.ValueFields)
                            {
                                var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                                if (dbField != null)
                                {
                                    var dbBulletinField = new BulletinField
                                    {
                                        BulletinInstanceId = dbInstance.Id,
                                        FieldId = dbField.Id,
                                        Value = field.Value,
                                    };
                                    dbBulletinField.StateEnum = BulletinFieldState.Filled;
                                    d.Db1.BulletinFields.Add(dbBulletinField);
                                    d.Db1.SaveChanges();
                                }
                            }
                        }
                    }
                    else
                    {
                        var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Url == bulletin.Url);
                        dbInstance.State = bulletin.State;

                        foreach (var field in bulletin.ValueFields)
                        {
                            var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                            if (dbField != null)
                            {
                                var dbBulletinField = d.Db1.BulletinFields.FirstOrDefault(q => q.BulletinInstanceId == dbInstance.Id
                                    && q.FieldId == dbField.Id);
                                if (dbBulletinField != null)
                                {
                                    dbBulletinField.Value = field.Value;
                                    dbBulletinField.StateEnum = BulletinFieldState.Edited;
                                }
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
