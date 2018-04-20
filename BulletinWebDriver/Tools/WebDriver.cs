using CollectorModels;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebDriver.Tools
{
    static class WebDriver
    {
        static int pageLoadingTimeout = 20;
        static IWebDriver _driver;
        static IWebDriver driver => _driver = _driver ?? CreateDriver();
        static Actions _actions;
        public static Actions Actions => _actions;


        public static void RestartDriver()
        {
            CloseDriver();
            CreateDriver();
        }

        static FirefoxDriver CreateDriver()
        {
            var result = default(FirefoxDriver);
            DCT.Execute(d =>
            {
                var foundedProxy = false;
                while(!foundedProxy)
                {
                    var validProxy = CollectorModels.Service.ProxyClientHelper.Next();
                    if (validProxy == null) continue;
                    foundedProxy = true;

                    var isValid = CheckProxy(validProxy);
                    Console.WriteLine($"PROXY {validProxy.Address}:{validProxy.Port} --- PING:{validProxy.Ping} --- IS_VALID: {isValid}");

                    //var validProxy = ProxyManager.UseProxy();
                    var options = new FirefoxOptions();
                    var proxy = new Proxy();
                    proxy.HttpProxy = $"{validProxy.Address}:{validProxy.Port}";
                    proxy.FtpProxy = $"{validProxy.Address}:{validProxy.Port}";
                    proxy.SslProxy = $"{validProxy.Address}:{validProxy.Port}";
                    options.Proxy = proxy;
                    options.SetPreference("permissions.default.image", 2);
                    options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);
                    result = new FirefoxDriver(options);
                    UpdateActions(result);
                }
               
            }); 
            return result;
        }

        public static bool CheckProxy(ProxyCache proxy)
        {
            var result = false;
            HttpWebResponse response = null;
            DCT.Execute(d =>
            {
                WebRequest.DefaultWebProxy = new WebProxy
                {
                    Address = new Uri("http://" + proxy.Address + ":" + proxy.Port)
                };
                var request = WebRequest.Create("https://avito.ru");
                request.Timeout = 5000;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        result = true;
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    if (response != null) response.Close();
                }

            });
            return result;
        }

        static void CloseDriver()
        {
            if (driver != null)
            {
                driver.Close();
            }
        }
        static void UpdateActions(IWebDriver driver)
        {
            _actions = new Actions(driver);
        }
        public static void UpdateActions()
        {
            UpdateActions(driver);
        }
        public static IWebElement Find(By query)
        {
            var result = default(IWebElement);
            DCT.Execute(d =>
            {
                result = driver.FindElement(query);
            });
            return result;
        }
        public static ReadOnlyCollection<IWebElement> FindMany(By query)
        {
            var result = default(ReadOnlyCollection<IWebElement>);
            DCT.Execute(d =>
            {
                result = driver.FindElements(query);
            });
            return result;
        }

        public static void NavigatePage(string url)
        {
            DCT.Execute(d =>
            {
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(2000);
                //Wait(q => ((IJavaScriptExecutor)q).ExecuteScript("return document.readyState").Equals("complete"), pageLoadingTimeout);
            });
        }

        public static bool Wait<TResult>(Func<IWebDriver, TResult> condition, int timeout = 20)
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
        public static void JsClick(IWebElement element, Action<IWebElement> after = null)
        {
            DCT.Execute(d =>
            {
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

                var internalActions = new Actions(driver);
                internalActions.MoveToElement(element).Click().Build().Perform();
                if(after != null)
                {
                    after(element);
                }
            });
        }

        public static void JsClick(By query, Action<IWebElement> after = null)
        {
            DCT.Execute(d =>
            {
                DoAction(query, e => JsClick(e, after));
            });
        }
        public static IWebElement DoAction(By query, Action<IWebElement> after = null)
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

        public static void DoClick(By query)
        {
            DCT.Execute(d =>
            {
                var element = driver.FindElement(query);

                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                element = driver.FindElement(query);
                var internalActions = new Actions(driver);
                internalActions.Click(element).Build().Perform();
            });
        }

        public static Func<IWebDriver, bool> NoAttribute(By query, string attribute)
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

        public static Func<IWebDriver, bool> NoAttributeEquals(By query, string attribute, string value)
        {
            return (d) => {
                try
                {
                    var currValue = driver.FindElement(query).GetAttribute(attribute);
                    return currValue != value;
                }
                catch (NoSuchElementException)
                {
                    return true;
                }
                catch (StaleElementReferenceException)
                {
                    return true;
                }
            };
        }

        public static Func<IWebDriver, bool> AttributeEquals(By query, string attribute, string value)
        {
            return (d) => {
                try
                {
                    var currValue = driver.FindElement(query).GetAttribute(attribute);
                    return currValue == value;
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

        public static Func<IWebDriver, bool> ElementExists(By query)
        {
            return (d) => {
                try
                {
                    var element = driver.FindElement(query);
                    return element != null;
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
    }
}
