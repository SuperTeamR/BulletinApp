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
    public class BulletinPackage : DataObjectBase
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public string Views { get; set; }
        [DataMember]
        public GroupSignature Signature { get; set; }
        [DataMember]
        public AccessPackage Access { get; set; }
        [DataMember]
        public Dictionary<string, string> ValueFields { get; set; }
        [DataMember]
        public Dictionary<string, FieldPackage> AccessFields { get; set; }
        public override TimeSpan SetTTL() => TimeSpan.MaxValue;
        public override Version SetVersion() => new Version(1, 0, 0, 0);

    }

    public enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Rejected = 3,
        Blocked = 4,
        Publicated = 5,
        Edited = 6,
        Removed = 7,
        Error = 99,
    }
}
