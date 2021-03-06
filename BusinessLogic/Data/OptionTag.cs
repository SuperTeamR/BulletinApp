﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
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
    }
}
