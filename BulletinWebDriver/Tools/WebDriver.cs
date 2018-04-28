using CollectorModels;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;

namespace BulletinWebDriver.Tools
{
    static class WebDriver
    {
        public static bool IsDisableProxy { get; set; }
        static IWebDriver _driver;
        public static IWebDriver driver => _driver = _driver ?? CreateDriver();
        static Actions _actions;
        public static Actions Actions => _actions;
        public static ProxyCardCheckCache currentProxy;
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
                result = IsDisableProxy ? TryCreateWithoutProxy() : TryCreateDriverWithProxy();
            }); 
            return result;
        }
        static FirefoxDriver TryCreateWithoutProxy()
        {
            var result = default(FirefoxDriver);
            DCT.Execute(d =>
            {
                //var options = new FirefoxOptions();
                //options.SetPreference("permissions.default.image", 2);
                //options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);

                result = new FirefoxDriver();
            });
            return result;

        }
        static FirefoxDriver TryCreateDriverWithProxy()
        {
            var result = default(FirefoxDriver);
            var proxyIsFounded = false;
            DCT.Execute(d =>
            {
                if (currentProxy == null) CheckProxy();
                if (currentProxy != null)
                {
                    Console.WriteLine($"PROXY {currentProxy.Address}:{currentProxy.Port} --- PING:{currentProxy.PingLast}");
                    var options = new FirefoxOptions();
                    var proxy = new Proxy();
                    proxy.HttpProxy = $"{currentProxy.Address}:{currentProxy.Port}";
                    proxy.FtpProxy = $"{currentProxy.Address}:{currentProxy.Port}";
                    proxy.SslProxy = $"{currentProxy.Address}:{currentProxy.Port}";
                    options.Proxy = proxy;
                    options.SetPreference("permissions.default.image", 2);
                    options.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", false);

                    var tempDriver = new FirefoxDriver(options);
                    tempDriver.Navigate().GoToUrl("https://www.avito.ru");

                    if (!(tempDriver.Title.Contains("Доступ с вашего IP-адреса временно ограничен")
                    && tempDriver.Title.Contains("Доступ временно заблокирован")))
                    {
                        proxyIsFounded = true;
                        result = tempDriver;
                        UpdateActions(result);
                    }
                }
            });
            if (!proxyIsFounded)
            {
                CloseDriver();
                TryCreateDriverWithProxy();
            }
            return result;
        }

        public static ITimeouts GetTimeouts()
        {
            var result = default(ITimeouts);
            DCT.Execute(d =>
            {

                result = driver.Manage().Timeouts();
            });
            return result;
        }
        public static bool CheckProxy()
        {
            var result = false;
            DCT.Execute(d =>
            {
                var foundedProxy = false;
                var count = 0;
                while (!foundedProxy)
                {
                    var nextProxy = CollectorModels.Service.ProxyClientHelper.Next();
                    if (nextProxy == null)
                    {
                        Console.WriteLine("Proxy is null");
                        continue;

                    }
                    Console.WriteLine($"Proxy {count}|Avito:{nextProxy.Avito}|Https:{nextProxy.Https}|Http:{nextProxy.Http}|Google:{nextProxy.Google}|Ping:{nextProxy.PingLast}|={nextProxy.Address}:{nextProxy.Port}");

                    if (!CheckResponse(nextProxy)) continue;

                    currentProxy = nextProxy;
                    foundedProxy = true;
                    result = true;
                    count++;
                }
            });
            return result;
        }
        public static bool CheckResponse(ProxyCardCheckCache proxy)
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
        public static void CloseDriver()
        {
            currentProxy = null;
            if (driver != null)
            {
                driver.Close();
                driver.Quit();
                driver.Dispose();
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
        public static string GetTitle()
        {
            return driver.Title;
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
                //Wait(q => ((IJavaScriptExecutor)q).ExecuteScript("return document.readyState").Equals("complete"), pageLoadingTimeout);
            });
        }
        public static bool Wait<TResult>(Func<IWebDriver, TResult> condition, int timeout = 20)
        {
            var result = false;
            DCT.Execute(d =>
            {
                
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
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
        public static void Find(string tag, string attribute, string value, Action<IWebElement> action)
        {
            DCT.Execute(d =>
            {
                var element = driver.FindElements(By.TagName(tag)).FirstOrDefault(q => q.GetAttribute(attribute) == value);
                if(element != null && action != null)
                {
                    action(element);
                }
            });

        }
    }
}