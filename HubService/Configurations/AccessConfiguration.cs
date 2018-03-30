using BulletinBridge.Data;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Web.DataService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HubService.Configurations
{
    class AccessConfiguration : DataServiceConfiguration<Access, AccessPackage>
    {
        public override IEnumerable<CacheObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<CacheObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<CacheObject>();
            var objects = obj.OfType<AccessPackage>();
            switch (code)
            {
                case "CheckAccess":
                    result = AccessHelper.MarkAccessAsChecked(objects);
                    break;
                case "CreateAccesses":
                    result = AccessHelper.AddAccesses(objects);
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}