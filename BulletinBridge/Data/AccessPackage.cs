using FessooFramework.Objects.Data;
using System;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class AccessPackage : CacheObject
    {
        [DataMember]
        public Guid BoardId { get; set; }
        [DataMember]
        public string Login {get;set;}
        [DataMember]
        public string Password { get; set; }
    }

    public enum AccessState
    {
        Created = 0,
        Activated = 1,
        Blocked = 2,
        Banned = 3,
        DemandPay = 4,
        Closed = 5,
        Unchecked = 6,
        Checking = 7,
        Error = 99,
    }
}
