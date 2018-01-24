using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    [DataContract]
    internal class FieldPackage
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public bool HasId { get; set; }

        FieldPackage(string id, string tag, bool hasId)
        {
            Id = id;
            Tag = tag;
            HasId = hasId;
        }

        public static FieldPackage Create(string id, string tag, bool hasId = true)
        {
            return new FieldPackage(id, tag, hasId);
        }
    }
}
