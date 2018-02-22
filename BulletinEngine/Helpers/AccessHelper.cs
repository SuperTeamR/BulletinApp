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
    static class AccessHelper
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

    }
}
