using FessooFramework.Objects.Data;
using System.Collections.Generic;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestBoardAPI_EditBulletins
    {
        public IEnumerable<CacheObject> Objects { get; set; }
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
