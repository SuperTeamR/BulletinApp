using BulletinBridge.Data;
using BulletinBridge.Models;
using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
using BulletinWebDriver.ServiceHelper;
using CollectorModels;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using FessooFramework.Tools.IOC;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        #region Timeouts
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
        protected bool WaitTitle(FirefoxDriver driver, int timeout, params string[] waitPatterns)
        {
            return WaitWebWorker("Title", driver, timeout, waitPatterns);
        }
        protected bool WaitPage(FirefoxDriver driver, int timeout, params string[] waitPatterns)
        {
            return WaitWebWorker("Page", driver, timeout, waitPatterns);
        }
        protected bool WaitUrl(FirefoxDriver driver, int timeout, params string[] waitPatterns)
        {
            return WaitWebWorker("Url", driver, timeout, waitPatterns);
        }

        protected bool WaitElementCountByCssSelector(FirefoxDriver driver, int timeout, int count, params string[] waitPatterns)
        {
            return WaitWebWorker("ElementCountByCssSelector", driver, timeout, count, waitPatterns);
        }

        protected bool WaitWebWorker(string code, FirefoxDriver driver, int timeout, int count,
            params string[] waitPatterns)
        {
            var result = false;
            try
            {
                ConsoleHelper.SendMessage($"WaitWebWorker => Wait load started. Type {code}, patterns {string.Join(";", waitPatterns)}");
                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
                wait.Until(d =>
                {
                    foreach (var pattern in waitPatterns)
                    {
                        switch (code)
                        {
                            case "ElementCountByCssSelector":
                                result = d.FindElements(By.CssSelector(pattern)).Count == count;
                                break;
                        }
                        if (result)
                            return result;
                    }
                    return result;
                    ;
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendMessage($"WAIT WEB DRIVER. Wait patterns not found from {driver.Url}. Patterns - {string.Join(";", waitPatterns)}");
            }
            return result;
        }

        protected bool WaitWebWorker(string code, FirefoxDriver driver, int timeout, params string[] waitPatterns)
        {
            var result = false;
            try
            {
                ConsoleHelper.SendMessage($"WaitWebWorker => Wait load started. Type {code}, patterns {string.Join(";", waitPatterns)}");
                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
                wait.Until(d =>
                {
                    foreach (var pattern in waitPatterns)
                    {
                        switch (code)
                        {
                            case "Title":
                                result = d.Title.Contains(pattern);
                                break;
                            case "Page":
                                result = d.PageSource.Contains(pattern);
                                break;
                            case "Url":
                                result = d.Url.Contains(pattern);
                                break;
                            default:
                                break;
                        }
                        if (result)
                            return result;
                    }
                    return result;
                    ;
                });
            }
            catch (Exception ex)
            {
                ConsoleHelper.SendMessage($"WAIT WEB DRIVER. Wait patterns not found from {driver.Url}. Patterns - {string.Join(";", waitPatterns)}");
            }
            return result;


        }
        protected bool Wait<TResult>(FirefoxDriver driver, Func<IWebDriver, TResult> condition, int timeout = 20)
        {
            var result = false;
            DCT.Execute(d =>
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.Until(condition);
                result = true;
            });
            return result;
        }
        protected Func<IWebDriver, bool> NoAttribute(FirefoxDriver driver, By query, string attribute)
        {
            return (d) =>
            {
                try
                {
                    var currValue = driver.FindElement(query).GetAttribute(attribute);
                    return currValue == null;
                }
                catch (NoSuchElementException)
                {
                    return false;
                }
                catch (StaleElementReferenceException)
                {
                    return false;
                }
            };
        }
        protected ReadOnlyCollection<IWebElement> FindMany(FirefoxDriver driver, By query)
        {
            var result = default(ReadOnlyCollection<IWebElement>);
            DCT.Execute(d =>
            {
                result = driver.FindElements(query);
            });
            return result;
        }
        #endregion
        #region Tools
        public void Execute(TaskCache task)
        {
            DCT.Execute(c =>
            {
                ConsoleHelper.SendMessage($"Task {UID}.{task.Command} started");
                switch (task.Command)
                {
                    case "AccessCheck":
                        checkAccess(task);
                        break;
                    case "ActivateBulletin":
                        executeCommand<TaskAccessCheckCache>(task, ActivateBulletins, false);
                        break;
                    case "InstancePublication":
                        executeCommand<TaskInstancePublicationCache>(task, (a, b) =>
                        {
                            var url = InstancePublication(a, b);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                var instance = BulletinInstanceHelper.Get(b.InstanceId);
                                instance.Url = url;
                                BulletinInstanceHelper.Save(instance);
                            }
                        });
                        break;
                    case "BulletinTemplateCollector":
                        executeCommand<TaskBulletinTemplateCollectorCache>(task, (a, b) =>
                        {
                            var templateModels = BulletinTemplateCollector(a, b);
                            BulletinTemplateHelper.Save(templateModels);
                        }, false);
                        break;
                    default:
                        ConsoleHelper.SendMessage($"Command '{task.Command}' not realized in BoardElement");
                        throw new Exception($"Command '{task.Command}' not realized in BoardElement");
                }
            }, continueExceptionMethod: (ex, continueMethod) =>
            {
                DriverTaskHelper.Error(task, ex.ToString());
                DriverTaskHelper.Complete(task);
            });
            ConsoleHelper.SendMessage($"Task {UID}.{task.Command} completed");

        }
        private void executeCommand<T>(TaskCache task, Action<FirefoxDriver, T> action, bool hasProxy = true)
            where T : CacheObject, new()
        {
            var taskModel = DriverTaskHelper.GetTask<T>(task);
            if (taskModel == null)
            {
                ConsoleHelper.SendException($"Command execute crash and stoped. Server return empty model, type of {typeof(T).Name}. Please check - 1. ServiceConfiguration. 2. Task model creator");
                throw new Exception($"Command execute crash and stoped. Server return empty model, type of {typeof(T).Name}. Please check - 1. ServiceConfiguration. 2. Task model creator");
            }
            //ProxyCardCheckCache proxy = null;
            ProxyCardCheckCache proxy = hasProxy ? ProxyHelper.GetProxy(URL, IPExceptionsString) : null;
            if (proxy == null && hasProxy)
            {
                ConsoleHelper.SendException($"Command execute crash and stoped, proxy not found or service not available");
                throw new Exception("Command execute crash and stoped, proxy not found or service not available");
            }
            else
            {

#if DEBUG && !DEBUG_REMOTE
       FirefoxHelper.ExecuteWithVisual(browser =>
#endif
#if RELEASE || DEBUG_REMOTE
                FirefoxHelper.ExecuteWithVisual(browser =>
#endif
                {
                    ToHome(browser);
                    action?.Invoke(browser, taskModel);
                }, proxy, 100);
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
                driver.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
            }
            catch (Exception ex)
            {

            }
        }
        public abstract bool Auth(FirefoxDriver driver, string login, string password);
        #endregion
        #region -- Steps
        private void checkAccess(TaskCache task)
        {
            DCT.Execute(c =>
            {
                var result = false;
                Guid? accessId = null;
                executeCommand<TaskAccessCheckCache>(task,
                (b, t) =>
                {
                    accessId = t.AccessId;
                    result = CheckAccess(b, t);
                });
                ConsoleHelper.SendMessage($"CheckAccess from {accessId} completed. IsEnabled{result}");
                if (accessId == null)
                    return;
                if (result)
                    AccessHelper.Enable(accessId.Value);
                else
                {
                    AccessHelper.Disable(accessId.Value);

                }
            }, continueExceptionMethod: (c, ex) =>
             {
                 //Задание завершилось с ошибкой
                 DriverTaskHelper.Error(task, ex.ToString());
             });
        }
        public abstract bool CheckAccess(FirefoxDriver driver, TaskAccessCheckCache taskModel);
        public abstract void ActivateBulletins(FirefoxDriver driver, TaskAccessCheckCache taskModel);
        public abstract string InstancePublication(FirefoxDriver driver, TaskInstancePublicationCache taskModel);
        public abstract IEnumerable<BulletinTemplateCache> BulletinTemplateCollector(FirefoxDriver driver, TaskBulletinTemplateCollectorCache taskModel);
        #endregion
        #region DriverTools
        public IWebElement FindElementSafe(IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public IWebElement FindElementSafe(IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool Exists(IWebElement element)
        {
            if (element == null)
            { return false; }
            return true;
        }
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
        protected void FindTagByTextContains(FirefoxDriver driver, string tag, string text, Action<IWebElement> action)
        {
            DCT.Execute(d =>
            {
                var elements = driver.FindElements(By.TagName(tag)).ToArray();
                IWebElement element = null;
                foreach (var item in elements)
                {
                    if (item.Text.Contains(text))
                    {
                        element = item;
                        break;
                    }
                }
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

        protected void JsClick(FirefoxDriver driver, By query, Action<IWebElement> after = null)
        {
            DCT.Execute(d =>
            {
                DoAction(driver, query, e => JsClick(driver, e, after));
            });
        }
        protected IWebElement DoAction(FirefoxDriver driver, By query, Action<IWebElement> after = null)
        {
            var result = default(IWebElement);
            DCT.Execute(d =>
            {
                var element = driver.FindElement(query);
                if (element != null && after != null)
                {
                    after(element);
                }
                result = element;

            });
            return result;
        }
        public IWebElement Find(FirefoxDriver driver, By query)
        {
            var result = default(IWebElement);
            DCT.Execute(d =>
            {
                result = driver.FindElement(query);
            });
            return result;
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
