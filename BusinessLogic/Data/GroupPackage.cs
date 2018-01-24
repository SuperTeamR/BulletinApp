using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    [DataContract]
    internal class GroupPackage
    {
        [DataMember]
        public Group Group { get; set; }
        [DataMember]
        public Dictionary<string, FieldPackage> Fields { get; set; }

        public GroupPackage(Group group, Dictionary<string, FieldPackage> fields)
        {
            Group = group;
            Fields = fields;
        }
    }
}
