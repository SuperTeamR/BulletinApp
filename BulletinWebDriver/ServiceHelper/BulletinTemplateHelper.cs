using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinWebDriver.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebDriver.ServiceHelper
{
    public static class BulletinTemplateHelper
    {
        public static void Save(BulletinTemplateCache model)
        {
            DCT.Execute(c => c.HubClient.Save<BulletinTemplateCache>((a) => { }, model));
        }
        public static void Save(IEnumerable<BulletinTemplateCache> models)
        {
            DCT.Execute(c => c.HubClient.Save<BulletinTemplateCache>((a) => { }, models));
        }
    }
}
