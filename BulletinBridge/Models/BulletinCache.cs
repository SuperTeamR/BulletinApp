using FessooFramework.Objects.Data;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class BulletinCache : CacheObject
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Images { get; set; }
        [DataMember]
        public string Price { get; set; }
        [DataMember]
        public string GroupSignature { get; set; }
        [DataMember]
        public string CurrentGroup { get; set; }

    }
}