using BulletinBridge.Messages.InternalApi;
using BulletinWebWorker.Service;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;

namespace BulletinWebWorker.Managers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет тасками WebWorker </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    static class WebWorkerManager
    {
        internal static TaskController BulletinWork = new TaskController(
            execute: () => AskForWork(),
            check: () => true,
            checkTimeout: () => 60000);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Engine </summary>
        ///
        /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
        ///
        /// <param name="api">  The API. </param>
        ///-------------------------------------------------------------------------------------------------

        static void AskForWork()
        {
            DCT.ExecuteAsync(d =>
            {
                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.Execute<RequestGetBulletinWorkModel, ResponseGetBulletinWorkModel>(new RequestGetBulletinWorkModel());
                }
            });

        }
    }
}
