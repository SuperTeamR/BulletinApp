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
using BulletinBridge.Data;
using BulletinWebDriver.Tools;
using BulletinWebDriver.ServiceHelper;

namespace BulletinWebDriver.Containers.BoardRealizations
{
    public class Avito : BoardElement
    {
        #region Property
        public override string URL { get => "https://www.avito.ru"; }
        public override IEnumerable<string> IPExceptionsString => new[] { "Доступ с вашего IP-адреса временно ограничен", "Доступ временно заблокирован", "Ошибка при установлении защищённого соединения" };
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
            SendMessage($"Auth => Start from {login}");
            WaitExecute(driver);
            Find(driver, "a", "data-marker", "header/login-button", e => JsClick(driver, e));
            WaitPage(driver, 30000, "Войти");
            SendMessage($"Auth => Click from Enter");
            WaitExecute(driver);

            var authResult = false;
            var exceptionRegistrationFail = "Неправильная пара логин";
            var exceptionStrings = new string[] { "Неправильная пара логин", "Некорректный номер телефона", "Личный кабинет", "Доступ заблокирован", "Текст с картинки" };

            for (int i = 0; i < 30; i++)
            {
                Find(driver, "input", "data-marker", "login-form/login",
                    e => {
                        e.Clear();
                        e.SendKeys(login);
                    });
                Find(driver, "input", "data-marker", "login-form/password",
                    e =>
                    {
                        e.Clear();
                        e.SendKeys(password);
                    });
                if(HasContains(driver, "div", "class", "captcha-root"))
                {
                    var capcha = "";
                    var capchaPath = "";
                    FindContains(driver, "div", "class", "captcha-image", e =>
                    {
                        capchaPath = GetScreenshotElementWithOffset(driver, e, login, new System.Drawing.Point(-1, -e.Size.Height - 5));
                    });

                    capcha = FreegateHelper.ImageToText(capchaPath);
                    DoAction(driver, By.CssSelector($"input[placeholder='Текст с картинки']"), e =>
                    {
                        e.Clear();
                        e.SendKeys(capcha);
                    });
                }

                Find(driver, "button", "data-marker", "login-form/submit", e => JsClick(driver, e));
             
                WaitPage(driver, 30000, exceptionStrings);
                authResult = driver.Title.Contains("Личный кабинет");
                if (authResult)
                    break;
                if (driver.PageSource.Contains(exceptionRegistrationFail))
                    break;
                Thread.Sleep(2000);
            }
            SendMessage($"Auth => Complete, auth is {authResult}");
            
            if (authResult)
            {
                return true;
            }
            foreach (var str in exceptionStrings)
            {
                if (driver.PageSource.Contains(str))
                    SendMessage($"Auth => Error - {str}");
            }
            return false;
        }
        #endregion
        #region Realization

        public override IEnumerable<MessageCache> CollectMessages(FirefoxDriver driver, TaskMessageCollectorCache taskModel)
        {
            var result = Enumerable.Empty<MessageCache>();
            try
            {
                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return result;

                //Сбор сообщений
                var messages = new List<MessageCache>();
                driver.Navigate().GoToUrl($"http://www.avito.ru/profile/messenger");
                WaitPage(driver, 10000, $"www.avito.ru/profile/messenger");
                var pageSource = driver.PageSource;

                var commonPattern = "class=\"messenger-channel([\\s\\S\\r\\n]*?messenger-channel-text-overflow[\\s\\S\\r\\n]*?)</div>";
                var datetimePattern = "class=\"messenger-channel-datetime\"[\\s\\S\\r\\n]*?datetime=\"(.*?)\"";
                var textPattern = "messenger-channel-text-overflow[\\s\\S\\r\\n]*?<span>(.*?)</span>";
                var textPattern2 = "messenger-channel-text-overflow[\\s\\S\\r\\n]*?<!-- react-text: \\d+ -->(.*?)<!-";

                var matches = RegexHelper.Execute(commonPattern, pageSource);
                foreach(var match in matches)
                {
                    var messageSource = match.Value;

                    var datetimeStr = RegexHelper.GetValue(datetimePattern, messageSource);
                    var datetime = DateTime.Parse(datetimeStr);
                    if (taskModel.LastMessage != null && datetime < taskModel.LastMessage) continue;

                    var text = RegexHelper.GetValue(textPattern, messageSource);
                    if (string.IsNullOrEmpty(text))
                        text = RegexHelper.GetValue(textPattern2, messageSource);
                    if (string.IsNullOrEmpty(text))
                        text = messageSource;
                    var cache = new MessageCache();
                    cache.AccessId = taskModel.AccessId;
                    cache.Text = text;
                    cache.PublicationDate = datetime;

                    messages.Add(cache);
                }

                result = messages;
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public override AccessStatistics GetAccessStatistics(FirefoxDriver driver, TaskAccessCheckCache taskModel)
        {
            AccessStatistics result = null;
            try
            {
                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return null;

                //Сбор просмотров
                var totalViews = 0;
                var rawViews = new List<string>();
                var nextPagePattern = "(Следующая страница)";
                var viewPattern = "class=\"profile-item-views-count-icon[\\s\\S\\r\\n]*?(\\d+)";
                var count = 1;
                var hasNextPage = true;
                while (hasNextPage)
                {
                    count++;
                    var html = driver.PageSource;
                    hasNextPage = !string.IsNullOrEmpty(RegexHelper.GetValue(nextPagePattern, html));

                    var matches = RegexHelper.Execute(viewPattern, html).ToArray();
                    if (matches.Any())
                        rawViews.AddRange(matches.Select(q => q.Groups[1].Value));

                    if (hasNextPage)
                    {
                        driver.Navigate().GoToUrl($"http://www.avito.ru/profile/items/active/rossiya?p={count}&s=4");
                        WaitPage(driver, 30000, $"www.avito.ru/profile/items/active/rossiya?p={count}&s=4");
                    }
                }
                foreach (var rawView in rawViews)
                {
                    var success = Int32.TryParse(rawView, out var viewCount);

                    if (success)
                        totalViews += viewCount;
                }

                //Сбор сообщений
                driver.Navigate().GoToUrl($"http://www.avito.ru/profile/messenger");
                WaitPage(driver, 30000, $"www.avito.ru/profile/messenger");
                var pageSource = driver.PageSource;


                var totalMessages = 0;
                var messagePattern = "(messenger-channel-context)";
                var messageMatches = RegexHelper.Execute(messagePattern, pageSource).ToArray();
                totalMessages = messageMatches.Count();


                var stat = new AccessStatistics();
                stat.Views = totalViews;
                stat.Messages = totalMessages;
                result = stat;
            }
            catch (Exception ex)
            {
            }
            return result;            

        }

        public override int? GetInstanceStatistics(FirefoxDriver driver, TaskInstanceStatisticsCache taskModel)
        {
            int? result = null;
            try
            {
                var viewPattern = "class=\"title-info-views[\\s\\S\\r\\n]*?}\">[\\s\\S\\r\\n]*?(\\d+)";
                driver.Navigate().GoToUrl(taskModel.Url);
                WaitPage(driver, 30000, taskModel.Url);
                var pageSource = driver.PageSource;
                var rawView = RegexHelper.GetValue(viewPattern, pageSource);
                var success = Int32.TryParse(rawView, out var viewCount);
                if (success)
                    result = viewCount;
            }
            catch (Exception ex)
            {
            }
            return result;

        }

        const string forwardNumber = "9264200230";

        public override AccessCache Registration(FirefoxDriver driver, AccessCache access)
        {
            var result = access;
            DCT.Execute(c =>
            {
                var tzid = -1;

                if (access.IsForwarding)
                {
                    SendMessage($"Registration => Wait ForwardNumber");
                    tzid = OnlineSimHelper.GetForward(forwardNumber).tzid;
                }
                else
                {
                    SendMessage($"Registration => Wait Number");
                    tzid = OnlineSimHelper.GetNum(ServiceType.avito).tzid;
                }
                SendMessage($"Registration => tzid: {tzid}");
                var getState = OnlineSimHelper.GetState(tzid);
                if(getState == null)
                {
                    SendMessage($"Registration => GetState is null. Abort operation");
                    return;
                }
                access.Phone = getState.number;
                access.PhoneTZID = getState.tzid;
                SendMessage($"Registration => Phone: {access.Phone}");

                WaitExecute(driver);
                //Перехожу в регистрацию
                driver.Navigate().GoToUrl("https://www.avito.ru/registration");
                WaitExecute(driver);

                //Заполняю все поля
                DoAction(driver, By.CssSelector($"input[name='name']"), e =>
                {
                    e.Clear();
                    e.SendKeys("Валера");
                });
                DoAction(driver, By.CssSelector($"input[name='email']"), e =>
                {
                    e.Clear();
                    e.SendKeys(access.Login);
                });
                var phone = access.Phone.Replace("+7", "");
                DoAction(driver, By.CssSelector($"input[name='phone']"), e =>
                {
                    e.Clear();
                    e.SendKeys(phone);
                });
              
                for (int i = 0; i < 30; i++)
                {
                    DoAction(driver, By.CssSelector($"input[name='password']"), e =>
                    {
                        e.Clear();
                        e.SendKeys(access.Password);
                    });
                    var capcha = "";
                    var capchaPath = "";
                    Find(driver, "img", "class", "form-captcha-image js-form-captcha-image", e =>
                    {
                        capchaPath = GetScreenshotElement(driver, e, access.Login);
                    });
                    capcha = FreegateHelper.ImageToText(capchaPath);
                    DoAction(driver, By.CssSelector($"input[name='captcha']"), e =>
                    {
                        e.Clear();
                        e.SendKeys(capcha);
                    });
                    FindTagByTextContains(driver, "button", "Зарегистрироваться", x => JsClick(driver, x));
                    Thread.Sleep(2000);
                    if (driver.PageSource.Contains("Регистрация. Шаг 2"))
                        break;
                }

                if (!driver.PageSource.Contains("Регистрация. Шаг 2"))
                {
                    SendMessage("Registration => Captcha is not resolved");
                    AccessHelper.Disable(access.Id);
                    return;
                }

                Find(driver, "button", "class", "button button-azure js-phone-checker-request-code", x => JsClick(driver, x));
                for (int i = 0; i < 30; i++)
                {
                    Thread.Sleep(3000);
                    var state = getState = OnlineSimHelper.GetState(tzid);
                    SendMessage($"Registration => Wait SMS, state = {state.response}");
                    if (state.responseEnum == getStateState.TZ_NUM_ANSWER)
                    {
                        DoAction(driver, By.CssSelector($"input[id='phone-checker-code']"), e =>
                        {
                            e.Clear();
                            e.SendKeys(state.msg);
                        });
                        break;
                    }
                }
                Find(driver, "button", "class", "button button-origin button-origin_large button-origin-green js-registration-form-submit", x => JsClick(driver, x));
                //Получаю смску
                Thread.Sleep(2000);
                //Подтверждаю
                AccessHelper.Enable(access.Id);
            });
            return access;
        }

        public override bool CheckAccess(FirefoxDriver driver, TaskAccessCheckCache taskModel)
        {
            return Auth(driver, taskModel.Login, taskModel.Password);
            //return Auth(driver, "RyboMan02@gmail.com", "Zcvb208x");
        }

        public override void ActivateBulletins(FirefoxDriver driver, TaskAccessCheckCache taskModel)

        {
            try
            {
                var inactivePage = @"www.avito.ru/profile/items/inactive";

                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return;
                WaitPage(driver, 3000, inactivePage);
                WaitExecute(driver);

                var ids = new List<string>();
                var nextPagePattern = "(Следующая страница)";
                var buttonPattern = "profile-item-title\">[\\s\\S\\r\\n]*?<a name=\"item_([\\d]+)";
                var count = 1;
                var hasNextPage = true;
                while (hasNextPage)
                {
                    count++;
                    var html = driver.PageSource;
                    hasNextPage = !string.IsNullOrEmpty(RegexHelper.GetValue(nextPagePattern, html));

                    var matches = RegexHelper.Execute(buttonPattern, html).ToArray();
                    if (matches.Any())
                        ids.AddRange(matches.Select(q => q.Groups[1].Value));

                    if (hasNextPage)
                    {
                        driver.Navigate().GoToUrl("http://" + inactivePage + $"/rossiya?p={count}&s=4");
                        WaitPage(driver, 30000, inactivePage + $"/rossiya?p={count}&s=4");
                    }
                }
                foreach (var id in ids)
                {
                    SendMessage($"ActivateBulletins => Trying to activate bulletin with Id {id}");
                    var activationLink = @"http://www.avito.ru/packages/put_free_package?item_id=" + id;
                    driver.Navigate().GoToUrl(activationLink);
                    //WaitExecute(driver);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public override void ActivateBulletin(FirefoxDriver driver, TaskInstanceActivationCache taskModel)
        {
            try
            {
                var inactivePage = @"www.avito.ru/profile/items/inactive";

                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return;
                WaitPage(driver, 3000, inactivePage);
                WaitExecute(driver);

                var idPattern = ".*?_(\\d+)$";
                var id = RegexHelper.GetValue(idPattern, taskModel.Url);
                SendMessage($"ActivateBulletins => Trying to activate bulletin with Id {id}");
                //https://www.avito.ru/account/pay_fee?item_id=1717723420&vas_from=item_self_user
                var activationLink = @"http://www.avito.ru/packages/put_free_package?item_id=" + id;

                driver.Navigate().GoToUrl(activationLink);
                WaitExecute(driver);
            }
            catch (Exception ex)
            {
            }
        }

        public override string InstancePublication(FirefoxDriver driver, TaskInstancePublicationCache taskModel)
        {
            string result = null;
            try
            {
                if (!Auth(driver, taskModel.Login, taskModel.Password))
                    return result;
                SendMessage($"InstancePublication => Started from task {taskModel.Id}. Instance {taskModel.InstanceId}");
                WaitPage(driver, 30000, "www.avito.ru/additem");
                WaitExecute(driver);

                WaitPage(driver, 30000, "header-button-add-item");

                FindTagByTextContains(driver, "a", "Подать объявление", e => JsClick(driver, e));
                SendMessage($"InstancePublication => Click from Add button");

                WaitPage(driver, 30000, "Выберите категорию");
                WaitExecute(driver);
                WaitPage(driver, 30000, "Животные");

                SendMessage($"InstancePublication => Page from add bulletin loaded");

                var setCategory = false;
                var oldImplicitWait = driver.Manage().Timeouts().ImplicitWait;
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(60000));
                wait.Until(d =>
                {
                    var category1 = false;
                    var category2 = false;
                    var category3 = false;
                    var category4 = false;
                    var category5 = false;

                    //SetCategory first
                    FindTagByTextContains(driver, "button", "Животные", e => JsClick(driver, e));
                    SendMessage($"InstancePublication => Set default category 'Животные'");
                    WaitPage(driver, 10000, taskModel.Category1);
                    //SetCategory
                    if (!string.IsNullOrWhiteSpace(taskModel.Category1))
                    {
                        FindTagByTextContains(driver, "button", taskModel.Category1, e => JsClick(driver, e));
                        if (WaitPage(driver, 10000, taskModel.Category2))
                        {
                            SendMessage($"InstancePublication => Set category 1 {taskModel.Category1}");
                            category1 = true;
                        };
                    }
                    else
                        category1 = true;
                    if (!string.IsNullOrWhiteSpace(taskModel.Category2))
                    {
                        FindTagByTextContains(driver, "button", taskModel.Category2, e => JsClick(driver, e));
                        if (WaitPage(driver, 10000, taskModel.Category3))
                        {
                            SendMessage($"InstancePublication => Set category 2 {taskModel.Category2}");
                            category2 = true;
                        }
                    }
                    else
                        category2 = true;
                    if (!string.IsNullOrWhiteSpace(taskModel.Category3))
                    {
                        FindTagByTextContains(driver, "button", taskModel.Category3, e => JsClick(driver, e));
                        if (WaitPage(driver, 10000, taskModel.Category4))
                        {
                            SendMessage($"InstancePublication => Set category 3 {taskModel.Category3}");
                            category3 = true;
                        }
                    }
                    else
                        category3 = true;
                    if (!string.IsNullOrWhiteSpace(taskModel.Category4))
                    {
                        FindTagByTextContains(driver, "button", taskModel.Category4, e => JsClick(driver, e));
                        if (WaitPage(driver, 10000, taskModel.Category5))
                        {
                            SendMessage($"InstancePublication => Set category 4 {taskModel.Category4}");
                            category4 = true;
                        }
                    }
                    else
                        category4 = true;
                    if (!string.IsNullOrWhiteSpace(taskModel.Category5))
                    {
                        FindTagByTextContains(driver, "button", taskModel.Category5, e => JsClick(driver, e));
                        SendMessage($"InstancePublication => Set category 5 {taskModel.Category5}");
                        category5 = true;
                    }
                    else
                        category5 = true;

                    if (category1 && category2 && category3 && category4 && category5)
                    {
                        setCategory = true;
                        return true;
                    }
                    return false;
                });
                driver.Manage().Timeouts().ImplicitWait = oldImplicitWait;
                if (setCategory)
                {
                    SendMessage($"InstancePublication => Category");
                }

                //Select type
                JsClick(driver, By.CssSelector($"input[value='20018']"));
                SendMessage($"InstancePublication => Select owner type");


                var comboboxCount = driver.FindElementByName("locationId");
                if (comboboxCount.Displayed)
                {
                    IList<IWebElement> allOptions = comboboxCount.FindElements(By.TagName("option"));
                    var option = allOptions.FirstOrDefault(q => q.Selected);
                    driver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", option, "value", "639180");
                    driver.ExecuteScript("arguments[0].setAttribute(arguments[1], arguments[2]);", option, "data-parent-id", "637680");
                    SendMessage($"InstancePublication => Set location from selector");
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
                    SendMessage($"InstancePublication => Set location from search string");
                }

                //Set fields
                //Title
                DoAction(driver, By.CssSelector($"input[id='item-edit__title']"), e => e.SendKeys(taskModel.Title));
                DoAction(driver, By.CssSelector($"textarea[id='item-edit__description']"), e => e.SendKeys(taskModel.Description));
                DoAction(driver, By.CssSelector($"input[id='item-edit__price']"), e => e.SendKeys(taskModel.Price));

                SendMessage($"InstancePublication => Set fields");

                if (taskModel.Images != null && taskModel.Images.Any())
                {
                    var count = 0;
                    foreach (var img in taskModel.Images)
                    {
                        try
                        {
                            count++;
                            var file = ImageHelper.ImageToTemp(img);
                            SendMessage($"InstancePublication => Image {img} save to {file}");
                            if (string.IsNullOrWhiteSpace(file))
                                continue;
                            var fileInput = driver.FindElementByCssSelector($"input[name='image']");
                            fileInput.SendKeys(file);
                            WaitElementCountByCssSelector(driver, 30, count, "div[class~='form-uploader-item'][data-state='active']");
                            SendMessage($"InstancePublication => Set image {img}");
                        }
                        catch (Exception ex)
                        {
                            var r4 = ex;
                        }
                    }
                }
                WaitExecute(driver);
                //Select base bulletin
                if (driver.PageSource.Contains("Разово"))
                {
                    SendMessage($"InstancePublication => Account {taskModel.Login} is blocked from publication limit");
                    Find(driver, "input", "name", "fees[eula]", c => JsClick(driver, c));
                }

                try
                {
                    WaitPage(driver, 10000, "Продолжить с пакетом «Обычная продажа»");
                    FindTagByTextContains(driver, "span", "Обычная продажа", e => JsClick(driver, e));
                }
                catch (Exception ex)
                {

                    var r5 = ex;
                    SendMessage($"InstancePublication => Error:{r5}");
                }
               
                WaitExecute(driver);
                SendMessage($"InstancePublication => Select sale type ");

                FindTagByTextContains(driver, "button", "Продолжить с пакетом «Обычная продажа»", e => JsClick(driver, e));
                WaitExecute(driver);
                SendMessage($"InstancePublication => Click continue");

                var servicePremium = Find(driver, By.Id("service-premium"));
                if(servicePremium != null && servicePremium.GetAttribute("checked") != null)
                    JsClick(driver, By.Id("service-premium"));
                var serviceVip = Find(driver, By.Id("service-vip"));
                if (serviceVip != null && serviceVip.GetAttribute("checked") != null)
                    JsClick(driver, By.Id("service-vip"));
                var serviceHighlight = Find(driver, By.Id("service-highlight"));
                if (serviceHighlight != null && serviceHighlight.GetAttribute("checked") != null)
                    JsClick(driver, By.Id("service-highlight"));

                SendMessage($"InstancePublication => Paid option has been disabled ");

                var isClassicDesign = false;
                var button = FindMany(driver, By.TagName("button")).FirstOrDefault(q => q.Text == "Подать объявление");
                if(button == null)
                {
                    isClassicDesign = true;
                    button = FindMany(driver, By.TagName("button")).FirstOrDefault(q => q.Text == "Продолжить");
                }
                    
                JsClick(driver, button);
                WaitExecute(driver);
                SendMessage($"InstancePublication => Click continue");

                //Забираем URL
                if(isClassicDesign)
                {
                    var a = Find(driver, By.XPath("//*[@class='content-text']/p/a"));
                    if(a != null)
                    {
                        var href = a.GetAttribute("href");
                        result = href;
                    }
                }
                else
                {
                    FindContains(driver, "a", "class", "item-added-popup-link", e =>
                    {
                        var href = e.GetAttribute("href");
                        result = href;

                    });
                }
                
                ////Забираем идентификатор
                //var idPattern = "itemId=(\\d+)";
                //var pageSource = driver.PageSource;
                //var id = RegexHelper.GetValue(idPattern, pageSource);

                ////Переходим на страницу профиля и забираем полный url по идентификатору
                //var inactivePage = @"www.avito.ru/profile/items/inactive";
                //driver.Navigate().GoToUrl("http://" + inactivePage + $"/rossiya?p=1&s=4");
                //WaitPage(driver, 3000, inactivePage);
                //WaitExecute(driver);
                //var nextPagePattern = "(Следующая страница)";
                //var urlPattern = $"a name=\"item_{id}\" href=\"(.*?)\"";
                //var pages = 1;
                //var hasNextPage = true;
                //while (hasNextPage)
                //{
                //    pages++;
                //    var html = driver.PageSource;
                //    hasNextPage = !string.IsNullOrEmpty(RegexHelper.GetValue(nextPagePattern, html));

                //    var match = RegexHelper.GetValue(urlPattern, html);
                //    if (!string.IsNullOrEmpty(match))
                //    {
                //        result = @"https://www.avito.ru" + match;

                //        SendMessage($"InstancePublication => Url is found - {result}");
                //        break;
                //    }
                //    if (hasNextPage)
                //    {
                //        driver.Navigate().GoToUrl("http://" + inactivePage + $"/rossiya?p={pages}&s=4");
                //        WaitPage(driver, 30000, inactivePage + $"/rossiya?p={pages}&s=4");
                //    }
                //}
                if (string.IsNullOrEmpty(result))
                    SendMessage($"InstancePublication => Url is not found");
                else
                    SendMessage($"InstancePublication => Url is found - {result}");
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
