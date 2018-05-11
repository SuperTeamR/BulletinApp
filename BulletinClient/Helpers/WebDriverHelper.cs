using BulletinClient.Core;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Linq;
using System.Threading;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace BulletinClient.Helpers
{
    public static class BoardHelper
    {
        static void Find(FirefoxDriver driver, string tag, string attribute, string value, Action<IWebElement> action)
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

        static void JsClick(FirefoxDriver driver, IWebElement element, Action<IWebElement> after = null)
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

        public static bool Auth(FirefoxDriver driver, string login, string password)
        {
            driver.Navigate().GoToUrl("https://www.avito.ru/rossiya#login?next=%2Fprofile");
            WaitPage(driver, 30000, "Вход");
            ConsoleHelper.SendMessage($"Auth => Start from {login}");
            //WaitExecute(driver);
            Find(driver, "a", "data-marker", "header/login-button", e => JsClick(driver, e));
            WaitPage(driver, 30000, "Войти");
            ConsoleHelper.SendMessage($"Auth => Click from Enter");
            //WaitExecute(driver);
            Find(driver, "input", "data-marker", "login-form/login", e => e.SendKeys(login));
            Find(driver, "input", "data-marker", "login-form/password", e => e.SendKeys(password));
            //WaitExecute(driver);
            ConsoleHelper.SendMessage($"Auth => Set login and password");
            Find(driver, "button", "data-marker", "login-form/submit", e => JsClick(driver, e));
            var exceptionStrings = new string[] { "Неправильная пара логин", "Некорректный номер телефона", "Личный кабинет", "Доступ заблокирован", "Текст с картинки" };
            WaitPage(driver, 30000, exceptionStrings);
            var authResult = driver.Title.Contains("Личный кабинет");
            ConsoleHelper.SendMessage($"Auth => Complete, auth is {authResult}");



            if (authResult)
            {
                return true;
            }
            foreach (var str in exceptionStrings)
            {
                if (driver.PageSource.Contains(str))
                    ConsoleHelper.SendMessage($"Auth => Error - {str}");
            }
            return false;
        }

        static bool WaitPage(FirefoxDriver driver, int timeout, params string[] waitPatterns)
        {
            return WaitWebWorker("Page", driver, timeout, waitPatterns);
        }

        static bool WaitWebWorker(string code, FirefoxDriver driver, int timeout, params string[] waitPatterns)
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
    }
}