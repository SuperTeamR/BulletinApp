using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data.Base
{
    [DataContract]
    [KnownType(typeof(BulletinPackage))]
    public abstract class DataObjectBase
    {
        public Guid Id { get; set; }
    }
}
