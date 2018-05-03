using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskBulletinTemplateCollectorCache : CacheObject
    {
        public Guid BulletinId { get; set; }
        public IEnumerable<string> Queries { get;set; }
    }
}
