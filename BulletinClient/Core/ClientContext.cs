using FessooFramework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Core
{
    internal class ClientContext : DCTContext
    {
        public ServiceClient HubClient => _Store.ServiceContext<ServiceClient>();

        public ClientContext()
        {
            _Store.Add<ServiceClient>();
        }
    }
}
