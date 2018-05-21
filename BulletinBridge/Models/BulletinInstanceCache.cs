using FessooFramework.Objects.Data;
using System;

namespace BulletinBridge.Data
{
    public class BulletinInstanceCache : CacheObject
    {
        public string Url { get; set; }
        public int Views { get; set; }
        public DateTime? ActivationDate { get; set; }

        public BulletinInstanceCache()
        {
        }
    }
}