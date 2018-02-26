using BulletinBridge.Data;
using BulletinBridge.Messages.BoardApi;
using BulletinBridge.Messages.InternalApi;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Service;
using BulletinEngine.Service.BoardApi;
using BulletinHub.Service;
using FessooFramework.Objects.Data;
using FessooFramework.Objects.Message;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Web;
using FessooFramework.Tools.Web.DataService.Configuration;
using HubService.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace HubService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, AutomaticSessionShutdown = true, UseSynchronizationContext = false)]
    [ServiceContract]
    public class DataService : FessooFramework.Tools.Web.DataService.DataServiceAPI
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
            return _Stat(this);
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
            BCT.Execute(c => {
                result = _Execute(data);
            });
            return result;
        }

        //protected override IEnumerable<ServiceRequestConfigBase> Configurations => new ServiceRequestConfigBase[]
        //{
        //    ServiceRequestConfig<RequestGetBulletinWorkModel, ResponseGetBulletinWorkModel>.New((a) => GetWorkApiHelper.GetWork(a)),
        //    ServiceRequestConfig<RequestGetProfileWorkModel, ResponseGetProfileWorkModel>.New((a) => GetWorkApiHelper.GetWork(a)),
        //    ServiceRequestConfig<RequestAddBulletinListWorkModel, ResponseAddBulletinListWorkModel>.New(a => AddWorkResultApiHelper.AddWorkResult(a)),
        //    ServiceRequestConfig<RequestAddBulletinsModel, ResponseAddBulletinsModel>.New((a) => ClientApiHelper.AddBulletins(a)),
        //    ServiceRequestConfig<RequestAddAccessModel, ResponseAddAccessModel>.New((a) => ClientApiHelper.AddAccess(a)),
        //};

        public override IEnumerable<DataServiceConfigurationBase> Convertors => new DataServiceConfigurationBase[]
        {
            new BulletinConfiguration(),
            new DataServiceConfiguration<Access, AccessPackage>(),
        };

        
    }
}
