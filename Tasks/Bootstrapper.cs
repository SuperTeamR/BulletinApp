using FessooFramework.Core;
using FessooFramework.Objects;
using FessooFramework.Objects.SourceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tasks
{
    class Bootstrapper : Bootstrapper<Bootstrapper>
    {
        public override string ApplicationName => "Test Engine";

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
