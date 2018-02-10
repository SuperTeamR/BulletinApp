using BulletinBridge.Data.Base;
using BulletinBridge.Messages;
using BulletinEngine.Core;
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

    public static class InternalApiHelper
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
                    DataObjectBase r = null;
                    if (d.Queue.Bulletins.TryDequeue(out r))
                    {
                        collection.Add(r);
                    }
                }
                result = new ResponseInternalApi_GetWork { Objects = collection };
            });
            return result;
        }
    }
}
