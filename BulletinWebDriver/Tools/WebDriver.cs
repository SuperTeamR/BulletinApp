using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        static FirefoxDriver CreateDriver()
        {
            var result = default(FirefoxDriver);
            DCT.Execute(d =>
            {

                //var validProxy = ProxyManager.UseProxy();
                //ChromeOptions options = new ChromeOptions();
                //options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
                //options.AddArgument($"--proxy-server=http://{validProxy.Address}");
                result = new FirefoxDriver();
                UpdateActions(result);
            });
            return result;
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
        public static void JsClick(IWebElement element)
        {
            DCT.Execute(d =>
            {
                Actions.MoveToElement(element).Click().Build().Perform();

            });
        }

        public static void JsClick(By query)
        {
            DCT.Execute(d =>
            {
                DoAction(query, JsClick);
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
