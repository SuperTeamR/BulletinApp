using BulletinBridge.Data;
using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.InternalApi
{
    public class RequestAddBulletinListWorkModel : RequestMessage<RequestAddBulletinListWorkModel, ResponseAddBulletinListWorkModel>
    {
        public IEnumerable<BulletinPackage> Objects { get; set; }
    }

    public class ResponseAddBulletinListWorkModel : ResponseMessage<RequestAddBulletinListWorkModel, ResponseAddBulletinListWorkModel>
    {

    }
}
