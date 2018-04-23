using BulletinBridge.Services;
using FessooFramework.Objects.Message;
using FessooFramework.Tools.DCT;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace HubService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AutomaticSessionShutdown = true, UseSynchronizationContext = false)]
    [ServiceContract]
    public class ImageService : ImageServiceAPI
    {
        [WebInvoke(
           Method = "POST",
           UriTemplate = "Ping",
           RequestFormat = WebMessageFormat.Xml,
           ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public override bool Ping(string p)
        {
            return _Ping(p);
        }
        [WebInvoke(
        Method = "GET",
        UriTemplate = "Stat",
        RequestFormat = WebMessageFormat.Json,
        ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        public override string Stat()
        {
            return "ImageService";
            //return _Stat(this);
        }
        [WebInvoke(
         Method = "POST",
         UriTemplate = "Execute",
         RequestFormat = WebMessageFormat.Xml,
         ResponseFormat = WebMessageFormat.Xml)]
        [OperationContract]
        public override ServiceMessage Execute(ServiceMessage data)
        {
            var result = default(ServiceMessage);
            DCT.Execute(c => {
                result = _Execute(data);
            });
            return result;
        }
    }
}