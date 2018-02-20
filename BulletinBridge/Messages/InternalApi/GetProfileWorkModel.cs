using BulletinBridge.Data;
using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.InternalApi
{
    public class RequestGetProfileWorkModel : RequestMessage<RequestGetProfileWorkModel, ResponseGetProfileWorkModel>
    {
    }
    public class ResponseGetProfileWorkModel : ResponseMessage<RequestGetProfileWorkModel, ResponseGetProfileWorkModel>
    {
        public IEnumerable<AccessPackage> Objects { get; set; }
    }
}
