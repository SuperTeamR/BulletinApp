using BulletinBridge.Data;
using BulletinWebDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.ServiceHelper
{
    public static class AccessHelper
    {
        //public static AccessCache GetAccess(Guid id)
        //{
        //    return DCT.Execute(c => c.HubClient.RObjectLoad<AccessCache>(id));
        //}
        public static void Enable(Guid accessId)
        {
            DCT.Execute(c => c.HubClient.RSendQueryObject<AccessCache>("Enable", id: accessId));
        }
        public static void Disable(Guid accessId)
        {
            DCT.Execute(c => c.HubClient.RSendQueryObject<AccessCache>("Disable", id: accessId));
        }

        internal static AccessCache GetAccess(object accessId)
        {
            throw new NotImplementedException();
        }
    }
}
