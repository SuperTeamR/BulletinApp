using FessooFramework.Core;
using FessooFramework.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBrowserHostTest
{
    public class ExampleBootstrapper : Bootstrapper
    {
        public override string ApplicationName => "BulletinApp";

        public override void SetComponents(ref List<SystemComponent> components)
        {
         
        }

        public override void SetConfiguration(ref SystemCoreConfiguration configuration)
        {
        }
    }
}
