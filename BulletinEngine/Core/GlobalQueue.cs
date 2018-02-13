using System;
using System.Collections.Concurrent;

namespace BulletinEngine.Core
{
    class GlobalQueue
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Очередь буллетинов, ожидающих обработку </summary>
        ///
        /// <value> The bulletins. </value>
        ///-------------------------------------------------------------------------------------------------

        public ConcurrentQueue<Guid> Bulletins { get; set; } = new ConcurrentQueue<Guid>();
    }
}
