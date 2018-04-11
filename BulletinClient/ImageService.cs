using BulletinBridge.Services;
using BulletinClient.Properties;
using FessooFramework.Tools.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient
{
    class ImageService : ImageServiceClient
    {
        public override string Address => Settings.Default.ImageServiceAddress;
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        public override string HashUID => Settings.Default.HashUID;
        public override string SessionUID => Settings.Default.SessionUID;
    }
}
