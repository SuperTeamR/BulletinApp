using BulletinBridge.Data.Base;
using System.Collections.Concurrent;

namespace BulletinEngine.Core
{
    public class GlobalQueue
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Очередь буллетинов, ожидающих обработку </summary>
        ///
        /// <value> The bulletins. </value>
        ///-------------------------------------------------------------------------------------------------

        public ConcurrentQueue<DataObjectBase> Bulletins { get; set; } = new ConcurrentQueue<DataObjectBase>();
    }
}
