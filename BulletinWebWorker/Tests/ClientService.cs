using BulletinBridge;
using BulletinBridge.Commands;
using BulletinBridge.Messages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Tests
{
    public static class ClientService
    {
        public static void SendMessage<TRequest, TResponse>(TRequest request, CommandApi api)
        {
            var message = MessageBase.Create(new SerializationData(request), api);
            switch (api)
            {
                case CommandApi.Board_AddBulletins:
                    UdpManager.Send(message);
                    break;
            }
            //return message.Data.Deserialize<TResponse>();
        }
    }
}
