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
    public class BulletinTemplateCache : CacheObject
    {
        [DataMember]
        public string URL { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Price { get; set; }
        /// <summary>
        /// Количество просмотров
        /// </summary>
        [DataMember]
        public string Count { get; set; }

        [DataMember]
        public string Images { get; set; }
        [DataMember]
        public string GroupSignature { get; set; }
        [DataMember]
        public string Region { get; set; }

    }
}
