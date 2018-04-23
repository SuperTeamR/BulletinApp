using FessooFramework.Core;
using FessooFramework.Objects;
using FessooFramework.Objects.SourceData;
using System.Collections.Generic;

namespace BulletinWebDriver
{
    class Bootstrapper : Bootstrapper<Bootstrapper>
    {
        public override string ApplicationName => "WebDriver Engine";
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