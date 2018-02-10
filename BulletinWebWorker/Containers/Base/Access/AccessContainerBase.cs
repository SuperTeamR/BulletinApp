using BulletinBridge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Base.Access
{
    internal abstract class AccessContainerBase
    {
        public abstract Guid Uid { get; }

        public abstract bool TryAuth(AccessPackage access);
        protected abstract bool Auth();
        protected abstract void Exit();
    }
}
