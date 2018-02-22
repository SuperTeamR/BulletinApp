using BulletinBridge.Data;
using BulletinBridge.Messages;
using BulletinBridge.Messages.InternalApi;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
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
                var instances = d.Db1.BulletinInstances.Where(q => 
                    q.State == (int)BulletinInstanceState.Unchecked
                    || q.State == (int)BulletinInstanceState.WaitPublication
                    || q.State == (int)BulletinInstanceState.Edited
                    || q.State == (int)BulletinInstanceState.OnModeration)
                    .Take(50).ToArray();

                result = new ResponseGetBulletinWorkModel
                {
                    Objects =
                    instances.Select(q => q._ConvertToServiceModel<BulletinPackage>()).ToArray(),
                };

                foreach (var i in instances)
                {
                    i.StateEnum = BulletinInstanceState.Checking;
                }
                d.Db1.SaveChanges();
            });
            return result;
        }

        public static ResponseGetProfileWorkModel GetWork(RequestGetProfileWorkModel request)
        {
            ResponseGetProfileWorkModel result = null;
            BCT.Execute((d) =>
            {
                var accesses = d.Db1.Accesses.Where(q => q.State == (int)AccessState.Unchecked).Take(50).ToArray();

                result = new ResponseGetProfileWorkModel
                {
                    Objects =
                    accesses.Select(q => q._ConvertToServiceModel<AccessPackage>())
                };


                foreach (var i in accesses)
                {
                    i.StateEnum = AccessState.Checking;
                }
                d.Db1.SaveChanges();


            });
            return result;
        }


    }
}
