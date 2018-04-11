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
        public override string Address => Settings.Default.DataServiceAddress;

        public override TimeSpan PostTimeout => TimeSpan.FromSeconds(100);

        public override string HashUID => Settings.Default.HashUID;
        public override string SessionUID => Settings.Default.SessionUID;



        public void CloneBulletins(IEnumerable<BulletinPackage> objs, Action<IEnumerable<BulletinPackage>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection((a) => act(a), "CloneBulletins", objects: objs);
        }
        public void CloneBulletins(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CloneBulletins(new[] { obj }, (a) => act(a.FirstOrDefault()));
        }
        public void CreateBulletins(IEnumerable<BulletinPackage> objs, Action<IEnumerable<BulletinPackage>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection((a) => act(a), "CreateBulletins", objects: objs);
        }
        public void CreateBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CreateBulletins(new [] { obj }, (a) => act(a.FirstOrDefault()));
        }

        public void CreateAccess(AccessPackage obj, Action<AccessPackage> action = null)
        {
            var act = action == null ? (a) => { } : action;
            CreateAccesses(new[] { obj }, (a) => act(a.FirstOrDefault()));
        }

        public void CreateAccesses(IEnumerable<AccessPackage> objs, Action<IEnumerable<AccessPackage>> action = null)
        {
            var act = action == null ? (a) => { } : action;
            SendQueryCollection((a) => act(a), "CreateAccesses", objects: objs);
        }

        public static void _CloneBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CloneBulletins(obj, action);
            });
        }
        public static void _CreateBulletin(BulletinPackage obj, Action<BulletinPackage> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CreateBulletin(obj, action);
            });
        }
        public static void _CreateBulletins(IEnumerable<BulletinPackage> bulletins, Action<IEnumerable<BulletinPackage>> action = null)
        {
            DCT.ExecuteAsync(d2 =>
            {
                using (var client = new ServiceClient())
                    client.CreateBulletins(bulletins, action);
            });
        }

        public static void _CreateAccess(AccessPackage obj, Action<AccessPackage> action = null)
        {
            DCT.ExecuteAsync(d =>
            {
                using (var client = new ServiceClient())
                    client.CreateAccess(obj, action);
            });
        }


    }
}
