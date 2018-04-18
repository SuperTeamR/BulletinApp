using BulletinBridge.Data;
using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.HelperService
{
    public static class BulletinHelper
    {
        public static IEnumerable<BulletinCache> DataCollection
        {
            get
            {
                if (dataCollection.Any())
                    return dataCollection;
                All((a) => { });
                return dataCollection;
            }
            set => dataCollection = value;
        }
        private static IEnumerable<BulletinCache> dataCollection { get; set; }

        public static void All(Action<IEnumerable<BulletinCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<BulletinCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }

        internal static void AddAvito(Action<IEnumerable<BulletinCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<BulletinCache>(callback, "AddAvito");
        }

        public static void Save(Action callback, BulletinCache model)
        {
            DCT.Execute(data =>
            {
                data.HubClient.Save<BulletinCache>((a) => callback?.Invoke(), model);
            });
        }

        internal static void Remove(Action callback, BulletinCache selectedObject)
        {
            DCT.Execute(data =>
            {
                data.HubClient.SendQueryObject<BulletinCache>((a) => callback?.Invoke(), "Remove", selectedObject);
            });
        }
    }
}
