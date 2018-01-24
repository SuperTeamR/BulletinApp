using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTools
{
    /// <summary>
    /// Обёртка для всех методом приложения:
    /// - Перхват ошибок - заглушка для отладки
    /// - Централизация исполнения кода
    /// - Много поточность
    /// - Считаем количество выполнений
    /// - Замеряем время выполенния
    /// </summary>
    public static class DCT<T> where T : DC, new()
    {
        #region Property
        public static bool IsLogEnable { get; set; }
        public static int MethodNameLevel = 2;
        #endregion
        #region Constructor
        static DCT()
        {
            IsLogEnable = false;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Выполенение метода, для возврата значение используем стандартную обёртку с внешней переменной
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(Action<T> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName);
        }

        public static TResult Execute<TResult>(Func<T, TResult> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            return execute<TResult>(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName);
        }
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteAsync(Action<T> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            var task = new Task(() => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
            TaskStart(task);
            //TaskPool.Execute((a) => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
        }
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static Task<TResult> ExecuteAsync<TResult>(Func<T, TResult> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            var task = new Task<TResult>(() => execute<TResult>(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
            return TaskStart(task);
        }
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteAsyncPool(Action<T> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            var task = new Task(() => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
            TaskStart(task);
            //ThreadPoolHelper.Execute(() => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
            //TaskPool.Execute((a) => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
        }
        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcherAsync(Action<T> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            DispatcherHelper.Execute(() => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName));
        }

        /// <summary>
        /// Асинхронное выполенение метода, без заморочек просто Task
        /// </summary>
        /// <param name="action"></param>
        public static void ExecuteCurrentDispatcher(Action<T> action, System.Enum group, object[] parameters = null, string comment = "", bool logInfo = true, Action<T, Exception> continueExceptionMethod = null, Action<T> continueMethod = null)
        {
            var methodName = GetMethodName(MethodNameLevel);
            DispatcherHelper.Execute(() => execute(action, group, parameters, comment, logInfo: logInfo, continueExceptionMethod: continueExceptionMethod, continueMethod: continueMethod, methodName: methodName), false);
        }

        public static string GetMethodNameWrapper(int frame = 1)
        {
            return GetMethodName(frame);
        }

        //public static void SendInfo(ExecutionGroup group, string v)
        //{
        //    if (group == ExecutionGroup.Console)
        //        Console.WriteLine("[" + DateTime.Now.ToString() + "] " + v);
        //    if (group == ExecutionGroup.MessageBox)
        //        Task.Factory.StartNew(() => MessageBox.Show(v));
        //    DCT.SendInfo(group, v);
        //}


        /// <summary>
        /// Выполенение метода, для возврата значение используем стандртную обёртку с внешней переменной
        /// </summary>
        /// <param name="action"></param>
        private static void execute(Action<T> method,
            System.Enum group,
            object[] parameters = null,
            string comment = "",
            T _data = null,
            bool logInfo = true,
            Action<T, Exception> continueExceptionMethod = null,
            Action<T> continueMethod = null,
            string methodName = "")
        {
            execute<object>((data) => { method(data); return null; }, group, parameters, comment, _data, logInfo, continueExceptionMethod, continueMethod, methodName);
        }

        /// <summary>
        /// Выполенение метода, для возврата значение используем стандртную обёртку с внешней переменной
        /// </summary>
        /// <param name="action"></param>
        private static TResult execute<TResult>(
            Func<T, TResult> method,
            System.Enum group,
            object[] parameters = null,
            string comment = "",
            T _data = null,
            bool logInfo = true,
            Action<T, Exception> continueExceptionMethod = null,
            Action<T> continueMethod = null,
            string methodName = "")
        {
            TResult result = default(TResult);
            var sw = new Stopwatch();
            var trackNumber = Guid.Empty;
            sw.Start();
            try  ///Разрешённый try
            {
                if (method == null) throw new NullReferenceException("Parameter 'method' cannot be null");
                var data = _data == null ? new T() : _data;
                trackNumber = data.TrackNumber;
                result = method(data);
                if (_data == null) data.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                sw.Stop();
                //Метод продолжение в случае ошибки, с отловом ошибок
                if (continueExceptionMethod != null)
                    execute((data) => continueExceptionMethod(data, e), group, parameters, comment, _data, logInfo, methodName: methodName);
            }
            finally
            {
                if (sw.IsRunning)
                {
                    sw.Stop();
                }
                if (continueMethod != null)
                    execute((data) => continueMethod(data), group, parameters, comment, _data, logInfo, methodName: methodName);
            }
            return result;
        }
        internal static void LogDisable()
        {
            if (IsLogEnable)
            {
                IsLogEnable = false;
            }
        }
        internal static void LogEnable()
        {
            if (!IsLogEnable)
            {
                IsLogEnable = true;
            }
        }
        #endregion
        #region Tools




        /// <summary>
        /// Получаем имя текущего метода
        /// </summary>
        /// <returns></returns>
        internal static string GetMethodName(int frame = 1)
        {
            try ///Разрешённый try
            {
                StackTrace st = new StackTrace();
                StackFrame sf = st.GetFrame(frame);
                var method = sf.GetMethod();
                if (method.Name.Contains("<"))
                    return method.DeclaringType.FullName + "." + method.Name.Remove(0, 1).Remove(method.Name.IndexOf(">") - 1);
                return method.DeclaringType.FullName + "." + method.Name;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }
        #endregion
        #region Task dispatcher
        /// <summary>
        /// Заготовка
        /// </summary>
        /// <param name="task"></param>
        public static Task TaskStart(Task task)
        {
            task.Start();
            return task;
        }
        /// <summary>
        /// Заготовка
        /// </summary>
        /// <param name="task"></param>
        public static Task<TResult> TaskStart<TResult>(Task<TResult> task)
        {

            task.Start();
            return task;
        }
        #endregion
        #region Send message
        public static void SendInfo(System.Enum group, string comment, Guid? track = null)
        {
            Send(comment);
        }
        public static void SendWarning(System.Enum group, string comment, Guid? track = null)
        {
           
            Send(comment);
        }
        public static void SendExceptions(System.Enum group, string comment, Guid? track = null)
        {
            Send(comment);
        }
        public static void SendExceptions(System.Enum group, Exception ex, Guid? track = null)
        {
            Send(ex.ToString());
        }
        private static void Send(string comment)
        {
            if (IsLogEnable)
                Console.WriteLine(comment);
        }

        #endregion
        #region Models

        ///// <summary>
        ///// Параметры сообщения логирования. Информация в разрезе параметр:значение. Для снижения нагрузки на поиск и диск используем хэш в виде идентификатора
        ///// </summary>
        //protected class LogParameter
        //{
        //    #region Property
        //    /// <summary>
        //    /// Дата создания
        //    /// </summary>
        //    internal DateTime Create { get; set; }
        //    /// <summary>
        //    /// Идентификатор
        //    /// </summary>
        //    internal Guid Id { get; set; }
        //    /// <summary>
        //    /// Параметр - имя показателя
        //    /// </summary>
        //    internal string Name { get; set; }
        //    /// <summary>
        //    /// Значение
        //    /// </summary>
        //    internal string Value { get; set; }
        //    /// <summary>
        //    /// Параметры - сделать идентификатор хэшкодом (NAME+VALUE)
        //    /// </summary>
        //    internal string Hash { get; set; }
        //    #endregion
        //    #region Constructor
        //    internal LogParameter(string name, string value)
        //    {
        //        Id = Guid.NewGuid();
        //        Create = DateTime.Now;
        //        Name = name;
        //        Value = value;
        //        Hash = getHashSha256(Name + Value);
        //    }
        //    #endregion
        //    #region Method
        //    private string getHashSha256(string text)
        //    {
        //        byte[] bytes = Encoding.Unicode.GetBytes(text);
        //        SHA256Managed hashstring = new SHA256Managed();
        //        byte[] hash = hashstring.ComputeHash(bytes);
        //        string hashString = string.Empty;
        //        foreach (byte x in hash)
        //        {
        //            hashString += String.Format("{0:x2}", x);
        //        }
        //        return hashString;
        //    }
        //    #endregion
        //}
        #endregion
    }
}
