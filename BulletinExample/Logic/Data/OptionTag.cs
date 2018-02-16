using BulletinExample.Entity.Data;
using System.Runtime.Serialization;

namespace BulletinExample.Logic.Data
{
    [DataContract]
    internal class OptionTag
    {
        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public string Text { get; set; }


        OptionTag(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public static OptionTag Create(string value, string text)
        {
            return new OptionTag(value, text);
        }

        public static OptionTag Create(SelectOption entity)
        {
            return new OptionTag(entity.Code, entity.Name);
        }
    }
}
