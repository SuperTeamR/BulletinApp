using BulletinBridge.Data;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.Web.DataService.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using FessooFramework.Objects.Data;
using BulletinHub.Helpers;

namespace HubService.Configurations
{
    class BulletinConfiguration : DataServiceConfiguration<BulletinInstance, BulletinPackage>
    {
        //public override IEnumerable<CacheObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<CacheObject> obj = null, IEnumerable<Guid> id = null)
        //{
        //    var result = Enumerable.Empty<CacheObject>();
        //    var objects = obj.OfType<BulletinPackage>();
        //    switch (code)
        //    {
        //        case "AssignBulletinWork":
        //            result = BulletinHelper.AddWorkResult(objects);
        //            break;
        //        case "CreateBulletins":
        //            result = BulletinHelper.CreateBulletins(objects);
        //            break;
        //        case "CloneBulletins":
        //            result = BulletinHelper.CloneBulletins(objects);
        //            break;
        //        default:
        //            break;
        //    }
        //    return result;
        //}
    }
}