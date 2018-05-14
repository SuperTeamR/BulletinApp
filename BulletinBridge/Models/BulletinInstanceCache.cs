using FessooFramework.Objects.Data;

namespace BulletinBridge.Data
{
    public class BulletinInstanceCache : CacheObject
    {
        public string Url { get; set; }
        public int Views { get; set; }

        public BulletinInstanceCache()
        {
        }
    }
}