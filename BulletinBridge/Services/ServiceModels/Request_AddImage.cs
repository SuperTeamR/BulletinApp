using FessooFramework.Objects.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Services.ServiceModels
{
    public class Request_AddImage : RequestMessage<Request_AddImage, Response_AddImage>
    {
        public string Name { get; set; }
        public byte[] Image { get; set; }
    }

    public class Response_AddImage : ResponseMessage<Request_AddImage, Response_AddImage>
    {
        public string Url { get; set; }
        public string Error { get; set; }
    }
}
