using BulletinExample.Entity.Context;
using BulletinExample.Entity.Data;
using BulletinExample.Tools;
using FessooFramework.Components;
using FessooFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Core
{
    public class BulletinContext : DCTContext
    {
        public BulletinContext()
        {
        }
        public BulletinDb Db1 => _Store.Context<BulletinDb>();
        public GlobalObjects Objects => Singleton<GlobalObjects>.Instance;
    }
}
