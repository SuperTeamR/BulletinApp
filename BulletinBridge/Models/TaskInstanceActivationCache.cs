using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskInstanceActivationCache : CacheObject
    {
        public Guid InstanceId { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
    }
}
