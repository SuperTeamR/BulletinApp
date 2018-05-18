using BulletinBridge.Models;
using BulletinWebDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.ServiceHelper
{
    public static class MessageServiceHelper
    {
        public static void Save(MessageCache model)
        {
            DCT.Execute(c => c.HubClient.Save<MessageCache>((a) => { }, model));
        }
        public static void Save(IEnumerable<MessageCache> models)
        {
            DCT.Execute(c => c.HubClient.Save<MessageCache>((a) => { }, models));

        }
    }
}
