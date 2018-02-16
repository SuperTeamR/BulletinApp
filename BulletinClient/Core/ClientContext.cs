using BulletinClient.Tools;
using FessooFramework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Core
{
    class ClientContext : DCTContext
    {
        public IOC<object> ContainerViewModel => _ContainerViewModel;
        private static IOC<object> _ContainerViewModel = new IOC<object>();

        public ServiceClient ServiceClient => _Store.ServiceContext<ServiceClient>();

        public ClientContext()
        {
            _Store.Add<ServiceClient>();
        }
    }
}
