using BulletinBridge.Data;
using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestAddAccessModel : RequestMessage<RequestAddAccessModel, ResponseAddAccessModel>
    {
        public IEnumerable<AccessPackage> Objects { get; set; }
    }

    public class ResponseAddAccessModel : ResponseMessage<RequestAddAccessModel, ResponseAddAccessModel>
    {
        public IEnumerable<AccessPackage> Objects { get; set; }
        public ResponseState State { get; set; }
    }

    public enum ResponseState
    {
        None = 0,
        Success = 1,
        Error = 2
    }
}
