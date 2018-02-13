using BulletinBridge.Data.Base;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestBoardApi_AddBulletins
    {
        public IEnumerable<DataObjectBase> Objects { get; set; }
    }

    public class ResponseBoardApi_AddBulletins
    {
        public ResponseBoardApi_AddBulletinsState State { get; set; }
    }

    public enum ResponseBoardApi_AddBulletinsState
    {
        None = 0,
        Success = 1,
        Error = 2,
    }

}
