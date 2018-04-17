using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    public class BulletinInstanceCache : CacheObject
    {
        public int Url { get; set; }

        public BulletinInstanceCache()
        {
        }
    }
}
