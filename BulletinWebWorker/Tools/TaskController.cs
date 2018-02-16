using FessooFramework.Tools.Controllers;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebWorker.Tools
{
    /// <summary>
    /// Объект хранит в себе метод, поток и текущее состояние его выполнениия
    /// Блокирует попытку повторного запуска и завершает попытку без запуска потока 
    /// Позволяет подписаться на ряд событий - заверешние, запуск и тд
    /// </summary>
    class TaskController
    {
        #region Property
        ObjectController<System.Threading.Tasks.Task> task = new ObjectController<System.Threading.Tasks.Task>(null, isAsync: false);
        ActionController ExecuteAction = new ActionController();
        Func<bool> CheckAction { get; set; }
        Func<int> CheckTimeout { get; set; }
        ActionController AfterExecuteAction = new ActionController();
        object LockExecute = new object();
        #endregion
        #region Constructor
        /// <summary>
        /// Создаёт таск и ждёт выполнения
        /// </summary>
        /// <param name="execute">Действие выполеняемое в таске</param>
        /// <param name="check">Проверка требуеться ли повторить дейтсвие</param>
        /// <param name="checkTimeout">Таймаут перед повторением</param>
        public TaskController(Action execute)
        {
            if (execute == null)
                throw new NullReferenceException("Действие не может быть пустым");
            ExecuteAction.Set(execute);
            CheckAction = null;
        }
        /// <summary>
        /// Создаёт таск и ждёт выполнения
        /// </summary>
        /// <param name="execute">Действие выполеняемое в таске</param>
        /// <param name="check">Проверка требуеться ли повторить дейтсвие</param>
        /// <param name="checkTimeout">Таймаут перед повторением</param>
        public TaskController(Action execute, Func<bool> check, Func<int> checkTimeout)
        {
            if (execute == null)
                throw new NullReferenceException("Действие не может быть пустым");
            ExecuteAction.Set(execute);
            CheckAction = check;
            CheckTimeout = checkTimeout;
        }
        #endregion
        #region Methods
        public bool Execute()
        {
            var result = false;
            DCT.Execute(data =>
            {
                lock (LockExecute)
                {
                    if (task.Value != null && !task.Value.IsCompleted)
                        return;
                    task.Clear();
                    task.Value = System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        if (CheckAction == null)
                            ExecuteAction.Execute();
                        else
                        {
                            while (CheckAction())
                            {
                                ExecuteAction.Execute();
                                var timeout = 30000;
                                if (CheckTimeout != null)
                                    timeout = CheckTimeout();
                                Thread.Sleep(timeout >= 1000 ? timeout : 30000);
                            }
                        }
                    }).ContinueWith((a) =>
                    {
                        System.Threading.Tasks.Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(500);
                            if (a.IsCompleted)
                                AfterExecuteAction.Execute();
                        });
                    });

                    result = true;
                }
            });

            return result;
        }
        /// <summary>
        /// Выполняет после завершения таска
        /// </summary>
        /// <param name="action"></param>
        public void SetAfterExecute(Action action)
        {
            AfterExecuteAction.Set(action);
        }
        #endregion
    }
}
