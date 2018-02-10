using BulletinExample.Tools;
using FessooFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Core
{
    public class DCT : _DCT<BulletinContext>
    {
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcherAsync(Action<BulletinContext> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null, string name = "")
        {
            DispatcherHelper.Execute(() => Execute(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod));
        }

        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcher(Action<BulletinContext> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null, string name = "")
        {
            DispatcherHelper.Execute(() => Execute(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod), false);
        }
    }
}
