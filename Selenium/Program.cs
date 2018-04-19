using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Selenium
{
    class Program
    {





        static void Main(string[] args)
        {

            //var validProxy = ProxyManager.UseProxy();

            ChromeOptions options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.images", 2);
            //options.AddArgument($"--proxy-server=http://{validProxy.Address}");

            using (IWebDriver driver = new ChromeDriver(options))
            {
                //driver.Navigate().GoToUrl("http://whatismyipaddress.com");

                //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(
                //    d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                //Console.ReadLine();


                driver.Navigate().GoToUrl("https://www.avito.ru/profile/login?next=%2Fprofile");

                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20)).Until(
                    d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                Do(driver, By.Name("login"),
                  (e) =>
                  {
                      e.SendKeys("Slava.Shleif@gmail.com");
                  });

                Do(driver, By.Name("password"),
                   (e) =>
                   {
                       e.SendKeys("OnlineHelp59");
                       e.Submit();
                   });


                AddBulletin(driver);

                Console.ReadLine();
            }
        }

        static void AddBulletin(IWebDriver driver)
        {
            try
            {
                var actions = new Actions(driver);

                driver.Navigate().GoToUrl("https://www.avito.ru/additem");

                //Categories
                var category1 = "Бытовая электроника";
                var category2 = "Телефоны";
                var category3 = "iPhone";

                Do(driver, By.CssSelector($"input[title='{category1}']"),
                    (e) =>
                    {
                        actions.MoveToElement(e).Click().Build().Perform();
                    });
                Do(driver, By.CssSelector($"input[title='{category2}']"),
                    (e) =>
                    {
                        actions.MoveToElement(e).Click().Build().Perform();
                    });
                Do(driver, By.CssSelector($"input[title='{category3}']"),
                    (e) =>
                    {
                        actions.MoveToElement(e).Click().Build().Perform();
                    });

                //Fields
                var bulletinTypeName = "params[2822]";
                var bulletinTitleId = "item-edit__title";
                var bulletinDescId = "item-edit__description";
                var bulletinPriceId = "item-edit__price";
                var bulletinTitleText = "Тестовое объявление";
                var bulletinDescText = "Тестовое описание\r\nОтличный товар";
                var bulletinPriceText = "2500";

                var bulletinImageName = "image";
                var bulletinImages = new[]
                {
                    "https://i.simpalsmedia.com/999.md/BoardImages/900x900/29b212371af4fbb93af45633fadb0a56.jpg",
                    "http://176.111.73.51:44444/Images/59ff199e231ede21488b0dab.jpg"


                };

                Do(driver, By.CssSelector($"input[name='{bulletinTypeName}']"),
                   (e) =>
                   {
                       actions.MoveToElement(e).Click().Build().Perform();
                   });
                Do(driver, By.CssSelector($"input[id='{bulletinTitleId}']"),
                  (e) =>
                  {
                      e.SendKeys(bulletinTitleText);
                  });
                Do(driver, By.CssSelector($"textarea[id='{bulletinDescId}']"),
                  (e) =>
                  {
                      e.SendKeys(bulletinDescText);
                  });
                Do(driver, By.CssSelector($"input[id='{bulletinPriceId}']"),
                  (e) =>
                  {
                      e.SendKeys(bulletinPriceText);
                  });


                foreach (var image in bulletinImages)
                {
                    new WebDriverWait(driver, TimeSpan.FromSeconds(20))
                       .Until(NoAttribute(driver, By.CssSelector($"input[name='{bulletinImageName}']"), "disabled"));
                    //WaitUntilElementVisible(driver, By.CssSelector($"input[name='{bulletinImageName}']"), 20);

                    Do(driver, By.CssSelector($"input[name='{bulletinImageName}']"),
                     (e) =>
                     {
                         e.Click();

                         Thread.Sleep(1000);
                         SendKeys.SendWait(image);

                     });

                    SendKeys.SendWait("{ENTER}");
                    //new WebDriverWait(driver, TimeSpan.FromSeconds(10))
                    //   .Until(AttributeEquals(driver, By.ClassName("form-uploader-item"), "data-state", "progress"));

                    Thread.Sleep(5000);

                    new WebDriverWait(driver, TimeSpan.FromSeconds(20))
                        .Until(ElementExists(driver, By.CssSelector($"input[name='{bulletinImageName}']")));

                    //new WebDriverWait(driver, TimeSpan.FromSeconds(20))
                    //    .Until(AttributeEquals(driver, By.ClassName($"img[name='images[]']"), "data-state", "active"));
                    //Thread.Sleep(5000);
                    //driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
                }

                var addedImagesCount = driver.FindElements(By.ClassName("form-uploader-item")).Where(q => q.GetAttribute("data-state") == "active").Count();

                if(addedImagesCount != bulletinImages.Length)
                    throw new Exception("Ошибка загрузки картинок");

                //var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                //var noProgress = wait.Until(NoAttributeEquals(driver, By.ClassName("form-uploader-item"), "data-state", "progress"));
                //if (!noProgress)
                //    throw new Exception("Ошибка загрузки картинок");

                Console.WriteLine("Успешная загрузка объявления");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ОШИБКА " + ex.ToString());
                SendKeys.SendWait("{ESC}");
                AddBulletin(driver);
            }
            
        }

        static IWebElement Do(IWebDriver driver, By query, Action<IWebElement> after = null)
        {
            var element = driver.FindElement(query);
            if(element != null && after != null)
            {
                after(element);
            }
            return element;
        }

        static IWebElement WaitUntilElementVisible(IWebDriver driver, By query, int timeout = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            return wait.Until(x => x.FindElement(query));
        }

        public static Func<IWebDriver, bool> NoAttribute(IWebDriver driver, By query, string attribute)
        {
            return (d) => {
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

        public static Func<IWebDriver, bool> NoAttributeEquals(IWebDriver driver, By query, string attribute, string value)
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
        public static Func<IWebDriver, bool> AttributeEquals(IWebDriver driver, By query, string attribute, string value)
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

        public static Func<IWebDriver, bool> ElementExists(IWebDriver driver, By query)
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
