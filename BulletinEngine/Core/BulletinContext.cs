
using BulletinEngine.Entity.Context;
using BulletinEngine.Tools;
using FessooFramework.Components;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Core
{
    public class BulletinContext : DCTContext
    {
        public BulletinDb Db1 => _Store.Context<BulletinDb>();
        public GlobalObjects Objects => Singleton<GlobalObjects>.Instance;
        public GlobalQueue Queue => Singleton<GlobalQueue>.Instance;

        public BulletinContext()
        {
            var settings = ConfigurationManager.AppSettings;
            if(ConfigurationManager.AppSettings["IsIntegrated"] == "True")
            {
                _Store.Add<BulletinDb>(ConfigurationManager.AppSettings["BulletinDbName"]);
            }
            else
            {
                _Store.Add<BulletinDb>(ConfigurationManager.AppSettings["BulletinDbName"],
               ConfigurationManager.AppSettings["BulletinDbIp"],
               ConfigurationManager.AppSettings["BulletinDbLogin"],
               ConfigurationManager.AppSettings["BulletinDbPassword"]);
            }
           
        }
    }
}
