using System;
using System.Collections.Generic;
using System.Linq;
using BulletinBridge.Data;
using BulletinBridge.Models;

namespace BulletinClient.HelperService
{
    public class InstanceHelper
    {
        public static IEnumerable<BulletinInstanceCache> DataCollection
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
        private static IEnumerable<BulletinInstanceCache> dataCollection { get; set; }


        public static void All(Action<IEnumerable<BulletinInstanceCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<BulletinInstanceCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }
    }
}