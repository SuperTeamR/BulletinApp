using FessooFramework.Objects.Data;
using System.Collections.Generic;

namespace BulletinBridge.Data
{
    public class AggregateBulletinPackage : CacheObject
    {
        public IEnumerable<AccessPackage> Accesses { get; set; }
        public BulletinPackage Bulletin { get; set; }
    }
}