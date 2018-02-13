using BulletinBridge.Data.Base;
using FessooFramework.Objects.Data;
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
        public override TimeSpan SetTTL() => TimeSpan.MaxValue;
        public override Version SetVersion() => new Version(1, 0, 0, 0);
    }
}
