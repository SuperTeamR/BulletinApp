using BulletinBridge;
using BulletinBridge.Commands;
using BulletinBridge.Messages.Base;
using System.Diagnostics;

namespace BulletinWebWorker.Task
{
    public static class HubService
    {
        public static void SendMessage<TRequest, TResponse>(TRequest request, CommandApi api)
        {
            var message = MessageBase.Create(new SerializationData(request), api);
            switch (api)
            {
                case CommandApi.Internal_GetBulletinWork:
                    UdpManager.Send(message);
                    break;
            }
            //return message.Data.Deserialize<TResponse>();
        }
    }
}
