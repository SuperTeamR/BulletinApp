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
            execute: () => AskForBulletinWork(),
            check: () => true,
            checkTimeout: () => 60000);

        internal static TaskController ProfileWork = new TaskController(
           execute: () => AskForProfileWork(),
           check: () => true,
           checkTimeout: () => 60000);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Hub по буллетинам </summary>
        ///
        /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
        ///
        /// <param name="api">  The API. </param>
        ///-------------------------------------------------------------------------------------------------

        static void AskForBulletinWork()
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Hub по профилю </summary>
        ///
        /// <remarks>   SV Milovanov, 19.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        static void AskForProfileWork()
        {
            DCT.ExecuteAsync(d =>
            {
                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.Execute<RequestGetProfileWorkModel, ResponseGetProfileWorkModel>(new RequestGetProfileWorkModel());
                }
            });
        }
    }
}
