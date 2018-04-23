using BulletinClient.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinClient.HelperService
{
    public static class HubServiceHelper
    {
        public static void SendQueryCollection<T>(Action<IEnumerable<T>> callback, string code, T obj = null)
            where T : CacheObject
        {
            DCT.Execute(c =>
            {
                c.HubClient.SendQueryCollection<T>(code, callback, obj == null ? null : new[] { obj });
            });
        }
    }
}