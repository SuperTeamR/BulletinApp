using BulletinBridge.Data.Base;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    [DataContract]
    public class AccessPackage : DataObjectBase
    {
        [DataMember]
        public Guid BoardId { get; set; }
        [DataMember]
        public string Login {get;set;}
        [DataMember]
        public string Password { get; set; }

        public override TimeSpan SetTTL() => TimeSpan.MaxValue;
        public override Version SetVersion() => new Version(1, 0, 0, 0);
    }
}
