using BulletinClient.Properties;
using FessooFramework.Tools.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Helpers
{
    public static class DataHelper
    {
        public static ObjectController<bool> IsAuth = new ObjectController<bool>(false);
        public static ObjectController<string> UserLogin = new ObjectController<string>(Settings.Default.UserLogin);
        public static ObjectController<string> UserPassword = new ObjectController<string>(Settings.Default.UserPassword);
    }
}
