using BulletinBridge.Data;
using BulletinClient.Core;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace BulletinClient.HelperService
{
    static class BulletinHelper
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

        public static void Generate()
        {
            HubServiceHelper.SendQueryCollection<BulletinCache>((a) =>
            {
                MessageBox.Show("Generate started!");
            }, "Generate");
        }


        public static void All(Action<IEnumerable<BulletinCache>> callback)
        {
            HubServiceHelper.SendQueryCollection<BulletinCache>((a) =>
            {
                DataCollection = a;
                callback(DataCollection);
            }, "All");
        }

        public static void AddAvito(Action<IEnumerable<BulletinCache>> callback, BulletinCache cache)
        {
            HubServiceHelper.SendQueryCollection<BulletinCache>(callback, "AddAvito", cache);
        }

        public static void Edit(Action<IEnumerable<BulletinCache>> callback, BulletinCache model)
        {
            HubServiceHelper.SendQueryCollection(callback, "Edit", model);
        }

        internal static void Remove(Action callback, BulletinCache selectedObject)
        {
            DCT.Execute(data =>
            {
                data.HubClient.SendQueryObject<BulletinCache>( "Remove", (a) => callback?.Invoke(),selectedObject);
            });
        }

        /// <summary>
        /// Конвертирую строку в Hash строку шифрованную с помощью 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StringToSha256String(params string[] str)
        {
            var temp = string.Empty;
            foreach (var s in str)
            {
                temp += s ?? string.Empty;
            }
            return CryptographyHelper.StringToSha256String(temp);
        }
    }
}