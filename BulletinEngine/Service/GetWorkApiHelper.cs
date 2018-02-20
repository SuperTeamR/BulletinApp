using BulletinBridge.Data;
using BulletinBridge.Messages;
using BulletinBridge.Messages.InternalApi;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Service
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Обрабатывает запросы WebWorker </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public static class GetWorkApiHelper
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получение буллетинов, ожидающих обработки </summary>
        ///
        /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
        ///
        /// <param name="request">  The request. </param>
        ///
        /// <returns>   The work. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static ResponseGetBulletinWorkModel GetWork(RequestGetBulletinWorkModel request)
        {
            ResponseGetBulletinWorkModel result = null;
            BCT.Execute((d) =>
            {
                var collection = new List<BulletinPackage>();
                while (d.Queue.Bulletins.Count > 0)
                {
                    var guid = Guid.Empty;
                    if (d.Queue.Bulletins.TryDequeue(out guid))
                    {
                        var dbInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Id == guid);
                        var package = dbInstance._ConvertToServiceModel<BulletinPackage>();
                        collection.Add(package);
                    }
                }
                result = new ResponseGetBulletinWorkModel { Objects = collection };
            });
            return result;
        }

        public static ResponseGetProfileWorkModel GetWork(RequestGetProfileWorkModel request)
        {
            ResponseGetProfileWorkModel result = null;
            BCT.Execute((d) =>
            {
                var collection = new List<AccessPackage>();
                while (d.Queue.Profiles.Count > 0)
                {
                    var guid = Guid.Empty;
                    if (d.Queue.Profiles.TryDequeue(out guid))
                    {
                        var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Id == guid);
                        var package = dbAccess._ConvertToServiceModel<AccessPackage>();
                        collection.Add(package);
                    }
                }
                result = new ResponseGetProfileWorkModel { Objects = collection };
            });
            return result;
        }


    }
}
