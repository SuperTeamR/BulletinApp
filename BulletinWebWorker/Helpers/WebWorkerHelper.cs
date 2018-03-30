using BulletinBridge.Data;
using BulletinBridge.Messages.InternalApi;
using BulletinWebWorker.Containers;
using BulletinWebWorker.Service;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace BulletinWebWorker.Managers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет тасками WebWorker </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    static class WebWorkerManager
    {

        internal static void Execute()
        {
            DCT.ExecuteAsync(d =>
            {
                AskForAggregateBulletinWork();
                AskForBulletinWork();
                AskForProfileWork();
                
            }, 
            continueMethod:c => Application.Current.Shutdown());
        }


        //internal static TaskController BulletinWork = new TaskController(
        //    execute: () => AskForBulletinWork(),
        //    check: () => true,
        //    checkTimeout: () => 600000);

        //internal static TaskController ProfileWork = new TaskController(
        //   execute: () => AskForProfileWork(),
        //   check: () => true,
        //   checkTimeout: () => 600000);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Hub по буллетинам </summary>
        ///
        /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
        ///
        /// <param name="api">  The API. </param>
        ///-------------------------------------------------------------------------------------------------

        static void AskForBulletinWork()
        {
            DCT.Execute(d =>
            {
                d._SessionInfo.HashUID = "Engine";
                d._SessionInfo.SessionUID = "Engine";

                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.CollectionLoad<BulletinPackage>(AskForBulletinWork);
                }
            });

        }

        private static void AskForBulletinWork(IEnumerable<BulletinPackage> objs)
        {
            WorkRouter.AssignBulletinWork(objs);
        }

        /// <summary>
        /// Запрашивает работу с Hub по буллетинам (не инстанций)
        /// </summary>
        static void AskForAggregateBulletinWork()
        {
            DCT.Execute(d =>
            {
                d._SessionInfo.HashUID = "Engine";
                d._SessionInfo.SessionUID = "Engine";

                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.CollectionLoad<AggregateBulletinPackage>(AskForAggregateBulletinWorkCallback);
                }

            });
        }

        static void AskForAggregateBulletinWorkCallback(IEnumerable<AggregateBulletinPackage> objs)
        {
            WorkRouter.AssignAggregateBulletinWork(objs);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Hub по профилю </summary>
        ///
        /// <remarks>   SV Milovanov, 19.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        static void AskForProfileWork()
        {
            DCT.Execute(d =>
            {
                d._SessionInfo.HashUID = "Engine";
                d._SessionInfo.SessionUID = "Engine";

                using (var client = new EngineService())
                {
                    var result = client.Ping();
                    Console.WriteLine($"Ping = {result}");
                    client.CollectionLoad<AccessPackage>(WorkRouter.AssignProfileWork);
                }
            });
        }
    }
}
