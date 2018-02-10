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
    public class AccessPackage : DataObjectBase
    {
        [DataMember]
        public Guid BoardId { get; set; }
        [DataMember]
        public string Login {get;set;}
        [DataMember]
        public string Password { get; set; }


    }
}
