using BulletinBridge.Data;
using BulletinBridge.Data.Base;
using BulletinBridge.Messages;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Service
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Обрабатывает запросы WebWorker </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    static class InternalApiHelper
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

        public static ResponseInternalApi_GetWork GetWork(RequestInternalApi_GetWork request)
        {
            ResponseInternalApi_GetWork result = null;
            BCT.Execute((d) =>
            {
                var collection = new List<DataObjectBase>();

                
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
                result = new ResponseInternalApi_GetWork { Objects = collection };
            });
            return result;
        }
    }
}
