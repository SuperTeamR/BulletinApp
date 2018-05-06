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
        public int Price { get; set; }
        /// <summary>
        /// Количество просмотров
        /// </summary>
        [DataMember]
        public int Count { get; set; }

        [DataMember]
        public string Images { get; set; }
        [DataMember]
        public string Category1 { get; set; }
        [DataMember]
        public string Category2 { get; set; }
        [DataMember]
        public string Category3 { get; set; }
        [DataMember]
        public string Category4 { get; set; }
        [DataMember]
        public string Category5 { get; set; }

        /// <summary>
        /// Область - Калининградская область, Москва, Московская область т.д.
        /// </summary>
        [DataMember]
        public string Region1 { get; set; }
        /// <summary>
        /// Город - Москва, Санкт-петербург, Подольск
        /// </summary>
        [DataMember]
        public string Region2 { get; set; }
        /// <summary>
        /// Район или метро 
        /// </summary>
        [DataMember]
        public string Region3 { get; set; }

        [DataMember]
        public bool IsIndividualSeller { get; set; }
        [DataMember]
        public bool IsHandled { get; set; }
    }
}
