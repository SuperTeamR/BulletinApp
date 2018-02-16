using FessooFramework.Objects.Data;
using System.Collections.Generic;

namespace BulletinBridge.Messages.BoardApi
{
    public class RequestBoardApi_AddBulletins
    {
        public IEnumerable<CacheObject> Objects { get; set; }
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
