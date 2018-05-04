using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinClient.Core;
using FessooFramework.Tools.Helpers;

namespace BulletinClient.HelperService
{
    static class TemplateHelper
    {
        public static IEnumerable<BulletinTemplateCache> DataCollection
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
        private static IEnumerable<BulletinTemplateCache> dataCollection { get; set; }


        public static void All(Action<IEnumerable<BulletinTemplateCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<BulletinTemplateCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }

        public static void MarkAsUsed(Action callback, BulletinTemplateCache template)
        {
            DCT.Execute(data =>
            {
                data.HubClient.SendQueryObject("MarkAsUsed", a => callback?.Invoke(), template);
            });

        }
    }
}