using BulletinBridge.Data;
using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.HelperService
{
    class SettingsHelper
    {
        public static void Save()
        {
            DCT.Execute(data =>
            {
                data.HubClient.Save((a) => { }, new UserSettingsCache());
            });
        }
    }
}
