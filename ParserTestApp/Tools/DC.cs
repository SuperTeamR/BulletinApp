using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Tools
{
    /// <summary>
    /// Основной контекст приложения, используемый в методах - аналог модели используемой в DCT
    /// </summary>
    public abstract class DC : IDisposable
    {
        #region Property
        public System.Enum Group { get; internal set; }
        public Guid TrackNumber { get; internal set; }
        //public static IOC<object> InstanceContainer = new IOC<object>();
        #endregion
        #region Constructor

        #endregion
        #region IDisposable
        public virtual void Dispose()
        {
        }
        #endregion

    }
}
