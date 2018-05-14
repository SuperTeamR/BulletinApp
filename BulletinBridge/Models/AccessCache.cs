using FessooFramework.Objects.Data;
using System;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class AccessCache : CacheObject
    {

        [DataMember]
        public Guid BoardId { get; set; }
        [DataMember]
        public string BoardName { get; set; }
        [DataMember]
        public string Login {get;set;}
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string Phone { get; set; }
        [DataMember]
        public int PhoneTZID { get; set; }
        [DataMember]
        public bool HasBlocked { get; set; }
        [DataMember]
        public string StateDesc { get; set; }
        [DataMember]
        public int Views { get; set; }
        [DataMember]
        public int Calls { get; set; }
        [DataMember]
        public int Messages { get; set; }
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
        Cloning = 8,
        Error = 99,
    }
}