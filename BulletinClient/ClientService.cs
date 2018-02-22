using BulletinClient.Properties;
using System;


namespace BulletinClient
{
    public class ServiceClient : FessooFramework.Tools.Web.DataService.DataServiceClient
    {
        public override string Address => Settings.Default.DataServiceAddress;

        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);

        public override string HashUID => Settings.Default.HashUID;
        public override string SessionUID => Settings.Default.SessionUID;
                
    }
}
