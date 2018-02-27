using BulletinBridge.Data;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.Web.DataService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FessooFramework.Objects.Data;
using BulletinHub.Service;

namespace HubService.Configurations
{
    public class BulletinConfiguration : DataServiceConfiguration<BulletinInstance, BulletinPackage>
    {
        public override IEnumerable<CacheObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<CacheObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<CacheObject>();
            var objects = obj.OfType<BulletinPackage>();
            switch (code)
            {
                case "AssignBulletinWork":
                    result = AddWorkResultApiHelper.AddWorkResult(objects);
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}