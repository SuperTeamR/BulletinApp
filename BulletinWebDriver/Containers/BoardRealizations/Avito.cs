using BulletinBridge.Models;
using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebDriver.Containers.BoardRealizations
{
    public class Avito : BoardElement
    {
        #region Property
        public override string URL { get => "https://www.avito.ru"; }
        public override IEnumerable<string> IPExceptionsString => new[] { "Доступ с вашего IP-адреса временно ограничен", "Доступ временно заблокирован" };
        public override IEnumerable<string> BlockedExceptionsString => new[] { "Учётная запись заблокирована по причине", "Доступ заблокирован" };
        public override int PageNavigationTimeout => 3000;
        #endregion
        #region Constructor
        public Avito() : base("Avito")
        {
        }
        #endregion
        #region Default
        public override bool Auth(FirefoxDriver driver, string login, string password)
        {
            WaitExecute(driver);
            Find(driver, "a", "data-marker", "header/login-button", e => JsClick(driver, e));
            WaitPage(driver, 30000, "Войти");
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            WaitExecute(driver);
            Find(driver, "input", "data-marker", "login-form/login", e => e.SendKeys(login));
            Find(driver, "input", "data-marker", "login-form/password", e => e.SendKeys(password));
            WaitExecute(driver);
            Find(driver, "button", "data-marker", "login-form/submit", e => JsClick(driver, e));
            WaitPage(driver, 30000, "Неправильная пара логин", "Некорректный номер телефона", "Личный кабинет");
            if (driver.Title.Contains("Личный кабинет"))
            {
                return true;
            }
            return false;
        }
        #endregion
        #region Realization
        public override bool CheckAccess(FirefoxDriver driver, TaskAccessCheckCache taskModel)
        {
            return Auth(driver, taskModel.Login, taskModel.Password);
            //return Auth(driver, "RyboMan02@gmail.com", "Zcvb208x");
        }

        public override string InstancePublication(FirefoxDriver driver, TaskInstancePublicationCache taskModel)
        {
            string result = null;
            try
            {
                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return result;
                WaitPage(driver, 60000, "www.avito.ru/additem");
                WaitExecute(driver);

                WaitPage(driver, 30000, "header-button-add-item");
                FindTagByTextContains(driver, "a", "Подать объявление", e => JsClick(driver, e));

                WaitPage(driver, 30000, "Выберите категорию");
                WaitExecute(driver);

                Thread.Sleep(1000);
                //SetCategory
                if (!string.IsNullOrWhiteSpace(taskModel.Category1))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category1}']"));
                    WaitPage(driver, 10000, taskModel.Category2);
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category2))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category2}']"));
                    WaitPage(driver, 10000, taskModel.Category3);
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category3))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category3}']"));
                    WaitPage(driver, 10000, taskModel.Category4);
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category4))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category4}']"));
                    WaitPage(driver, 10000, taskModel.Category5);
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category5))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category5}']"));
                    Thread.Sleep(1000);
                }

                //Select type
                JsClick(driver, By.CssSelector($"input[value='20018']"));

                //

                //Select city
                //Another
                //var comboboxCount = driver.FindElementByName("locationId");
                //IList<IWebElement> allOptions = comboboxCount.FindElements(By.TagName("option"));
                //var option = allOptions.FirstOrDefault(q => q.Text.Contains("Выбрать другой"));
                //if (option != null)
                //    option.Click();

                ////Select first city
                //Find(driver, "select", "class", "js-regions-region", e =>
                //{
                //    IList<IWebElement> fistCities = e.FindElements(By.TagName("option"));
                //    var city = fistCities.FirstOrDefault(q => q.Text.Contains("Московская область"));
                //    city.Click();
                //});
                ////Select second city
                //Find(driver, "select", "class", "js-regions-city", e =>
                //{
                //    IList<IWebElement> fistCities = e.FindElements(By.TagName("option"));
                //    var city = fistCities.FirstOrDefault(q => q.Text.Contains("Подольск"));
                //    city.Click();
                //});
                ////Select confirmation
                //FindTagByTextContains(driver, "button", "Выбрать", e => JsClick(driver, e));
                DoAction(driver, By.CssSelector($"input[id='item-edit__address']"), e =>
                {
                    e.Clear();
                    e.SendKeys("Подольск");
                    });

                //Set fields
                //Title
                DoAction(driver, By.CssSelector($"input[id='item-edit__title']"), e => e.SendKeys(taskModel.Title));
                DoAction(driver, By.CssSelector($"textarea[id='item-edit__description']"), e => e.SendKeys(taskModel.Description));
                DoAction(driver, By.CssSelector($"input[id='item-edit__price']"), e => e.SendKeys(taskModel.Price));

                if (taskModel.Images != null && taskModel.Images.Any())
                {
                    foreach (var img in taskModel.Images)
                    {
                        var file = ImageHelper.ImageToTemp(img);
                        var fileInput = driver.FindElementByCssSelector($"input[name='image']");
                        fileInput.SendKeys(file);
                        Thread.Sleep(15000);
                    }
                }
                WaitExecute(driver);
                //Select base bulletin
                FindTagByTextContains(driver, "span", "Обычная продажа", e => JsClick(driver, e));
                WaitExecute(driver);

                FindTagByTextContains(driver, "button", "Продолжить с пакетом «Обычная продажа»", e => JsClick(driver, e));
                WaitExecute(driver);

                JsClick(driver, By.Id("service-premium"));
                WaitExecute(driver);
                JsClick(driver, By.Id("service-vip"));
                WaitExecute(driver);
                JsClick(driver, By.Id("service-highlight"));
                WaitExecute(driver);
                //Confirmation
                var button = FindMany(driver, By.TagName("button")).FirstOrDefault(q => q.Text == "Продолжить");
                JsClick(driver, button);
                WaitExecute(driver);
                //Get URL
                var a = Find(driver, By.XPath("//*[@class='content-text']/p/a"));
                var href = a.GetAttribute("href");
                result = href;
            }
            catch (Exception ex)
            {
                
            }
            return result;
        }
        #endregion
    }
}
