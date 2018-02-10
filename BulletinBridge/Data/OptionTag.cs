using BulletinBridge.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    [DataContract]
    public class OptionTag : DataObjectBase
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

        //public static OptionTag Create(SelectOption entity)
        //{
        //    return new OptionTag(entity.Code, entity.Name);
        //}
    }
}
