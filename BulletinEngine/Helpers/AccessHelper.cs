using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Helpers
{
    public static class AccessHelper
    {
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
                var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Id == instanceId);

                var dbBoard = d.Db1.Boards.FirstOrDefault(q => q.Id == dbInstance.BoardId);
                var boardId = dbBoard.Id;

                var dbBulletin = d.Db1.Bulletins.FirstOrDefault(q => q.Id == dbInstance.BulletinId);
                var userId = dbBulletin.UserId;
                var accessId = dbInstance.AccessId;

                Access access;
                if (accessId == Guid.Empty)
                {
                    access = d.Db1.Accesses.FirstOrDefault(q => q.UserId == userId && q.BoardId == boardId);
                    accessId = access.Id;
                }
                else
                {
                    access = d.Db1.Accesses.FirstOrDefault(q => q.Id == accessId);
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
                var allAccesses = d.Db1.Accesses.ToArray();
                var usedAccesses = d.Db1.BulletinInstances.Where(q => q.BulletinId == bulletinId).Select(q => q.AccessId).ToArray();


                var unusedAccesses = allAccesses.Where(q => !usedAccesses.Contains(q.Id));
                result = unusedAccesses.Select(q => new AccessPackage
                {
                    Login = q.Login,
                    Password = q.Password,
                    BoardId = q.BoardId,
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
                    var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == p.Login && q.Password == p.Password);
                    if (dbAccess != null)
                    {
                        dbAccess.StateEnum = BulletinEngine.Entity.Data.AccessState.Activated;
                        d.Db1.SaveChanges();
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
                        BoardId = d.Db1.Boards.FirstOrDefault().Id,
                        UserId = d.Db1.Users.FirstOrDefault().Id
                    };
                    dbAccess.StateEnum = BulletinEngine.Entity.Data.AccessState.Created;

                    var dbBulletins = d.Db1.Bulletins.Where(q => q.UserId == dbAccess.UserId).ToArray();
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
