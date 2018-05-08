using FessooFramework.Tools.Web.DataService;
using System;
using System.Configuration;

namespace BulletinWebDriver.Core
{
    internal class HubServiceClient : DataServiceClient
    {
#if DEBUG && !DEBUG_REMOTE
        public override string Address => "http://localhost:59888/DataService.svc";
#endif
#if RELEASE || DEBUG_REMOTE
        public override string Address => "http://176.111.73.51/BulletinHub/DataService.svc";
#endif
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        public override string HashUID => "Engine";
        public override string SessionUID => "Engine";
    }
}
