
using BulletinEngine.Entity.Context;
using FessooFramework.Components;
using FessooFramework.Tools.DataContexts;
using System;
using System.Configuration;
using System.Linq;

namespace BulletinEngine.Core
{
    public class BulletinContext : DCTContext
    {
        public Guid UserId
        {
            get
            {
                var result = Guid.Empty;
                BCT.Execute(d =>
                {
                    var users = d.MainDb.UserAccesses.ToArray();
                    var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.LoginHash == d._SessionInfo.HashUID);
                    if (user != null)
                        result = user.Id;

                });
                return result;
            }
        }
        public BulletinDb BulletinDb => _Store.Context<BulletinDb>();
        public MainDB MainDb => _Store.Context<MainDB>();
        public BulletinContext()
        {
            var settings = ConfigurationManager.AppSettings;
            if(ConfigurationManager.AppSettings["IsIntegrated"] == "True")
            {
                _Store.Add<BulletinDb>(ConfigurationManager.AppSettings["BulletinDbName"]);
                _Store.Add<MainDB>(ConfigurationManager.AppSettings["MainDbName"]);
            }
            else
            {
                var r = ConfigurationManager.AppSettings["BulletinDbName"];
                _Store.Add<BulletinDb>(ConfigurationManager.AppSettings["BulletinDbName"],
               ConfigurationManager.AppSettings["BulletinDbIp"],
               ConfigurationManager.AppSettings["BulletinDbLogin"],
               ConfigurationManager.AppSettings["BulletinDbPassword"]);

                _Store.Add<MainDB>(ConfigurationManager.AppSettings["MainDbName"],
               ConfigurationManager.AppSettings["MainDbIp"],
               ConfigurationManager.AppSettings["MainDbLogin"],
               ConfigurationManager.AppSettings["MainDbPassword"]);
            }
        }
    }
}