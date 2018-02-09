using FessooFramework.Objects;
using FessooFramework.Objects.ALM;
using FessooFramework.Tools.Repozitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Containers.Base
{
    public abstract class BulletinContainerBase : DataContainer
    {
        public abstract Guid Uid { get; }

        public BulletinContainerBase(string text) : base(text)
        {
        }
        public override DataComponent Create(string uid)
        {
            return base.Create(uid);
        }
        public override void Default()
        {
            base.Default();
        }
        public override void Dispose()
        {
            base.Dispose();
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return base.ToString();
        }
        protected override void _3_Loaded()
        {
            base._3_Loaded();
        }
        protected override IEnumerable<TestingCase> _4_Testing()
        {
            return base._4_Testing();
        }
        protected override void _6_Unload()
        {
            base._6_Unload();
        }
        protected override void _StateChanged(SystemState newState, SystemState oldState)
        {
            base._StateChanged(newState, oldState);
        }
        protected override IEnumerable<ALMConf<SystemState>> _StateConfiguration()
        {
            return base._StateConfiguration();
        }
    }
}
