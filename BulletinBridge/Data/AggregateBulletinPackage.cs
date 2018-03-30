using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    public class AggregateBulletinPackage : CacheObject
    {
        public IEnumerable<AccessPackage> Accesses { get; set; }
        public BulletinPackage Bulletin { get; set; }
    }
}
