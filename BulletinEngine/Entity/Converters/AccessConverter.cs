using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Entity.Converters
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Конвертер доступа к борде</summary>
    ///
    /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    class AccessConverter
    {
        public static AccessPackage ConvertToCache(Access obj)
        {
            AccessPackage result = null;
            BCT.Execute(d =>
            {
                result = new AccessPackage
                {
                    BoardId = obj.BoardId,
                    Login = obj.Login,
                    Password = obj.Password
                };
            });
            return result;
        }
        public static Access ConvertToModel(AccessPackage obj, Access entity)
        {
            var result = new Access
            {
                BoardId = obj.BoardId,
                Login = obj.Login,
                Password = obj.Password
            };
            return result;
        }
    }
}
