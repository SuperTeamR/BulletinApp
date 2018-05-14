using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    [DataContract]
    public class CallCache : CacheObject
    {
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public string VirtualNumber { get; set; }
        [DataMember]
        public string ForwardNumber { get; set; }
        [DataMember]
        public DateTime CallDate { get; set; }
    }
}
