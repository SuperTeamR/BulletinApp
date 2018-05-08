using BulletinBridge.Models;
using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebDriver.Containers.BoardRealizations
{
    public class Avito : BoardElement
    {
        #region Property
        public override string URL { get => "https://www.avito.ru"; }
        public override IEnumerable<string> IPExceptionsString => new[] { "Доступ с вашего IP-адреса временно ограничен", "Доступ временно заблокирован", , "Ошибка при установлении защищённого соединения" };
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
            ConsoleHelper.SendMessage($"Auth => Start from {login}");
            WaitExecute(driver);
            Find(driver, "a", "data-marker", "header/login-button", e => JsClick(driver, e));
            WaitPage(driver, 30000, "Войти");
            ConsoleHelper.SendMessage($"Auth => Click from Enter");
            WaitExecute(driver);
            Find(driver, "input", "data-marker", "login-form/login", e => e.SendKeys(login));
            Find(driver, "input", "data-marker", "login-form/password", e => e.SendKeys(password));
            WaitExecute(driver);
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
                ConsoleHelper.SendMessage($"InstancePublication => Started from task {taskModel.Id}. Instance {taskModel.InstanceId}");
                WaitPage(driver, 30000, "www.avito.ru/additem");
                WaitExecute(driver);
               
                WaitPage(driver, 30000, "header-button-add-item");
                FindTagByTextContains(driver, "a", "Подать объявление", e => JsClick(driver, e));
                ConsoleHelper.SendMessage($"InstancePublication => Click from Add button");

                WaitPage(driver, 30000, "Выберите категорию");
                WaitExecute(driver);
                WaitPage(driver, 30000, "Животные");

                ConsoleHelper.SendMessage($"InstancePublication => Page from add bulletin loaded");
                //SetCategory first
                JsClick(driver, By.CssSelector($"input[title='Животные']"));
                ConsoleHelper.SendMessage($"InstancePublication => Set default category 'Животные'");
                WaitPage(driver, 10000, taskModel.Category1);
                //SetCategory
                if (!string.IsNullOrWhiteSpace(taskModel.Category1))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category1}']"));
                    WaitPage(driver, 10000, taskModel.Category2);
                    ConsoleHelper.SendMessage($"InstancePublication => Set category 1 {taskModel.Category1}");
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category2))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category2}']"));
                    WaitPage(driver, 10000, taskModel.Category3);
                    ConsoleHelper.SendMessage($"InstancePublication => Set category 2 {taskModel.Category2}");
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category3))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category3}']"));
                    WaitPage(driver, 10000, taskModel.Category4);
                    ConsoleHelper.SendMessage($"InstancePublication => Set category 3 {taskModel.Category3}");
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category4))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category4}']"));
                    WaitPage(driver, 10000, taskModel.Category5);
                    ConsoleHelper.SendMessage($"InstancePublication => Set category 4 {taskModel.Category4}");
                }
                if (!string.IsNullOrWhiteSpace(taskModel.Category5))
                {
                    JsClick(driver, By.CssSelector($"input[title='{taskModel.Category5}']"));
                    ConsoleHelper.SendMessage($"InstancePublication => Set category 5 {taskModel.Category5}");
                }

                //Select type
                JsClick(driver, By.CssSelector($"input[value='20018']"));
                ConsoleHelper.SendMessage($"InstancePublication => Select owner type");


                var comboboxCount = driver.FindElementByName("locationId");
                if (comboboxCount.Displayed)
                {
                    IList<IWebElement> allOptions = comboboxCount.FindElements(By.TagName("option"));
                    var option = allOptions.FirstOrDefault(q => q.Selected);
                    driver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", option, "value", "639180");
                    driver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", option, "data-parent-id", "637680");
                    ConsoleHelper.SendMessage($"InstancePublication => Set location from selector");
                }
                else
                {
                    DoAction(driver, By.CssSelector($"input[id='item-edit__address']"), e =>
                     {
                         e.Clear();
                         e.SendKeys("Московская область, Подольск");
                         e.SendKeys(OpenQA.Selenium.Keys.Return);
                         Find(driver, "li", "data-marker", "suggest(0)", e2 => JsClick(driver, e2));
                     });
                    ConsoleHelper.SendMessage($"InstancePublication => Set location from search string");
                }

                //Set fields
                //Title
                DoAction(driver, By.CssSelector($"input[id='item-edit__title']"), e => e.SendKeys(taskModel.Title));
                DoAction(driver, By.CssSelector($"textarea[id='item-edit__description']"), e => e.SendKeys(taskModel.Description));
                DoAction(driver, By.CssSelector($"input[id='item-edit__price']"), e => e.SendKeys(taskModel.Price));

                ConsoleHelper.SendMessage($"InstancePublication => Set fields");

                if (taskModel.Images != null && taskModel.Images.Any())
                {
                    var count = 0;
                    foreach (var img in taskModel.Images)
                    {
                        count++;
                        var file = ImageHelper.ImageToTemp(img);
                        var fileInput = driver.FindElementByCssSelector($"input[name='image']");
                        fileInput.SendKeys(file);
                        WaitElementCountByCssSelector(driver, 30, count, "div[class~='form-uploader-item'][data-state='active']");
                        ConsoleHelper.SendMessage($"InstancePublication => Set image {img}");
                    }
                }
                WaitExecute(driver);
                //Select base bulletin
                FindTagByTextContains(driver, "span", "Обычная продажа", e => JsClick(driver, e));
                WaitExecute(driver);
                ConsoleHelper.SendMessage($"InstancePublication => Select sale type ");

                FindTagByTextContains(driver, "button", "Продолжить с пакетом «Обычная продажа»", e => JsClick(driver, e));
                WaitExecute(driver);
                ConsoleHelper.SendMessage($"InstancePublication => Click continue");

                JsClick(driver, By.Id("service-premium"));
                WaitExecute(driver);
                JsClick(driver, By.Id("service-vip"));
                WaitExecute(driver);
                JsClick(driver, By.Id("service-highlight"));
                WaitExecute(driver);
                ConsoleHelper.SendMessage($"InstancePublication => Paid option has been disabled ");
                //Confirmation

                var button = FindMany(driver, By.TagName("button")).FirstOrDefault(q => q.Text == "Продолжить");
                JsClick(driver, button);
                WaitExecute(driver);
                ConsoleHelper.SendMessage($"InstancePublication => Click continue");
                //Get URL
                var a = Find(driver, By.XPath("//*[@class='content-text']/p/a"));
                var href = a.GetAttribute("href");
                result = href;
                ConsoleHelper.SendMessage($"InstancePublication => Get URL completed - {result}");
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        public override IEnumerable<BulletinTemplateCache> BulletinTemplateCollector(FirefoxDriver driver, TaskBulletinTemplateCollectorCache taskModel)
        {
            var result = new List<BulletinTemplateCache>();
            try
            {
                //Получаю данные
                var html = "";
                foreach (var query in taskModel.Queries)
                {
                    var pageCount = 2;
                    for (int i = 1; i <= pageCount; i++)
                    {
                        var url = $"https://www.avito.ru/rossiya?p={i}&q={query}";
                        WaitExecute(driver);
                        driver.Navigate().GoToUrl(url);
                        html += driver.PageSource;
                    }
                }
                //Получаю вхождения
                var pattern =
                    "id=\"([\\s,\\S,\\n].*?)\" data-type[\\s,\\S,\\n]*?" + //ID
                    "class=\"item-photo item-photo_large\">([\\s,\\S,\\n]*?)<div class=\"favorites[\\s,\\S,\\n]*?" + //Images
                    "class=\"item-description-title-link\"[\\s,\\S,\\n].*?href=\"([\\s,\\S,\\n].*?)\"[\\s,\\S,\\n]*?" + //Links
                    "title=\"([\\s,\\S,\\n].*?)\"[\\s,\\S,\\n]*?<div class=\"about"; //Title
                var patternLinks = "//([\\s,\\S,\\n].*?)\"";
                var matches = RegexHelper.Execute(pattern, html);
                foreach (var m in matches)
                {
                    try
                    {
                        var id = m.Groups[1].Value;

                        var imgSource = m.Groups[2].Value.ToString();
                        var imgMatches = RegexHelper.Execute(patternLinks, imgSource);
                        var images = imgMatches.Select(q => "https://" + q.Groups[1].Value);
                        var temp = new BulletinTemplateCache();
                        temp.URL = "https://avito.ru" + m.Groups[3].Value;
                        temp.Title = m.Groups[4].Value;
                        temp.Images = string.Join(";", images);

                        WaitExecute(driver);
                        driver.Navigate().GoToUrl(temp.URL);

                        //Price
                        var price = RegexHelper.GetValue("js-price-value-string\">([\\s,\\S,\\n,\\r]*?)&nbsp;", driver.PageSource);
                        try
                        {
                            if (price.Contains("Цена не указана") || price.Contains("Договорная") || string.IsNullOrWhiteSpace(price))
                                temp.Price = 0;
                            else
                            {
                                var priceText = price.Trim();
                                priceText = priceText.Replace(" ", "");
                                temp.Price = Int32.Parse(priceText);
                            }
                        }
                        catch (Exception ex)
                        {
                            var r3 = ex;
                        }
                        //Title
                        temp.Title = RegexHelper.GetValue("title-info-title-text\">([\\s,\\S,\\n].*?)</span>", driver.PageSource);
                        //Count
                        var count = RegexHelper.GetValue("<a href=\"#\" class=\"js-show-stat pseudo-link\"[\\s,\\S,\\n]*?>([\\s,\\S,\\n]*?)</a>", driver.PageSource);
                        try
                        {
                            if (string.IsNullOrEmpty(count))
                                temp.Count = 0;
                            else
                                temp.Count = Int32.Parse(Regex.Match(count, "\\d+").Value);
                        }
                        catch (Exception ex)
                        {
                            var r4 = ex;
                        }
                        //Description
                        temp.Description = RegexHelper.GetValue("itemprop=\"description\">([\\s,\\S,\\n]*?)</div>", driver.PageSource);
                        //Owner type
                        var seller = RegexHelper.GetValue("seller-info-name([\\s,\\S,\\n]*?)seller-info-value", driver.PageSource);
                        temp.IsIndividualSeller = seller.Contains("Продавец");
                        //City
                        var city = RegexHelper.GetValue("Адрес</div> <div class=\"seller-info-value\">([\\s,\\S,\\n]*?)</div>", driver.PageSource);
                        var cityParts = city.Split(',');
                        temp.Region1 = cityParts[0].Trim();
                        temp.Region2 = cityParts.Count() > 1 ? cityParts[1].Trim() : null;
                        //Category
                        var categoryText = RegexHelper.GetValue("<div class=\"breadcrumbs js-breadcrumbs\">([\\s,\\S,\\n]*?)</div>", driver.PageSource);
                        var categories = RegexHelper.Execute("title=\".*?>([\\s,\\S,\\n]*?)</a>", categoryText).ToArray();
                        var rawCategories = new string[5];
                        for (var i = 1; i < categories.Length; i++)
                        {
                            var categoryElement = categories[i];
                            rawCategories[i - 1] = categoryElement.Groups[1].Value;
                        }
                        temp.Category1 = rawCategories[0];
                        temp.Category2 = rawCategories[1];
                        temp.Category3 = rawCategories[2];
                        temp.Category4 = rawCategories[3];
                        temp.Category5 = rawCategories[4];

                        temp.IsHandled = true;
                        result.Add(temp);
                    }
                    catch (Exception ex)
                    {
                        var r2 = ex;
                    }
                }
            }
            catch (Exception ex)
            {
                var r = ex;
            }
            return result;
        }
        private string Clear(string str)
        {
            str = str.Replace("\r\n", "");
            return str;
        }
        #endregion
    }
}
