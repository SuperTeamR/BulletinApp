using BulletinBridge.Data.Base;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    public class TabState : DataObjectBase
    {
        public string Href { get; set; }
        public string Title { get; set; }
        public override TimeSpan SetTTL() => TimeSpan.MaxValue;
        public override Version SetVersion() => new Version(1, 0, 0, 0);
    }
}
