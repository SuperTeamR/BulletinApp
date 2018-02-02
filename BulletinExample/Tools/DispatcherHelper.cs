using BulletinExample.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinExample.Tools
{
    public static class DispatcherHelper
    {
        #region Property
        /// <summary>
        ///     Диспетчер основного потока
        /// </summary>
        internal static SynchronizationContext CurrentSynchronizationContext { get; set; }
        #endregion
        #region Public methods
        /// <summary>
        ///     Устанавливает текущий диспетчер как основной диспетчер для выполнения операций
        /// </summary>
        /// <param name="dispatcher"></param>
        public static void SetDispatherAsDefault()
        {
            CurrentSynchronizationContext = SynchronizationContext.Current;

        }
        public static void Execute(Action action, bool isAsync = true)
        {
            DCT.Execute(data =>
            {
                if (action == null)
                    throw new NullReferenceException("CurrentDispatcher.Execute - Action не может быть пустым");
                if (isAsync)
                    CurrentSynchronizationContext.Post((a) => execute(a), action);
                else
                    CurrentSynchronizationContext.Send((a) => execute(a), action);
            });
        }

        #endregion
        #region Private
        private static void execute(object args)
        {
            DCT.Execute(data =>
            {
                if (args == null)
                    throw new NullReferenceException("CurrentDispatcher.Execute - Action не может быть пустым");
                var action = (Action)args;
                var sw = new Stopwatch();
                sw.Start();
                action?.Invoke();
                sw.Stop();
                //if (sw.ElapsedMilliseconds > 500)
                //   DCT.SendInfo("DispatcherHelper очень долго выполнялась операция, продолжительность " + sw.ElapsedMilliseconds + Environment.NewLine + "Описание операции: " + action.ToString());
            });
        }
        #endregion
    }
}
