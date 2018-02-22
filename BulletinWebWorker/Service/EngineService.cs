using BulletinBridge.Messages.InternalApi;
using BulletinWebWorker.Containers;
using BulletinWebWorker.Properties;
using FessooFramework.Tools.Web;
using FessooFramework.Tools.Web.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Service
{
    class EngineService : DataServiceClient
    {
        public override string Address => Settings.Default.DataServiceAddress;
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        //protected override IEnumerable<ServiceResponseConfigBase> Configurations => new ServiceResponseConfigBase[] {
        //    ServiceResponseConfig<ResponseGetBulletinWorkModel>.New(a =>
        //        {
        //            WorkRouter.AssignBulletinWork(a.Objects);
        //        }
        //    ),
        //    ServiceResponseConfig<ResponseGetProfileWorkModel>.New(a => { WorkRouter.AssignProfileWork(a.Objects); })
        //};
    }
}
