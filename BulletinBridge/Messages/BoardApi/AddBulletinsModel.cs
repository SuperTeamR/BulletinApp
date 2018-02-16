using BulletinBridge.Data;
using FessooFramework.Objects.Data;
using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestAddBulletinsModel : RequestMessage<RequestAddBulletinsModel, ResponseAddBulletinsModel>
    {
        public IEnumerable<BulletinPackage> Objects { get; set; }
    }
    public class ResponseAddBulletinsModel : ResponseMessage<RequestAddBulletinsModel, ResponseAddBulletinsModel>
    {
      
    }
}
