using BulletinBridge.Data;
using BulletinWebDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.ServiceHelper
{
    public static class BulletinInstanceHelper
    {
        public static BulletinInstanceCache Get(Guid id)
        {
            return DCT.Execute(c => c.HubClient.RObjectLoad<BulletinInstanceCache>(id));
        }
        public static void Save(BulletinInstanceCache model)
        {
            DCT.Execute(c => c.HubClient.Save<BulletinInstanceCache>((a)=> { },model));
        }
    }
}
