using BulletinEngine.Entity.Context;
using BulletinEngine.Tools;
using FessooFramework.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Core
{
    internal class BulletinContext : DCTContext
    {
        
        public BulletinDb Db1 => _Store.Context<BulletinDb>();

        public GlobalObjects Objects => Singleton<GlobalObjects>.Instance;
        public GlobalQueue Queue => Singleton<GlobalQueue>.Instance;


    }
}
