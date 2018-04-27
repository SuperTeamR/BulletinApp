using BulletinBridge.Models;
using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using FessooFramework.Tools.IOC;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebDriver.Containers
{
    public abstract class BoardElement : _IOCElement
    {
        #region Property
        public abstract string URL { get; }
        public abstract IEnumerable<string> IPExceptionsString { get; }
        public abstract IEnumerable<string> BlockedExceptionsString { get; }
        public abstract int PageNavigationTimeout { get; }
        #endregion
        #region Constructor
        public BoardElement(string Name)
        {
            UID = Name;
        }
        #endregion
        #region Timeout - antirobot
        private DateTime? LastExecute { get; set; }
        private DateTime? NextExecute { get; set; }
        protected void WaitExecute(FirefoxDriver driver)
        {
            if (NextExecute != null && DateTime.Now.Ticks < NextExecute.Value.Ticks)
            {
                Thread.Sleep(500);
                WaitExecute(driver);
            }
            LastExecute = DateTime.Now;
            NextExecute = LastExecute.Value.AddMilliseconds(PageNavigationTimeout);
            if (IPExceptionsString != null)
                foreach (var str in IPExceptionsString)
                {
                    if (driver.PageSource.Contains(str))
                    {
                        driver.Close();
                        driver.Quit();
                    }
                }
            if (BlockedExceptionsString != null)
                foreach (var str in BlockedExceptionsString)
                {
                    if (driver.PageSource.Contains(str))
                    {
                        driver.Close();
                        driver.Quit();
                    }
                }
        }
        #endregion
        #region Tools
        public void Execute(TaskCache task)
        {
            DCT.Execute(c =>
            {
                switch (task.Command)
                {
                    case "AccessCheck":
                        checkAccess(task);
                        break;
                    case "InstancePublication":
                        executeCommand<TaskInstancePublicationCache>(task,(a, b) => InstancePublication(a, b));
                        break;
                    default:
                        ConsoleHelper.SendMessage($"Command '{task.Command}' not realized in BoardElement");
                        throw new Exception($"Command '{task.Command}' not realized in BoardElement");
                }
            }, continueExceptionMethod: (ex, continueMethod) =>
            {
                DriverTaskHelper.Error(task, ex.ToString());
            });
        }
        private void executeCommand<T>(TaskCache task, Action<FirefoxDriver, T> action)
            where T : CacheObject, new()
        {
            var taskModel = DriverTaskHelper.GetTask<T>(task);
            if (taskModel == null)
            {
                ConsoleHelper.SendException($"Command execute crash and stoped. Server return empty model, type of {typeof(T).Name}. Please check - 1. ServiceConfiguration. 2. Task model creator");
                throw new Exception($"Command execute crash and stoped. Server return empty model, type of {typeof(T).Name}. Please check - 1. ServiceConfiguration. 2. Task model creator");
            }
            var proxy = ProxyHelper.GetProxy(URL, IPExceptionsString);
            if (proxy == null)
            {
                ConsoleHelper.SendException($"Command execute crash and stoped, proxy not found or service not available");
                throw new Exception("Command execute crash and stoped, proxy not found or service not available");
            }
            else
            {
#if DEBUG
                FirefoxHelper.ExecuteWithVisual(browser =>
#else
                FirefoxHelper.ExecuteOne(browser =>

#endif
                {
                    ToHome(browser);
                    action?.Invoke(browser, taskModel);
                }, proxy);
                //Задание завершилось успешно
                DriverTaskHelper.Complete(task);
            }
        }
        #endregion
        #region Default
        protected void ToHome(FirefoxDriver driver)
        {
            try
            {
                WaitExecute(driver);
                driver.Navigate().GoToUrl(URL);
            }
            catch (Exception ex)
            {

            }
        }
        public abstract bool Auth(FirefoxDriver driver, string login, string password);
        #endregion
        #region 1.Check access
        private void checkAccess(TaskCache task)
        {
            DCT.Execute(c =>
            {
                var result = false;
                executeCommand<TaskAccessCheckCache>(task,
                (b, t) =>
                {
                    result = CheckAccess(b, t);
                });
                if (result)
                {
                    //Доступ включить
                }
                else
                {
                    //Доступ выключить
                }
            }, continueExceptionMethod: (c, ex) =>
             {
                //Задание завершилось с ошибкой
                DriverTaskHelper.Error(task, ex.ToString());
             });
        }
        public abstract bool CheckAccess(FirefoxDriver driver, TaskAccessCheckCache taskModel);
        public abstract bool InstancePublication(FirefoxDriver driver, TaskInstancePublicationCache taskModel);
        #endregion


        #region DriverTools
        protected void Find(FirefoxDriver driver, string tag, string attribute, string value, Action<IWebElement> action)
        {
            DCT.Execute(d =>
            {
                var element = driver.FindElements(By.TagName(tag)).FirstOrDefault(q => q.GetAttribute(attribute) == value);
                if (element != null && action != null)
                {
                    action(element);
                }
            });
        }
        protected void JsClick(FirefoxDriver driver, IWebElement element, Action<IWebElement> after = null)
        {
            DCT.Execute(d =>
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

                var internalActions = new Actions(driver);
                internalActions.MoveToElement(element).Click().Build().Perform();
                if (after != null)
                {
                    after(element);
                }
            });
        }
        //protected void NavigatePage()
        //{
        //    DCT.Execute(d =>
        //    {
        //        WaitExecute
        //        //driver.Navigate().GoToUrl(url);
        //        //Wait(q => ((IJavaScriptExecutor)q).ExecuteScript("return document.readyState").Equals("complete"), pageLoadingTimeout);
        //    });
        //}
            #endregion
        }
}
