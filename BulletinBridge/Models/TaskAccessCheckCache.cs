using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskAccessCheckCache : CacheObject
    {
        public Guid AccessId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public TaskAccessCheckCache() : base(){ }
        public TaskAccessCheckCache(Guid id, DateTime create) : base(id, create){ }
    }
}
