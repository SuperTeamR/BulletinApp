using FessooFramework.Core;
using FessooFramework.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample
{
    public class BootstrapperExample : Bootstrapper
    {
        public override string ApplicationName => "BulletinExample";

        public override void SetComponents(ref List<SystemComponent> components)
        {
        }

        public override void SetConfiguration(ref SystemCoreConfiguration configuration)
        {
        }
    }
}
