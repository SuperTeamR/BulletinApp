using BulletinExample.Tools;
using FessooFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Core
{
    public static class DCT
    {
        public static BulletinContext Context => _DCT.Context<BulletinContext>();
        /// <summary>   Executes void. </summary>
        ///
        /// <remarks>   Fess59, 26.01.2018. </remarks>
        ///
        /// <param name="action">                   The action. </param>
        /// <param name="continueExceptionMethod">  (Optional) The continue exception method. Выполнится
        ///                                         при ошибке в method. </param>
        /// <param name="continueMethod">           (Optional) The continue method. Выполнится после
        ///                                         method и continueExceptionMethod. </param>
        public static void Execute(Action<BulletinContext> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null, string name = "")
        {
            _DCT.Execute(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, name: name);
        }
        /// <summary>   Executes result. </summary>
        ///
        /// <remarks>   Fess59, 26.01.2018. </remarks>
        ///
        /// <typeparam name="TResult">  Type of the result. </typeparam>
        /// <param name="action">                   The action. </param>
        /// <param name="continueExceptionMethod">  (Optional) The continue exception method. Выполнится
        ///                                         при ошибке в method. </param>
        /// <param name="continueMethod">           (Optional) The continue method. Выполнится после
        ///                                         method и continueExceptionMethod. </param>
        ///
        /// <returns>   A TResult. </returns>
        public static TResult Execute<TResult>(Func<BulletinContext, TResult> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null)
        {
            return _DCT.Execute(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }
        /// <summary>   Executes the asynchronous operation. With result </summary>
        ///
        /// <remarks>   Fess59, 26.01.2018. </remarks>
        ///
        /// <typeparam name="TResult">  Type of the result. </typeparam>
        /// <param name="action">                   The action. </param>
        /// <param name="complete">                 The complete. Для отправки результата в другой блок
        ///                                         кода, используется в ExecuteAsync<TReusult> </param>
        /// <param name="continueExceptionMethod">  (Optional) The continue exception method. Выполнится
        ///                                         при ошибке в method. </param>
        /// <param name="continueMethod">           (Optional) The continue method. Выполнится после
        ///                                         method и continueExceptionMethod. </param>
        public static void ExecuteAsync<TResult>(Func<BulletinContext, TResult> action, Action<BulletinContext, TResult> complete, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null)
        {
            _DCT.ExecuteAsync(action, complete, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }
        /// <summary>   Executes the asynchronous operation. Void </summary>
        ///
        /// <remarks>   Fess59, 26.01.2018. </remarks>
        ///
        /// <param name="action">                   The action. </param>
        /// <param name="continueExceptionMethod">  (Optional) The continue exception method. Выполнится
        ///                                         при ошибке в method. </param>
        /// <param name="continueMethod">           (Optional) The continue method. Выполнится после
        ///                                         method и continueExceptionMethod. </param>
        public static void ExecuteAsync(Action<BulletinContext> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null)
        {
            _DCT.ExecuteAsync(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }
        /// <summary>   Executes the main thread operation. </summary>
        ///
        /// <remarks>   Fess59, 26.01.2018. </remarks>
        ///
        /// <param name="action">                   The action. </param>
        /// <param name="continueExceptionMethod">  (Optional) The continue exception method. Выполнится
        ///                                         при ошибке в method. </param>
        /// <param name="continueMethod">           (Optional) The continue method. Выполнится после
        ///                                         method и continueExceptionMethod. </param>
        public static void ExecuteMainThread(Action<BulletinContext> action, Action<BulletinContext, Exception> continueExceptionMethod = null, Action<BulletinContext> continueMethod = null)
        {
            _DCT.ExecuteMainThread(action, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }

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
