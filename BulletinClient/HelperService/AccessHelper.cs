using BulletinBridge.Data;
using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinClient.HelperService
{
    public static class AccessHelper
    {
        public static IEnumerable<AccessCache> DataCollection
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
        private static IEnumerable<AccessCache> dataCollection { get; set; }
        public static void All(Action<IEnumerable<AccessCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<AccessCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }
        internal static void AddAvito(Action<IEnumerable<AccessCache>> callback, AccessCache cache)
        {
            HubServiceHelper.SendQueryCollection<AccessCache>(callback, "AddAvito", cache);
        }

        internal static void ActivateAccess(Action<IEnumerable<AccessCache>> callback, AccessCache cache)
        {
            HubServiceHelper.SendQueryCollection<AccessCache>(callback, "ActivateAccess", cache);
        }
        public static void Save(Action callback, AccessCache model)
        {
            DCT.Execute(data =>
            {
                data.HubClient.Save<AccessCache>((a) => callback?.Invoke(), model);
            });
        }
        internal static void Remove(Action callback, AccessCache selectedObject)
        {
            DCT.Execute(data =>
            {
                data.HubClient.SendQueryObject("Remove", (a) => callback?.Invoke(), selectedObject);
            });
        }
    }
}