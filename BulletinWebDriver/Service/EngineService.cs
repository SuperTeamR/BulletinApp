using FessooFramework.Tools.Web.DataService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Service
{
    class EngineService : DataServiceClient
    {
        public override string Address => ConfigurationManager.AppSettings["DataServiceAddress"];
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        public override string HashUID => "Engine";
        public override string SessionUID => "Engine";
    }
}
