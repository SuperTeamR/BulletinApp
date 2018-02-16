using FessooFramework.Objects.Data;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class OptionTag : CacheObject
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Text { get; set; }


        public OptionTag() { }
        OptionTag(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public static OptionTag Create(string value, string text)
        {
            return new OptionTag(value, text);
        }
    }
}
