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

    }
}
