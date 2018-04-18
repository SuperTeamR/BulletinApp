using BulletinBridge.Data;
using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.HelperService
{
    public static class AccessHelper
    {
        public static IEnumerable<AccessPackage> DataCollection
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
        private static IEnumerable<AccessPackage> dataCollection { get; set; }

        public static void All(Action<IEnumerable<AccessPackage>> callback)
        {
            HubServiceHelper.SendQueryCollection<AccessPackage>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }

        internal static void AddAvito(Action<IEnumerable<AccessPackage>> callback, AccessPackage cache)
        {
            HubServiceHelper.SendQueryCollection<AccessPackage>(callback, "AddAvito", cache);
        }

        public static void Save(Action callback, AccessPackage model)
        {
            DCT.Execute(data =>
            {
                data.HubClient.Save<AccessPackage>((a) => callback?.Invoke(), model);
            });
        }

        internal static void Remove(Action callback, AccessPackage selectedObject)
        {
            DCT.Execute(data =>
            {
                data.HubClient.SendQueryObject<AccessPackage>((a) => callback?.Invoke(), "Remove", selectedObject);
            });
        }

      
    }
}
