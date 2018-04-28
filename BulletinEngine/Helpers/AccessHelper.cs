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
            BCT.Execute(c =>
            {
                result = c.BulletinDb.Accesses.Where(q => q.UserId == c.UserId).ToArray();
            });
            return result;
        }
        internal static Access AddAvito(Access access)
        {
            BCT.Execute(c =>
            {
                var board = c.BulletinDb.Boards.FirstOrDefault(q => q.Name == "Avito");
                access.BoardId = board.Id;
                access.UserId = c.UserId;
                access.StateEnum = FessooFramework.Objects.Data.DefaultState.Enable;
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
        internal static void Enable(IEnumerable<Guid> ids)
        {
            BCT.Execute(c =>
            {
                foreach (var id in ids)
                {
                    var entity = c.BulletinDb.Accesses.Find(id);
                    if (entity != null)
                        entity.StateEnum = FessooFramework.Objects.Data.DefaultState.Enable;
                }
                c.SaveChanges();
            });
        }
        internal static void Disable(IEnumerable<Guid> ids)
        {
            BCT.Execute(c =>
            {
                foreach (var id in ids)
                {
                    var entity = c.BulletinDb.Accesses.Find(id);
                    if (entity != null)
                        entity.StateEnum = FessooFramework.Objects.Data.DefaultState.Disable;
                }
                c.SaveChanges();
            });
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
        public static AccessCache GetFreeAccess(Guid instanceId)
        {
            AccessCache result = null;
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
                result = new AccessCache
                {
                    //Id = access.Id,
                    Login = access.Login,
                    Password = access.Password,
                    BoardId = boardId
                };
            });
            return result;
        }

    }
}