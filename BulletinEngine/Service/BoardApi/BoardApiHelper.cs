using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Service.BoardApi
{
    public static class BoardApiHelper
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавляет буллетины в базу с состоянием New </summary>
        ///
        /// <remarks>   SV Milovanov, 08.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The ResponseBoardApi_AddBulletins. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static ResponseBoardApi_AddBulletins AddBulletins(RequestBoardApi_AddBulletins request)
        {
            ResponseBoardApi_AddBulletins result = new ResponseBoardApi_AddBulletins();
            BCT.Execute((d) =>
            {
                var bulletinPackages = request.Objects.Cast<BulletinPackage>();
                foreach(var bulletin in bulletinPackages)
                {
                    //Получение группы
                    var hash = bulletin.Signature.GetHash();
                    var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == hash); 
                    
                    //Сохранение контейнера буллетинов
                    var dbBulletin = new Bulletin
                    {
                        UserId = Guid.Empty,//d.Objects.CurrentUser.Id,
                    };

                    //dbBulletin._Save();
                    d.Db1.SaveChanges();
                    

                    //Сохранение инстанций буллетинов для каждой борды
                    var dbBoards = d.Db1.Boards.ToArray();
                    foreach(var board in dbBoards)
                    {
                        var dbInstance = new BulletinInstance
                        {
                            BoardId = board.Id,
                            //На момент первого создания инстации Access пустой
                            AccessId = Guid.Empty,
                            BulletinId = dbBulletin.Id,
                            GroupId = dbGroup.Id,
                        };
                        dbInstance.StateEnum = BulletinInstanceState.PreparedPublicated;
                        //dbInstance._Save();
                        d.Db1.SaveChanges();

                        foreach (var field in bulletin.ValueFields)
                        {
                            var dbField = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                            if(dbField != null)
                            {
                                var dbBulletinField = new BulletinField
                                {
                                    BulletinInstanceId = dbInstance.Id,
                                    FieldId = dbField.Id,
                                    Value = field.Value,
                                };
                                dbBulletinField.StateEnum = BulletinFieldState.Filled;
                                //dbBulletinField._Save();
                                d.Db1.SaveChanges();
                            }
                        }

                        
                    }
                    dbBulletin.StateEnum = Entity.Data.BulletinState.WaitPublication;

                    d.Db1.SaveChanges();
                }
              
            });
            return result;
        }
    }
}
