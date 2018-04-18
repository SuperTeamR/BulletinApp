using BulletinClient.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.HelperService
{
    public static class HubServiceHelper
    {
        public static void SendQueryCollection<T>(Action<IEnumerable<T>> callback, string code)
            where T : CacheObject
        {
            DCT.Execute(c =>
            {
                c.HubClient.SendQueryCollection<T>(callback, code, c._SessionInfo.SessionUID, c._SessionInfo.HashUID);
            });
        }
    }
}
