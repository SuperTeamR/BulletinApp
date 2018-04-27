using FessooFramework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.Core
{
    internal class BWContext : DCTContext
    {
        public HubServiceClient HubClient => _Store.ServiceContext<HubServiceClient>();
        public BWContext()
        {
            _Store.Add<HubServiceClient>();
        }
    }
}
