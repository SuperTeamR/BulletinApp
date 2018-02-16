using BulletinBridge.Data;
using FessooFramework.Objects.Data;
using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.InternalApi
{
    public class RequestGetBulletinWorkModel : RequestMessage<RequestGetBulletinWorkModel, ResponseGetBulletinWorkModel>
    {
    }
    public class ResponseGetBulletinWorkModel : ResponseMessage<RequestGetBulletinWorkModel, ResponseGetBulletinWorkModel>
    {
        public IEnumerable<BulletinPackage> Objects { get; set; }
    }
}
