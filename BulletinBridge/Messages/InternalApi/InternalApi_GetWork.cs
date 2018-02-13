using BulletinBridge.Data.Base;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages
{
    public class RequestInternalApi_GetWork
    {

    }


    public class ResponseInternalApi_GetWork
    {
        public IEnumerable<DataObjectBase> Objects { get; set; }
    }
}
