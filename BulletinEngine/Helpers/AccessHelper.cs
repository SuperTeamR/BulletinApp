using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Helpers
{
    public static class AccessHelper
    {
        public static IEnumerable<Access> All()
        {
            var result = Enumerable.Empty<Access>();
            BCT.Execute(c=> 
            {
                result = c.BulletinDb.Accesses.Where(q => q.UserId == c.UserId).ToArray();
            });
            return result;
        }
        internal static Access AddAvito(Access access)
        {
            BCT.Execute(c =>
            {
                var board = c.BulletinDb.Boards.FirstOrDefault(q=>q.Name == "Avito");
                access.BoardId = board.Id;
                access.UserId = c.UserId;
                access.StateEnum = Entity.Data.AccessState.Created;
                c.SaveChanges();
            });
            return access;
        }
        internal static void Remove(IEnumerable<Access> entities)
        {
            BCT.Execute(c =>
            {
                foreach (var entity in entities)
                    c.BulletinDb.Accesses.Remove(entity);
                c.SaveChanges();
            });
        }
        public static Guid? GetActiveAccessId(Guid bulletinId)
        {
            Guid? result = null;
            BCT.Execute(d =>
            {
                var allAccesses = d.BulletinDb.Accesses.Where(q => q.UserId == d.UserId).ToArray();
                var bulletins = d.BulletinDb.BulletinInstances.Where(q => q.State == (int)BulletinInstanceState.WaitPublication).ToArray();
                if(bulletins.Any())
                {
                    var usedAccesses = bulletins.Select(q => q.AccessId).Distinct().ToArray();
                    var freeAccesses = allAccesses.Where(q => !usedAccesses.Contains(q.Id)).ToArray();
                    
                    if(freeAccesses.Any())
                    {
                        //Если есть неиспользованный аккаунт - берем его
                        var freeAccess = freeAccesses.FirstOrDefault();
                        result = freeAccess.Id;
                        return;
                    }
                    else
                    {

                    }
                }
                var grouped = bulletins.GroupBy(q => q.AccessId);
            });
            return result;
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получает свободный доступ к борде для пользователя из инстанции буллетина </summary>
        ///
        /// <remarks>   SV Milovanov, 12.02.2018. </remarks>
        ///
        /// <param name="userId">   Identifier for the user. </param>
        /// <param name="boardId">  Identifier for the board. </param>
        ///
        /// <returns>   The free access. </returns>
        ///-------------------------------------------------------------------------------------------------
        public static AccessPackage GetFreeAccess(Guid instanceId)
        {
            AccessPackage result = null;
            BCT.Execute(d =>
            {
                var dbInstance = d.BulletinDb.BulletinInstances.FirstOrDefault(q => q.Id == instanceId);

                var dbBoard = d.BulletinDb.Boards.FirstOrDefault(q => q.Id == dbInstance.BoardId);
                var boardId = dbBoard.Id;

                var dbBulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == dbInstance.BulletinId);
                var userId = dbBulletin.UserId;
                var accessId = dbInstance.AccessId;

                Access access;
                if (accessId == Guid.Empty)
                {
                    access = d.BulletinDb.Accesses.FirstOrDefault(q => q.UserId == userId && q.BoardId == boardId);
                    accessId = access.Id;
                }
                else
                {
                    access = d.BulletinDb.Accesses.FirstOrDefault(q => q.Id == accessId);
                }
                result = new AccessPackage
                {
                    //Id = access.Id,
                    Login = access.Login,
                    Password = access.Password,
                    BoardId = boardId
                };
            });
            return result;
        }
        /// <summary>
        /// Получает неиспользованные доступы для буллетина
        /// </summary>
        /// <param name="bulletinId"></param>
        /// <returns></returns>
        public static IEnumerable<AccessPackage> GetUnusedAccesses(Guid bulletinId)
        {
            var result = new List<AccessPackage>();

            BCT.Execute(d =>
            {
                var allAccesses = d.BulletinDb.Accesses.ToArray();
                var usedAccesses = d.BulletinDb.BulletinInstances.Where(q => q.BulletinId == bulletinId).Select(q => q.AccessId).ToArray();


                var unusedAccesses = allAccesses.Where(q => !usedAccesses.Contains(q.Id));
                result = unusedAccesses.Select(q => new AccessPackage
                {
                    Login = q.Login,
                    Password = q.Password,
                    BoardId = q.BoardId,
                    State = q.State,
                }).ToList();
            });
            return result;
        }
        public static IEnumerable<AccessPackage> MarkAccessAsChecked(IEnumerable<AccessPackage> packages)
        {
            var result = Enumerable.Empty<AccessPackage>();
            BCT.Execute(d =>
            {
                foreach (var p in packages)
                {
                    var dbAccess = d.BulletinDb.Accesses.FirstOrDefault(q => q.Login == p.Login && q.Password == p.Password);
                    if (dbAccess != null)
                    {
                        dbAccess.StateEnum = BulletinEngine.Entity.Data.AccessState.Activated;
                        d.BulletinDb.SaveChanges();
                    }
                }
            });
            return result;
        }
        public static IEnumerable<AccessPackage> AddAccesses(IEnumerable<AccessPackage> packages)
        {
            var result = Enumerable.Empty<AccessPackage>();
            BCT.Execute(d =>
            {
                foreach (var p in packages)
                {
                    var dbAccess = new Access
                    {
                        Login = p.Login,
                        Password = p.Password,
                        BoardId = d.BulletinDb.Boards.FirstOrDefault().Id,
                        UserId = d.MainDb.UserAccesses.FirstOrDefault().Id
                    };
                    dbAccess.StateEnum = BulletinEngine.Entity.Data.AccessState.Cloning;

                    var dbBulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == dbAccess.UserId).ToArray();
                    foreach(var b in dbBulletins)
                    {
                        b.StateEnum = Entity.Data.BulletinState.Cloning;
                    }
                }
                d.SaveChanges();
            });
            return result;
        }
    }
}