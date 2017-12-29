using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserTestApp.Tools
{
    /// <summary>
    /// Основная обертка приложения, хранит в себе:
    ///  - Контексты
    ///  - Настройки - реализовать
    ///  - Доступы к внешним интеграциям - реализовать
    ///  - Реализацию логгера и запись 
    /// </summary>
    /// <summary>
    /// Обёртка для DataContextTools. С типом данных
    /// Костыль короче - не должен лежать в общем проекте - надо положить в ClientTools
    /// </summary>
    internal static class _DCT
    {
        #region Property
        public static bool IsEnable
        {
            get
            {
                return DCT<CustomDataContext>.IsLogEnable;
            }
            set
            {
                if (value != DCT<CustomDataContext>.IsLogEnable)
                {
                    DCT<CustomDataContext>.IsLogEnable = value;
                }
            }
        }
        #endregion
        #region Constructor
        static _DCT()
        {
            DCT<CustomDataContext>.MethodNameLevel = 3;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Выполенение метода, для возврата значение используем стандартную обёртку с внешней переменной
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(Action<CustomDataContext> action, _DCTGroup group = _DCTGroup.None, object[] parameters = null, string comment = "", bool logInfo = true, Action<CustomDataContext, Exception> continueExceptionMethod = null, Action<CustomDataContext> continueMethod = null)
        {
            DCT<CustomDataContext>.Execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }

        public static TResult Execute<TResult>(Func<CustomDataContext, TResult> action, _DCTGroup group = _DCTGroup.None, object[] parameters = null, string comment = "", bool logInfo = true, Action<CustomDataContext, Exception> continueExceptionMethod = null, Action<CustomDataContext> continueMethod = null)
        {
            return DCT<CustomDataContext>.Execute<TResult>(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }

        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcherAsync(Action<CustomDataContext> action, System.Enum group = null, object[] parameters = null, string comment = "", bool logInfo = true, Action<CustomDataContext, Exception> continueExceptionMethod = null, Action<CustomDataContext> continueMethod = null)
        {
            DCT<CustomDataContext>.ExecuteCurrentDispatcherAsync(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcher(Action<CustomDataContext> action, System.Enum group = null, object[] parameters = null, string comment = "", bool logInfo = true, Action<CustomDataContext, Exception> continueExceptionMethod = null, Action<CustomDataContext> continueMethod = null)
        {
            DCT<CustomDataContext>.ExecuteCurrentDispatcher(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod);
        }
        public static void SendInfo(string v, bool isMessageBox = false)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] " + v);
            if (isMessageBox)
                Task.Factory.StartNew(() => MessageBox.Show(v));
        }
        #endregion
    }


    public enum _DCTGroup
    {
        None = 0,
        WebWorker = 1,
        BulletinContainerBase = 2,
        BulletinContainerList = 3,
        ContainerAvito = 4,
    }

}
