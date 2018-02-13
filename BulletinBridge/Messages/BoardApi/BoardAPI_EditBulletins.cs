using BulletinBridge.Data.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestBoardAPI_EditBulletins
    {
        public IEnumerable<DataObjectBase> Objects { get; set; }
    }

    public class ResponseBoardApi_EditBulletins
    {
        public ResponseBoardApi_EditBulletinsState State { get; set; }
    }

    public enum ResponseBoardApi_EditBulletinsState
    {
        None = 0,
        Success = 1,
        Error = 2,
    }
}
