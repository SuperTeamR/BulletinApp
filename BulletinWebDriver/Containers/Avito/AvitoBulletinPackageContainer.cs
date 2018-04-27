using BulletinBridge.Data;
using BulletinWebDriver;
using BulletinWebDriver.Helpers;
using BulletinWebDriver.Tools;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{
    // Нахождение прокси
    // Авторизация
    // Выбор категорий
    // Заполнение полей
    // Загрузка картинок
    // Подтверждение
    // Получение URL
    //----------------
    // Проверка состояния
    // Выгрузка буллетина
    class AvitoBulletinPackageContainer : BulletinPackageContainerBase
    {
        public override Guid Uid => BoardIds.Avito;
        public string ProfileUrl => @"https://www.avito.ru/profile";
        public override void AddBulletins(IEnumerable<TaskCache> tasks)
        {

            DCT.Execute(d =>
            {
                var accessContainer = AccessContainerList.Get(Uid);

                foreach (var task in tasks)
                {
                    var bulletin = task.BulletinPackage;
                    if (accessContainer.TryAuth(bulletin.Access))
                    {
                        PrepareBulletin(bulletin);
                        ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));
                        Publicate(bulletin);
                        GetUrl(bulletin);
                    }
                }
                DCT.ExecuteAsync(d2 =>
                {
                    foreach (var task in tasks)
                    {
                        var bulletin = task.BulletinPackage;
                        if (string.IsNullOrEmpty(bulletin.Url))
                        {
                            bulletin.State = (int)BulletinState.Error;
                            task.State = (int)TaskCacheState.Error;
                        }
                        else
                        {
                            bulletin.State = (int)BulletinState.OnModeration;
                            task.State = (int)TaskCacheState.Completed;
                        }
                    }
                    TaskHelper.Complete(tasks);
                });
            });
        }
        void PrepareBulletin(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                WebDriver.UpdateActions();
                if(bulletin.State == (int)BulletinState.Edited)
                {
                    WebDriver.NavigatePage(Path.Combine(bulletin.Url, "edit"));
                    Thread.Sleep(2000);
                }
                else
                {
                    WebDriver.NavigatePage("https://www.avito.ru/additem");
                    Thread.Sleep(2000);
                    ChooseCategories(bulletin.Signature);
                }
                if (!SetValueFields(bulletin))
                {
                    PrepareBulletin(bulletin);
                }
            }, continueExceptionMethod: (d, e) => 
            {
                PrepareBulletin(bulletin);
            });
        }
        void ChooseCategories(GroupSignature signature)
        {
            DCT.Execute(d =>
            {
               foreach(var category in signature.GetCategories())
               {
                    if (!string.IsNullOrEmpty(category))
                    {
                        WebDriver.JsClick(By.CssSelector($"input[title='{category}']"));
                        Thread.Sleep(1000);
                    }
                        
               }
            });
        }
        bool SetValueFields(BulletinPackage bulletin)
        {
            var result = false;
            DCT.Execute(d =>
            {
                var accessFiels = bulletin.AccessFields;
                var valueFields = bulletin.ValueFields;
                var bulletinTypeField = accessFiels["Вид объявления "];
                var bulletinTypeOption = bulletinTypeField.Options.FirstOrDefault(q => q.Text == "Продаю свое");

                var bulletinTypeCode = bulletinTypeOption.Value;
                var bulletinTitleCode = accessFiels["Название объявления"].HtmlId;
                var bulletinDescCode = accessFiels["Описание объявления"].HtmlId;
                var bulletinPriceCode = accessFiels["Цена"].HtmlId;
                var bulletinImageCode = accessFiels["Фотографии"].HtmlId;

                var bulletinTitleText = valueFields["Название объявления"];
                var bulletinDescText = valueFields["Описание объявления"];
                var bulletinPriceText = valueFields["Цена"];
                var bulletinImageText = valueFields.ContainsKey("Фотографии") ? valueFields["Фотографии"] : string.Empty;

                WebDriver.JsClick(By.CssSelector($"input[value='{bulletinTypeCode}']"));
                if (bulletin.State == (int)BulletinState.Edited)
                {
                    WebDriver.DoAction(By.CssSelector($"input[id='{bulletinTitleCode}']"), e => e.Clear());
                    WebDriver.DoAction(By.CssSelector($"textarea[id='{bulletinDescCode}']"), e => e.Clear());
                    WebDriver.DoAction(By.CssSelector($"input[id='{bulletinPriceCode}']"), e => e.Clear());
                }
                WebDriver.DoAction(By.CssSelector($"input[id='{bulletinTitleCode}']"), e => e.SendKeys(bulletinTitleText));
                WebDriver.DoAction(By.CssSelector($"textarea[id='{bulletinDescCode}']"), e => e.SendKeys(bulletinDescText));
                WebDriver.DoAction(By.CssSelector($"input[id='{bulletinPriceCode}']"), e => e.SendKeys(bulletinPriceText));

                var oldImages = WebDriver.FindMany(By.ClassName("form-uploader-item__delete")).ToArray();
                if(oldImages.Length > 0)
                {
                    foreach(var image in oldImages)
                    {
                        WebDriver.JsClick(image);
                    }
                }

                if(!string.IsNullOrEmpty(bulletinImageText))
                {
                    var images = bulletinImageText.Split(new[] { "\r\n" }, StringSplitOptions.None);

                    foreach (var image in images)
                    {
                        if (!WebDriver.Wait(WebDriver.NoAttribute(By.CssSelector($"input[name='{bulletinImageCode}']"), "disabled"), 20))
                            return;

                        WebDriver.JsClick(By.CssSelector($"input[name='{bulletinImageCode}']"),
                         (e) =>
                         {
                             Thread.Sleep(1000);
                             SendKeys.SendWait(image);
                         });
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(10000);

                        if (!WebDriver.Wait(WebDriver.ElementExists(By.CssSelector($"input[name='{bulletinImageCode}']")), 20))
                            return;
                    }

                    var addedImagesCount = WebDriver.FindMany(By.ClassName("form-uploader-item")).Where(q => q.GetAttribute("data-state") == "active").Count();

                    if (addedImagesCount != images.Length)
                        return;
                }

                result = true;
            });
            if(!result)
            {
                SendKeys.SendWait("{ESC}");
            }
            return result;
            
        }


        void ContinueAddOrEdit(BulletinState state)
        {
            DCT.Execute(d =>
            {
                if(state == BulletinState.Edited)
                {
                    var element = WebDriver.FindMany(By.ClassName("packages-tab-name")).FirstOrDefault(q => q.Text == "Без пакета");
                    element.Click();
                    var button = WebDriver.FindMany(By.ClassName("button-origin")).FirstOrDefault(q => q.Text == "Продолжить без пакета");
                    WebDriver.JsClick(button);
                 
                }
                else if (state == BulletinState.WaitPublication || state == BulletinState.WaitRepublication || state == BulletinState.Created)
                {
                    //WebDriver.DoClick(By.Id("pack1"));
                    var button = WebDriver.FindMany(By.ClassName("button-origin")).FirstOrDefault(q => q.Text == "Продолжить с пакетом «Обычная продажа»");
                    WebDriver.JsClick(button);
                }
                else
                {
                    var button = WebDriver.FindMany(By.ClassName("button-origin")).FirstOrDefault(q => q.Text == "Продолжить");
                    WebDriver.JsClick(button);
                }
            });
        }
        void Publicate(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                if(bulletin.State == (int)BulletinState.WaitPublication || bulletin.State == 0)
                {
                    WebDriver.NavigatePage("https://www.avito.ru/additem/confirm");
                }
                else if (bulletin.State == (int)BulletinState.Edited)
                {
                    WebDriver.NavigatePage(Path.Combine(bulletin.Url, "edit", "confirm"));
                }

                //Снимаем галочки
                WebDriver.JsClick(By.Id("service-premium"));
                WebDriver.JsClick(By.Id("service-vip"));
                WebDriver.JsClick(By.Id("service-highlight"));

                //Подтверждаем
                var button = WebDriver.FindMany(By.TagName("button")).FirstOrDefault(q => q.Text == "Продолжить");
                WebDriver.JsClick(button);
            });
        }
        void GetUrl(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                var a = WebDriver.Find(By.XPath("//*[@class='content-text']/p/a"));
                var href = a.GetAttribute("href");
                bulletin.Url = href;
            });
        }
        public override void CheckModerationState(IEnumerable<TaskCache> tasks)
        {
            DCT.Execute(d =>
            {

                var packages = tasks.Select(q => q.BulletinPackage);
                foreach (var b in packages)
                {
                    var state = CheckBulletinState(b.Url);
                    b.State = (int)state;
                }

                foreach(var t in tasks)
                {
                    t.State = (int)TaskCacheState.Completed;
                }

                TaskHelper.Complete(tasks);
            });
        }
        BulletinState CheckBulletinState(string url)
        {
            var result = BulletinState.Error;

            DCT.Execute(d =>
            {
                WebDriver.NavigatePage(ProfileUrl);

                var tabStates = new List<TabState>();
                var tabs = WebDriver.FindMany(By.ClassName("tabs-item"));
                tabStates.Add(new TabState
                {
                    Title = "Активные",
                    Href = ProfileUrl,
                });

                foreach(var tab in tabs)
                {
                    var a = tab.FindElement(By.XPath("/a"));
                    var tabUrl = a.GetAttribute("href");
                    var tabState = a.Text;

                    if (tabState == "Удаленные") continue;

                    tabStates.Add(new TabState
                    {
                        Title = tabState,
                        Href = tabUrl,
                    });
                }
                var foundedBulletin = default(BulletinPackage);
                foreach(var tabState in tabStates)
                {
                    var bulletinState = GetStateFromTabString(tabState.Title);
                    if (bulletinState == BulletinState.Blocked
                    || bulletinState == BulletinState.Removed) continue;

                    WebDriver.NavigatePage(tabState.Href);

                    var nextPages = new List<string>();
                    nextPages.Add(tabState.Href);

                    var hasNextPage = true;
                    do
                    {
                        var bulletinOnPage = GetBulletinPages(tabState.Title);

                        var hasBulletin = bulletinOnPage.FirstOrDefault(q => q.Url == url);
                        if(hasBulletin != null)
                        {
                            foundedBulletin = hasBulletin;
                        }
                        var nextPage = WebDriver.Find(By.ClassName("js-pagination-next"));
                        if (nextPage == null) hasNextPage = false;
                        else
                        {
                            var nextPageHref = nextPage.GetAttribute("href");
                            nextPages.Add(nextPageHref);
                            hasNextPage = true;
                            nextPage.Click();
                            Thread.Sleep(1000);
                        }

                    } while (hasNextPage);

                    if (foundedBulletin != null)
                        break;
                }

                if(foundedBulletin != null)
                {
                    WebDriver.NavigatePage(foundedBulletin.Url);
                    Thread.Sleep(1500);

                    var warningElement = WebDriver.Find(By.ClassName("item-view-warning"));
                    if(warningElement != null)
                    {
                        if(warningElement.Text.Contains("Сейчас это объявление проверяется модераторами"))
                        {
                            foundedBulletin.State = (int)BulletinState.OnModeration;
                        }
                        result = EnumHelper.GetValue<BulletinState>(foundedBulletin.State);
                    }
                }

            });
            return result;
        }
        BulletinState GetStateFromTabString(string state)
        {
            switch(state)
            {
                case "Активные":
                    return BulletinState.Publicated;
                case "Блокированные":
                    return BulletinState.Blocked;
                case "Отклонённые":
                    return BulletinState.Rejected;
                case "Удалённые":
                    return BulletinState.Removed;
                case "Завершённые":
                    return BulletinState.Closed;
                default:
                    return BulletinState.Error;
            }
        }
        List<BulletinPackage> GetBulletinPages(string state)
        {
            var result = new List<BulletinPackage>();
            DCT.Execute(data =>
            {
                
                var titles = WebDriver.FindMany(By.XPath("//*[@class='profile-item-description']"));

                foreach (var t in titles)
                {
                    var a = t.FindElement(By.XPath("div/h3/a"));
                    var url = a.GetAttribute("href");
                    var name = a.Text;
                    var bulletin = new BulletinPackage
                    {
                        Url = url,
                        Title = name,
                        State = (int)GetStateFromTabString(state)
                    };
                    result.Add(bulletin);
                }
            });
            return result;
        }
        public override void GetBulletinList(IEnumerable<TaskCache> tasks)
        {
            DCT.Execute(d =>
            {
                var bulletins = new List<BulletinPackage>();

                foreach(var task in tasks)
                {
                    var access = task.AccessPackage;
                    var r = GetBulletinList(access);
                    bulletins.AddRange(r);
                    task.State = (int)TaskCacheState.Completed;
                }
                TaskHelper.Complete(tasks);
            });
        }
        IEnumerable<BulletinPackage> GetBulletinList(AccessPackage access)
        {
            var result = Enumerable.Empty<BulletinPackage>();
            DCT.Execute(d =>
            {
                var tabStates = new List<TabState>();
                var bulletins = new List<BulletinPackage>();

                var accessContainer = AccessContainerList.Get(Uid);

                if (accessContainer.TryAuth(access))
                {
                    WebDriver.NavigatePage(ProfileUrl);

                    var tabs = WebDriver.FindMany(By.ClassName("tabs-item"));

                    tabStates.Add(new TabState
                    {
                        Title = "Активные",
                        Href = ProfileUrl
                    });

                    foreach (var tab in tabs)
                    {
                        var a = tab.FindElement(By.XPath("/a"));
                        var tabUrl = a.GetAttribute("href");
                        var tabState = a.Text;

                        if (tabState == "Удаленные") continue;

                        tabStates.Add(new TabState
                        {
                            Title = tabState,
                            Href = tabUrl,
                        });
                    }

                    foreach(var tabState in tabStates)
                    {
                        var bulletinState = GetStateFromTabString(tabState.Title);
                        if (bulletinState == BulletinState.Blocked
                        || bulletinState == BulletinState.Removed) continue;

                        WebDriver.NavigatePage(tabState.Href);

                        var nextPages = new List<string>();
                        nextPages.Add(tabState.Href);

                        var hasNextPage = true;
                        do
                        {

                            var bulletinOnPage = GetBulletinPages(tabState.Title);
                            bulletins.AddRange(bulletinOnPage);

                            var nextPage = WebDriver.Find(By.ClassName("js-pagination-next"));
                            if (nextPage == null) hasNextPage = false;
                            else
                            {
                                var nextPageHref = nextPage.GetAttribute("href");
                                nextPages.Add(nextPageHref);
                                hasNextPage = true;
                                nextPage.Click();
                                Thread.Sleep(1000);
                            }
                        } while (hasNextPage);
                    }

                    foreach(var bulletin in bulletins)
                    {
                        var url = Path.Combine(bulletin.Url, "edit");
                        WebDriver.NavigatePage(url);
                        Thread.Sleep(1500);

                        var groupElement = WebDriver.Find(By.ClassName("form-category-path"));

                        if (groupElement == null) continue;

                        var categories = groupElement.Text.Split('/').Select(q => q.Trim()).ToArray();
                        bulletin.Signature = new GroupSignature(categories);
                        bulletin.Access = access;
                    }
                    result = bulletins;
                }
            });
            return result;
        }

        public override void GetBulletinDetails(IEnumerable<TaskCache> tasks)
        {
            DCT.Execute(d =>
            {
                var accessContainer = AccessContainerList.Get(Uid);

                var packages = tasks.Select(q => q.BulletinPackage);

                var accessCollection = packages.Where(q => q.Access != null).GroupBy(q => q.Access.Login).Select(q => new
                {
                    Access = q.Key,
                    Collection = q.ToList()
                }).ToList();

                foreach(var a in accessCollection)
                {

                    var bulletins = a.Collection;
                    foreach(var bulletin in bulletins)
                    {
                        Thread.Sleep(2000);

                        var url = Path.Combine(bulletin.Url, "edit");
                        WebDriver.NavigatePage(url);
                        Thread.Sleep(1500);

                        var values = new Dictionary<string, string>();
                        foreach(var pair in bulletin.AccessFields)
                        {
                        }
                    }
                }

            });
        }

        public override void EditBulletins(IEnumerable<TaskCache> tasks)
        {
            DCT.Execute(d =>
            {
                var accessContainer = AccessContainerList.Get(Uid);

                foreach (var task in tasks)
                {
                    var bulletin = task.BulletinPackage;
                    if (accessContainer.TryAuth(bulletin.Access))
                    {

                        PrepareBulletin(bulletin);
                        ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));
                        Publicate(bulletin);
                    }
                }
                DCT.ExecuteAsync(d2 =>
                {
                    foreach (var task in tasks)
                    {
                        var bulletin = task.BulletinPackage;
                        if (string.IsNullOrEmpty(bulletin.Url))
                        {
                            bulletin.State = (int)BulletinState.Error;
                            task.State = (int)TaskCacheState.Error;
                        }
                        else
                        {
                            bulletin.State = (int)BulletinState.OnModeration;
                            task.State = (int)TaskCacheState.Completed;
                        }
                    }
                    TaskHelper.Complete(tasks);
                });
            });
        }
    }
}