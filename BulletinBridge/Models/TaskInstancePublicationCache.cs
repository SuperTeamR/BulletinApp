using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskInstancePublicationCache : CacheObject
    {
        ///Access
        public string Login { get; set; }
        public string Password { get; set; }

        ///Category
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }
        public string Category5 { get; set; }

        ///Fields
        public string Title { get;set;}
        public string Description { get;set; }
        public string Price { get;set; }
        public IEnumerable<string> Images { get;set; }
    }
}
