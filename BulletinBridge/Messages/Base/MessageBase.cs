using BulletinBridge.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.Base
{
    [DataContract]
    public class MessageBase
    {
        [DataMember]
        public CommandApi Api { get; set; }

        [DataMember]
        public SerializationData Data { get; set; }

        public static MessageBase Create(SerializationData data, CommandApi api)
        {
            return new MessageBase
            {
                Data = data,
                Api = api
            };
        }
    }
}
