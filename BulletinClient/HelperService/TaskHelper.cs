using System;
using System.Collections.Generic;
using System.Linq;
using BulletinBridge.Models;

namespace BulletinClient.HelperService
{
    public class TaskHelper
    {
        public static IEnumerable<TaskCache> DataCollection
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
        private static IEnumerable<TaskCache> dataCollection { get; set; }


        public static void All(Action<IEnumerable<TaskCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<TaskCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }
    }
}