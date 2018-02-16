using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FessooFramework.Core;
using FessooFramework.Objects;
using FessooFramework.Objects.SourceData;

namespace BulletinWebWorker
{
    class Bootstrapper : Bootstrapper<Bootstrapper>
    {
        public override string ApplicationName => "WebWorker Engine";

        public override void SetComponents(ref List<SystemComponent> components)
        {
        }

        public override void SetConfiguration(ref SystemCoreConfiguration configuration)
        {
        }

        public override void SetDbContext(ref DataContextStore store)
        {
        }
    }
}
