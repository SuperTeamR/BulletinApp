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
    public class MessageCache : CacheObject
    {
        [DataMember]
        public Guid AccessId { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public DateTime PublicationDate { get; set; }
        [DataMember]
        public string Url { get; set; }
    }
}
