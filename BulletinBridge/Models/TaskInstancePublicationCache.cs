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
        public string Login { get; set; }
        public string Password { get; set; }
        public TaskInstancePublicationCache() : base(){ }
        public TaskInstancePublicationCache(Guid id, DateTime create) : base(id, create){ }
    }
}
