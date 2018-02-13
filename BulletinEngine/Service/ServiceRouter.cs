using BulletinBridge;
using BulletinBridge.Messages;
using BulletinBridge.Messages.Base;
using BulletinBridge.Messages.BoardApi;
using BulletinEngine.Service.BoardApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Service
{
    static class ServiceRouter
    {
        public static MessageBase ExecuteRouting(MessageBase message)
        {
            switch (message.Api)
            {
                case BulletinBridge.Commands.CommandApi.Internal_GetBulletinWork:
                    Execute<RequestInternalApi_GetWork, ResponseInternalApi_GetWork>(message, InternalApiHelper.GetWork);
                    break;
                case BulletinBridge.Commands.CommandApi.Board_AddBulletins:
                    Execute<RequestBoardApi_AddBulletins, ResponseBoardApi_AddBulletins>(message, BoardApiHelper.AddBulletins);
                    break;
                case BulletinBridge.Commands.CommandApi.Board_EditBulletins:
                    Execute<RequestBoardAPI_EditBulletins, ResponseBoardApi_EditBulletins>(message, BoardApiHelper.EditBulletins);
                    break;
                default:
                    break;
            }
            UdpManager.Send(message);
            return message;
        }
        static MessageBase Execute<TRequest, TResponse>(MessageBase message, Func<TRequest, TResponse> action)
        {
            var request = message.Data.Deserialize<TRequest>();
            message.Data = null;
            var response = action(request);
            message.Data = new SerializationData(response);
            return message;
        }
    }
}
