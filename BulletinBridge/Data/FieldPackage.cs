using FessooFramework.Objects.Data;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class FieldPackage : CacheObject
    {
        [DataMember]
        public string HtmlId { get; set; }

        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public bool HasId { get; set; }

        [DataMember]
        public OptionTag[] Options { get; set; }

        [DataMember]
        public bool IsDynamic { get; set; }

        public FieldPackage()
        {

        }
        FieldPackage(string id, string tag, bool hasId, OptionTag[] options)
        {
            HtmlId = id;
            Tag = tag;
            HasId = hasId;
            Options = options;
        }
        public static FieldPackage Create(string id, string tag, bool hasId = true, OptionTag[] options = null)
        {
            return new FieldPackage(id, tag, hasId, options);
        }
    }
}
