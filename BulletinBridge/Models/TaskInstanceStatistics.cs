using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskInstanceStatisticsCache : CacheObject
    {
        public Guid InstanceId { get; set; }
        public string Url { get; set; }
    }
}
