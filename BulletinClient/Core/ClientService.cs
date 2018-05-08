using BulletinBridge.Data;
using BulletinClient.Core;
using BulletinClient.Properties;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinClient
{
    public class ServiceClient : FessooFramework.Tools.Web.DataService.DataServiceClient
    {
#if DEBUG && !DEBUG_REMOTE
        public override string Address => "http://localhost:59888/DataService.svc";
#endif
#if RELEASE || DEBUG_REMOTE
        public override string Address => "http://176.111.73.51/BulletinHub/DataService.svc";
#endif
        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);
        public override string HashUID => "Example";
        public override string SessionUID => "Example";

        #region Create
        public static void _CreateBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CreateBulletin(obj, action);
            });
        }
        public void CreateBulletins(IEnumerable<BulletinPackage> objs, Action<IEnumerable<BulletinPackage>> action = null)
        {
            DCT.Execute(d =>
            {
                var act = action == null ? (a) => { } : action;
                
                SendQueryCollection("Create", (a) => act(a), objects: objs);
            });
          
        }
        public void CreateBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.Execute(d =>
            {
                var act = action == null ? (a) => { } : action;

            SendQueryObject("Create", (a) => act(a), obj: obj);
                //CreateBulletins(new[] { obj }, (a) => act(a.FirstOrDefault()));
            });
        }
        #endregion

        #region Prepare
        public static void _PrepareInstance(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CreateBulletin(obj, action);
            });
        }
        public static void _PrepareInstances(IEnumerable<BulletinPackage> bulletins, Action<IEnumerable<BulletinPackage>> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CreateBulletins(bulletins, action);
            });
        }
        public void PrepareInstances(IEnumerable<BulletinPackage> objs, Action<IEnumerable<BulletinPackage>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection("PrepareInstances",(a) => act(a), objects: objs);
        }
        public void PrepareInstance(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CreateBulletins(new[] { obj }, (a) => act(a.FirstOrDefault()));
        }
        #endregion

        public void CloneBulletins(IEnumerable<BulletinPackage> objs, Action<IEnumerable<BulletinPackage>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection("CloneBulletins", (a) => act(a), objects: objs);
        }

        public void CloneBulletins(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CloneBulletins(new[] { obj }, (a) => act(a.FirstOrDefault()));
        }

        public void CreateAccess(AccessCache obj, Action<AccessCache> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CreateAccesses(new[] { obj }, (a) => act(a.FirstOrDefault()));
        }

        public void CreateAccesses(IEnumerable<AccessCache> objs, Action<IEnumerable<AccessCache>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection("CreateAccesses", (a) => act(a), objects: objs);

        }

        public static void _CloneBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CloneBulletins(obj, action);
            });
        }

        public static void _CreateAccess(AccessCache obj, Action<AccessCache> action = null)
        {
            DCT.ExecuteAsync(d =>
            {
                using (var client = new ServiceClient())
                    client.CreateAccess(obj, action);
            });
        }
    }
}