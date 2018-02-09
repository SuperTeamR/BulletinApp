using BulletinBridge.Commands;
using BulletinBridge.Messages;
using BulletinBridge.Messages.Base;
using BulletinWebWorker.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Service
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Временный роутер для обработки ответов с UDP </summary>
    ///
    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public static class ResponseRouter
    {
        public static MessageBase ExecuteRouting(MessageBase message)
        {
            switch (message.Api)
            {
                case BulletinBridge.Commands.CommandApi.Internal_GetBulletinWork:
                    var r = message.Data.Deserialize<ResponseInternalApi_GetWork>();
                    WebWorkerManager.Container.AssignWork(r.Objects, message.Api);
                    break;
                default:
                    break;
            }
            return message;
        }
    }
}
