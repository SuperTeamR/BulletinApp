using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XlsIntegration;

namespace BulletinEngine.Service.BoardApi
{
    public static class ClientApiHelper
    {
        //public static ResponseAddAccessModel AddAccess(RequestAddAccessModel request)
        //{
        //    ResponseAddAccessModel result = new ResponseAddAccessModel();
        //    BCT.Execute(d =>
        //    {
        //        var accessPackage = request.Objects.FirstOrDefault();
        //        var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == accessPackage.Login && q.Password == accessPackage.Password);
        //        if(dbAccess == null)
        //        {
        //            dbAccess = new Access
        //            {
        //                Login = accessPackage.Login,
        //                Password = accessPackage.Password,
        //            };

        //            d.Db1.Accesses.Add(dbAccess);
        //            d.SaveChanges();
        //        }
        //        //accessPackage.Id = dbAccess.Id;

        //        //d.Queue.Profiles.Enqueue(accessPackage.Id);

        //        result = new ResponseAddAccessModel
        //        {
        //            Objects = new[] { accessPackage },
        //            State = ResponseState.Success
        //        };
        //    });
        //    return result;
        //}



        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавляет буллетины в базу </summary>
        ///
        /// <remarks>   SV Milovanov, 08.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The ResponseBoardApi_AddBulletins. </returns>
        ///-------------------------------------------------------------------------------------------------
        //public static ResponseAddBulletinsModel AddBulletins(RequestAddBulletinsModel request)
        //{
        //    ResponseAddBulletinsModel result = new ResponseAddBulletinsModel();
        //    BCT.Execute(d =>
        //    {
        //        var bulletinPackages = request.Objects.Cast<BulletinPackage>();
        //        foreach (var bulletin in bulletinPackages)
        //        {
        //            //Получение группы
        //            var hash = bulletin.Signature.GetHash();
        //            var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);


        //            //Получение доступа
        //            var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == bulletin.Access.Login
        //                && q.Password == bulletin.Access.Password);
        //            if (dbAccess == null)
        //                continue;
        //            //Сохранение контейнера буллетинов
        //            var dbBulletin = new Bulletin
        //            {
        //                UserId = Guid.Empty,//d.Objects.CurrentUser.Id,
        //            };

        //            //dbBulletin._Save();
        //            d.Db1.SaveChanges();

        //            //Сохранение инстанций буллетинов для каждой борды
        //            var dbBoards = d.Db1.Boards.ToArray();
        //            foreach (var board in dbBoards)
        //            {
        //                var dbInstance = new BulletinInstance
        //                {
        //                    BoardId = board.Id,
        //                    AccessId = dbAccess.Id,
        //                    BulletinId = dbBulletin.Id,
        //                    GroupId = dbGroup.Id,
        //                };
        //                dbInstance.StateEnum = BulletinInstanceState.WaitPublication;
        //                //dbInstance._Save();
        //                d.Db1.SaveChanges();

        //                foreach (var field in bulletin.ValueFields)
        //                {
        //                    var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
        //                    if (dbField != null)
        //                    {
        //                        var dbBulletinField = new BulletinField
        //                        {
        //                            BulletinInstanceId = dbInstance.Id,
        //                            FieldId = dbField.Id,
        //                            Value = field.Value,
        //                        };
        //                        dbBulletinField.StateEnum = BulletinFieldState.Filled;
        //                        //d.Db1.BulletinFields.Add(dbBulletinField);
        //                        //dbBulletinField._Save();
        //                        d.Db1.SaveChanges();
        //                    }
        //                }
        //            }
        //            dbBulletin.StateEnum = Entity.Data.BulletinState.WaitPublication;

        //            d.Db1.SaveChanges();
        //        }
        //    });
        //    return result;

        //}



        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавляет буллетины в базу </summary>
        ///
        /// <remarks>   SV Milovanov, 08.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The ResponseBoardApi_AddBulletins. </returns>
        ///-------------------------------------------------------------------------------------------------

        //public static ResponseBoardApi_AddBulletins AddBulletins(RequestBoardApi_AddBulletins request)
        //{
        //    ResponseBoardApi_AddBulletins result = new ResponseBoardApi_AddBulletins();
        //    BCT.Execute(d =>
        //    {
        //        var bulletinPackages = request.Objects.Cast<BulletinPackage>();
        //        foreach(var bulletin in bulletinPackages)
        //        {
        //            //Получение группы
        //            var hash = bulletin.Signature.GetHash();
        //            var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash); 
                    
        //            //Сохранение контейнера буллетинов
        //            var dbBulletin = new Bulletin
        //            {
        //                UserId = Guid.Empty,//d.Objects.CurrentUser.Id,
        //            };

        //            //dbBulletin._Save();
        //            d.Db1.SaveChanges();

        //            //Сохранение инстанций буллетинов для каждой борды
        //            var dbBoards = d.Db1.Boards.ToArray();
        //            foreach(var board in dbBoards)
        //            {
        //                var dbInstance = new BulletinInstance
        //                {
        //                    BoardId = board.Id,
        //                    //На момент первого создания инстации Access пустой
        //                    AccessId = Guid.Empty,
        //                    BulletinId = dbBulletin.Id,
        //                    GroupId = dbGroup.Id,
        //                };
        //                dbInstance.StateEnum = BulletinInstanceState.WaitPublication;
        //                //dbInstance._Save();
        //                d.Db1.SaveChanges();

        //                foreach (var field in bulletin.ValueFields)
        //                {
        //                    var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
        //                    if(dbField != null)
        //                    {
        //                        var dbBulletinField = new BulletinField
        //                        {
        //                            BulletinInstanceId = dbInstance.Id,
        //                            FieldId = dbField.Id,
        //                            Value = field.Value,
        //                        };
        //                        dbBulletinField.StateEnum = BulletinFieldState.Filled;
        //                        //dbBulletinField._Save();
        //                        d.Db1.SaveChanges();
        //                    }
        //                }
        //            }
        //            dbBulletin.StateEnum = Entity.Data.BulletinState.WaitPublication;

        //            d.Db1.SaveChanges();
        //        }
              
        //    });
        //    return result;
        //}

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Редактирует буллетины в базе </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The ResponseBoardApi_EditBulletins. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static ResponseBoardApi_EditBulletins EditBulletins(RequestBoardAPI_EditBulletins request)
        {
            ResponseBoardApi_EditBulletins result = new ResponseBoardApi_EditBulletins();
            BCT.Execute(d =>
            {
                var bulletinPackages = request.Objects.Cast<BulletinPackage>();
                foreach (var bulletin in bulletinPackages)
                {
                    //Получение группы
                    var hash = bulletin.Signature.GetHash();
                    var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);

                    var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Id == bulletin.Id);

                    var dbBulletin = d.Db1.Bulletins.FirstOrDefault(q => q.Id == dbInstance.BulletinId);

                    foreach (var field in bulletin.ValueFields)
                    {
                        var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                        if (dbField != null)
                        {
                            var dbBulletinField = d.Db1.BulletinFields.FirstOrDefault(q => q.BulletinInstanceId == dbInstance.Id && q.FieldId == dbField.Id);
                            if(dbBulletinField == null)
                            {
                                dbBulletinField = new BulletinField
                                {
                                    BulletinInstanceId = dbInstance.Id,
                                    FieldId = dbField.Id,
                                };
                                d.Db1.BulletinFields.Add(dbBulletinField);
                                d.Db1.SaveChanges();
                            }
                            dbBulletinField.StateEnum = BulletinFieldState.Edited;
                            dbBulletinField.Value = field.Value;
                            d.Db1.SaveChanges();
                        }
                    }

                    dbInstance.StateEnum = BulletinInstanceState.Edited;
                    dbBulletin.StateEnum = Entity.Data.BulletinState.Edited;
                    
                    d.Db1.SaveChanges();
                }
            }); 
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Генерирует xls из группы </summary>
        ///
        /// <remarks>   SV Milovanov, 13.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The XLS for group. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static ResponseBoardAPI_GetXlsForGroup GetXlsForGroup(RequestBoardAPI_GetXlsForGroup request)
        {
            ResponseBoardAPI_GetXlsForGroup result = new ResponseBoardAPI_GetXlsForGroup();
            BCT.Execute(d =>
            {
                var hash = request.GroupSignature.GetHash();
                var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                var groupId = dbGroup.Id;

                var fieldIds = d.Db1.GroupedFields.Where(q => q.GroupId == groupId).Select(q => q.FieldId).ToArray();
                var dbFields = d.Db1.FieldTemplates.Where(q => fieldIds.Contains(q.Id)).ToArray();
                var fieldNames = dbFields.Select(q => q.Name).ToArray();

                var xls = XlsParser.CreateXls(fieldNames);
                result.Xls = xls;
                result.State = ResponseBoardAPI_GetXlsForGroupState.Success;
            });
            return result;
        }
    }
}
