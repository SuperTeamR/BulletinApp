using System;
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

        public ConcurrentQueue<Guid> Bulletins { get; set; } = new ConcurrentQueue<Guid>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Очередь профилей, ожидающих обработку </summary>
        ///
        /// <value> The profiles. </value>
        ///-------------------------------------------------------------------------------------------------

        public ConcurrentQueue<Guid> Profiles { get; set; } = new ConcurrentQueue<Guid>();
    }
}
