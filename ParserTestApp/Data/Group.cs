using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Category
{
    [DataContract]
    public class Group
    {
        [DataMember]
        public string Category1 { get; private set; }
        [DataMember]
        public string Category2 { get; private set; }
        [DataMember]
        public string Category3 { get; private set; }
        [DataMember]
        public string Category4 { get; private set; }
        [DataMember]
        public string Category5 { get; private set; }

        public Group(string category1, string category2, string category3 = null, string category4 = null, string category5 = null)
        {
            Category1 = category1;
            Category2 = category2;
            Category3 = category3;
            Category4 = category4;
            Category5 = category5;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var c = (Group)obj;
            return (Category1 == c.Category1)
                && (Category2 == c.Category2)
                && (Category3 == c.Category3)
                && (Category4 == c.Category4)
                && (Category5 == c.Category5);

        }
    }
}
