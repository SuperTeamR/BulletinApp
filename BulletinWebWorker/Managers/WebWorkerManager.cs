using BulletinBridge.Messages;
using BulletinWebWorker.Containers;
using BulletinWebWorker.Task;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.Controllers;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Managers
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет тасками WebWorker </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal static class WebWorkerManager
    {
        internal static TaskController BulletinWork = new TaskController(
            execute: () => AskForWork(BulletinBridge.Commands.CommandApi.Internal_GetBulletinWork),
            check: () => true,
            checkTimeout: () => 10000);

        internal static WorkContainer Container = new WorkContainer();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Запрашивает работу с Engine </summary>
        ///
        /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
        ///
        /// <param name="api">  The API. </param>
        ///-------------------------------------------------------------------------------------------------

        static void AskForWork(BulletinBridge.Commands.CommandApi api)
        {
            DCT.ExecuteAsync(d =>
            {
                 HubService.SendMessage<RequestInternalApi_GetWork, ResponseInternalApi_GetWork>(new RequestInternalApi_GetWork(), api);
                //container.AssignWork(response.Objects, api);
            });

        }
    }
}
