using FessooFramework.Objects.Data;

namespace BulletinBridge.Models
{
    public class BulletinSheetCache : CacheObject
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int Views { get; set; }
        public string Price { get; set; }
        public string[] Images { get; set; }
        public bool IsHandled { get; set; }
    }
}