using BulletinClient.Properties;
using FessooFramework.Tools.Web.MainService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient
{
    public class MainService : MainServiceClient
    {
        public override string Address => Settings.Default.MainServiceAddress;
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        public override string HashUID => Settings.Default.HashUID;
        public override string SessionUID => Settings.Default.SessionUID;
    }
}
