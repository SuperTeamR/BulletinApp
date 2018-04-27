using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Models
{
    public class TaskCache : CacheObject
    {
        public string Board { get; set; }
        public string Command { get; set; }
        public string Error { get; set; }
    }
}
