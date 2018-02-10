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
    public class BulletinPackage : DataObjectBase
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public int State { get; set; }
        [DataMember]
        public string Views { get; set; }
        [DataMember]
        public GroupSignature Signature { get; set; }
        [DataMember]
        public AccessPackage Access { get; set; }
        [DataMember]
        public Dictionary<string, string> ValueFields { get; set; }
        [DataMember]
        public Dictionary<string, FieldPackage> AccessFields { get; set; }

    }

    public enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Publication = 3,
        Closed = 4,
        Error = 99,
    }
}
