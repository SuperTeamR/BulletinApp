using System;
using System.Collections.Generic;
using BulletinBridge.Data;

namespace BulletinWebWorker.Containers.Base
{
    internal abstract class BulletinPackageContainerBase
    {
        public abstract Guid Uid { get; }
        public abstract void AddBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);
        public abstract void EditBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);
    }
}
