using FessooFramework.Components;

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