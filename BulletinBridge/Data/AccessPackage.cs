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
}
