using BulletinBridge.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestBoardAPI_GetXlsForGroup
    {
        public GroupSignature GroupSignature { get; set; }
    }

    public class ResponseBoardAPI_GetXlsForGroup
    {
        public byte[] Xls { get; set; }

        public ResponseBoardAPI_GetXlsForGroupState State { get; set; }
    }

    public enum ResponseBoardAPI_GetXlsForGroupState
    {
        None = 0,
        Success = 1,
        Error = 2,
    }
}
