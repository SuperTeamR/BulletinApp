using BulletinBridge.Data;
using BulletinBridge.Messages.InternalApi;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Service;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{

    class AvitoBulletinPackageContainer : BulletinPackageContainerBase
    {
        public override Guid Uid => BoardIds.Avito;

        public string ProfileUrl => @"https://www.avito.ru/profile";


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------
        public override void AddBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach (var bulletin in packages)
                    {
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            Thread.Sleep(2000);

                            Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");
 
                            ChooseCategories(bulletin.Signature);

                            SetValueFields(bulletin, fieldValueContainer);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                            Thread.Sleep(1000);

                            Publicate(bulletin);

                            Thread.Sleep(1000);

                            GetUrl(bulletin);
                        }
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    DCT.ExecuteAsync(d2 =>
                    {
                        foreach (var b in packages)
                        {
                            b.State = (int)BulletinState.OnModeration;
                        }
                        SendResultRouter.BulletinWorkResult(packages);
                    });
                });
            });
        }


        public void TestImage()
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var accessContainer = AccessContainerList.Get(Uid);

                    var access = new AccessPackage
                    {
                        Login = "mostrerkilltest@gmail.com",
                        Password = "OnlineHelp59",
                    };

                    if (accessContainer.TryAuth(access))
                    {
                        Thread.Sleep(2000);

                        Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");

                        ChooseCategories(new GroupSignature("Хобби и отдых", "Охота и рыбалка"));

                        var form = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                               .FirstOrDefault(q => q.GetAttribute("name") == "image");
                        if(form != null)
                        {
                            form.Focus();
                            var sendKeyTask = Task.Delay(500).ContinueWith((_) =>
                            {
                                SendKeys.SendWait("https://upload.wikimedia.org/wikipedia/commons/4/47/PNG_transparency_demonstration_1.png");
                                SendKeys.SendWait("{Enter}");
                            });
                            form.InvokeMember("click");
                            Thread.Sleep(6000);
                        }
                    }
                });
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Отредактировать буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------
        public override void EditBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit"));

                        SetValueFields(bulletin, fieldValueContainer);

                        ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                        Thread.Sleep(1000);

                        Publicate(bulletin);
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    foreach (var b in packages)
                    {
                        b.State = (int)BulletinState.OnModeration;
                    }
                    SendResultRouter.BulletinWorkResult(packages);
                });
            });
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получает коллекцию буллетинов для заданного доступа </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="access">   Доступ к борде </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the bulletins in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public override void GetBulletinList(IEnumerable<AccessPackage> accesses)
        {
            var result = Enumerable.Empty<BulletinPackage>();
            DCT.Execute(d =>
            {
                var bulletins = new List<BulletinPackage>();

                Tools.WebWorker.Execute(() =>
                {
                    foreach(var access in accesses)
                    {
                        var r = GetBulletinList(access);
                        bulletins.AddRange(r);
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    SendResultRouter.AccessWorkResult(accesses);
                    SendResultRouter.BulletinWorkResult(bulletins);
                });

            });
        }

        IEnumerable<BulletinPackage> GetBulletinList(AccessPackage access)
        {
            var result = Enumerable.Empty<BulletinPackage>();

            DCT.Execute(d =>
            {
                var tabStates = new List<TabState>();
                var bulletins = new List<BulletinPackage>();

                var fieldValueContainer = FieldValueContainerList.Get(Uid);
                var accessContainer = AccessContainerList.Get(Uid);

                if (accessContainer.TryAuth(access))
                {
                    Thread.Sleep(2000);

                    Tools.WebWorker.NavigatePage(ProfileUrl);

                    var doc = Tools.WebWorker.WebDocument.Body.InnerHtml;
                    var tabs = Tools.WebWorker.WebDocument.GetElementsByTagName("li").Cast<HtmlElement>()
                        .Where(q => q.GetAttribute("className").Contains("tabs-item")).ToArray();

                    tabStates.Add(new TabState
                    {
                        Title = "Активные",
                        Href = ProfileUrl,
                    });

                    foreach (var tab in tabs)
                    {
                        if (!tab.CanHaveChildren) continue;

                        foreach (HtmlElement ch in tab.Children)
                        {
                            if (ch.TagName.ToLower() == "a")
                            {
                                var tabUrl = ch.GetAttribute("href");
                                var tabState = ch.InnerText;

                                if (tabState == "Удаленные") continue;

                                tabStates.Add(new TabState
                                {
                                    Title = tabState,
                                    Href = tabUrl,
                                });
                            }
                        }
                    }
                }
                foreach (var tabState in tabStates)
                {
                    var bulletinState = GetStateFromTabString(tabState.Title);
                    if (bulletinState == BulletinState.Blocked
                    || bulletinState == BulletinState.Removed) continue;

                    Tools.WebWorker.NavigatePage(tabState.Href);

                    var nextPages = new List<string>();
                    nextPages.Add(tabState.Href);

                    var hasNextPage = true;
                    do
                    {
                        var bulletinsOnPage = GetBulletinPages(tabState.Title);
                        bulletins.AddRange(bulletinsOnPage);

                        var nextPage = Tools.WebWorker.WebDocument.GetElementsByTagName("a").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("className").Contains("js-pagination-next"));
                        if (nextPage == null) hasNextPage = false;
                        else
                        {
                            var nextPageHref = nextPage.GetAttribute("href");
                            nextPages.Add(nextPageHref);
                            hasNextPage = true;
                            nextPage.InvokeMember("click");
                            Thread.Sleep(1000);
                        }
                    } while (hasNextPage);
                }

                foreach (var bulletin in bulletins)
                {
                    var url = Path.Combine(bulletin.Url, "edit");
                    Tools.WebWorker.NavigatePage(url);
                    Thread.Sleep(1500);

                    var groupElement = Tools.WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("className") != null && q.GetAttribute("className").Contains("form-category-path"));

                    if (groupElement == null) continue;

                    var categories = groupElement.InnerText.Split('/').Select(q => q.Trim()).ToArray();
                    bulletin.Signature = new GroupSignature(categories);
                    bulletin.Access = access;
                }
                result = bulletins;
            });
            return result;
        }
        public override void GetBulletinDetails(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    var accessCollection = packages.Cast<BulletinPackage>().Where(q => q.Access != null).GroupBy(q => q.Access.Login).Select(q => new { Access = q.Key, Collection = q.ToList() }).ToList();
                    foreach (var a in accessCollection)
                    {
                        var bulletins = a.Collection;
                        foreach (var bulletin in bulletins)
                        {
                            if (accessContainer.TryAuth(bulletin.Access))
                            {
                                Thread.Sleep(2000);

                                var url = Path.Combine(bulletin.Url, "edit");
                                Tools.WebWorker.NavigatePage(url);
                                Thread.Sleep(1500);
                                var values = new Dictionary<string, string>();
                                foreach (var pair in bulletin.AccessFields)
                                {
                                    var v = fieldValueContainer.GetFieldValue(bulletin.AccessFields, pair.Key);
                                    values.Add(pair.Key, v);
                                }
                                bulletin.ValueFields = values;
                                bulletin.State = (int)CheckBulletinState(bulletin.Url);
                            }
                        }
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    SendResultRouter.BulletinWorkResult(packages);
                });
            });
        }
        public override void CheckModerationState(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    foreach(var b in packages)
                    {
                        var state = CheckBulletinState(b.Url);
                        b.State = (int)state;
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    SendResultRouter.BulletinWorkResult(packages);
                });
            });
        }




        void ChooseCategories(GroupSignature signature)
        {
            DCT.Execute(d =>
            {
                if (!string.IsNullOrEmpty(signature.Category1))
                {
                    var categoryRadio = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                   .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category1);
                    if (categoryRadio == null) return;
                    categoryRadio.InvokeMember("click");
                    Thread.Sleep(1000);
                }
                //2
                if (!string.IsNullOrEmpty(signature.Category2))
                {
                    var serviceRadio = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category2);
                    if (serviceRadio == null) return;
                    serviceRadio.InvokeMember("click");
                    Thread.Sleep(1000);
                }
                //3
                if (!string.IsNullOrEmpty(signature.Category3))
                {
                    var serviceTypeRadio = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category3);
                    if (serviceTypeRadio == null) return;
                    serviceTypeRadio.InvokeMember("click");
                    Thread.Sleep(1000);
                }
                //4
                if (!string.IsNullOrEmpty(signature.Category4))
                {
                    var serviceTypeRadio2 = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category4);
                    if (serviceTypeRadio2 == null) return;
                    serviceTypeRadio2.InvokeMember("click");
                    Thread.Sleep(1000);
                }
                //5
                if (!string.IsNullOrEmpty(signature.Category5))
                {
                    var serviceTypeRadio3 = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category5);
                    if (serviceTypeRadio3 == null) return;
                    serviceTypeRadio3.InvokeMember("click");
                    Thread.Sleep(1000);
                }
            });
        }

        void SetValueFields(BulletinPackage bulletin, FieldValueContainerBase fieldContainer)
        {
            DCT.Execute(d =>
            {
                foreach (var pair in bulletin.ValueFields)
                {
                    var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                    fieldContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                }
            });
            
        }

        void ContinueAddOrEdit(BulletinState state)
        {
            DCT.Execute(d =>
            {
                if(state == BulletinState.Edited)
                {
                    //Продолжить с пакетом «Обычная продажа»
                    var radioButton = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("id") == "pack3");
                    if (radioButton != null) radioButton.InvokeMember("click");

                    var buttons = Tools.WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var pack = "Продолжить без пакета";
                    var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                    if (button != null)
                        button.InvokeMember("click");
                }
                else if (state == BulletinState.WaitPublication)
                {
                    var radioButton = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                              .FirstOrDefault(q => q.GetAttribute("id") == "pack1");
                    if (radioButton != null) radioButton.InvokeMember("click");


                    var buttons = Tools.WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var pack = "Продолжить с пакетом «Обычная продажа»";
                    var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                    if (button != null)
                        button.InvokeMember("click");
                }
                else
                {
                    var button = Tools.WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                              .FirstOrDefault(q => q.GetAttribute("type") == "submit" && q.InnerText == "Продолжить");

                    if (button != null)
                        button.InvokeMember("click");
                }
               

            });
        }

        void Publicate(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                if(bulletin.State == (int)BulletinState.WaitPublication)
                {
                    //Стадия публикации
                    Tools.WebWorker.NavigatePage("https://www.avito.ru/additem/confirm");
                }
                else if (bulletin.State == (int)BulletinState.Edited)
                {
                    Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit", "confirm"));
                }
                
                //Снимаем галочки
                var servicePremium = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("id") == "service-premium");
                if (servicePremium != null)
                    servicePremium.InvokeMember("click");
                var serviceVip = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("id") == "service-vip");
                if (serviceVip != null)
                    serviceVip.InvokeMember("click");
                var serviceHighlight = Tools.WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .FirstOrDefault(q => q.GetAttribute("id") == "service-highlight");
                if (serviceHighlight != null)
                    serviceHighlight.InvokeMember("click");

                //Подтверждаем
                var text = "Продолжить";
                var buttonContinue = Tools.WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                if (buttonContinue != null)
                    buttonContinue.InvokeMember("click");

            });
        }

        void GetUrl(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                var divContent = GetByTag("div", e => e.GetAttribute("className").Contains("content-text"));
                var a = GetChildElement(divContent, e => e.TagName == "A");
                var href = a.GetAttribute("href");
                bulletin.Url = href;
            });
        }

        BulletinState CheckBulletinState(string url)
        {
            var result = BulletinState.Error;
            DCT.Execute(d =>
            {
                var tabStates = new List<TabState>();
                Tools.WebWorker.NavigatePage(ProfileUrl);

                var doc = Tools.WebWorker.WebDocument.Body.InnerHtml;
                var tabs = Tools.WebWorker.WebDocument.GetElementsByTagName("li").Cast<HtmlElement>()
                    .Where(q => q.GetAttribute("className").Contains("tabs-item")).ToArray();

                tabStates.Add(new TabState
                {
                    Title = "Активные",
                    Href = ProfileUrl,
                });

                foreach (var tab in tabs)
                {
                    if (!tab.CanHaveChildren) continue;

                    foreach (HtmlElement ch in tab.Children)
                    {
                        if (ch.TagName.ToLower() == "a")
                        {
                            var tabUrl = ch.GetAttribute("href");
                            var tabState = ch.InnerText;

                            if (tabState == "Удаленные") continue;

                            tabStates.Add(new TabState
                            {
                                Title = tabState,
                                Href = tabUrl,
                            });
                        }
                    }
                }
                BulletinPackage foundedBulletin = null;
                foreach (var tabState in tabStates)
                {
                    var bulletinState = GetStateFromTabString(tabState.Title);
                    if (bulletinState == BulletinState.Blocked
                    || bulletinState == BulletinState.Removed) continue;

                    Tools.WebWorker.NavigatePage(tabState.Href);

                    var nextPages = new List<string>();
                    nextPages.Add(tabState.Href);

                    var hasNextPage = true;
                    do
                    {
                        var bulletinsOnPage = GetBulletinPages(tabState.Title);

                        var hasBulletin = bulletinsOnPage.FirstOrDefault(q => q.Url == url);
                        if(hasBulletin != null)
                        {
                            foundedBulletin = hasBulletin;
                            break;
                        }
                        var nextPage = Tools.WebWorker.WebDocument.GetElementsByTagName("a").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("className").Contains("js-pagination-next"));
                        if (nextPage == null) hasNextPage = false;
                        else
                        {
                            var nextPageHref = nextPage.GetAttribute("href");
                            nextPages.Add(nextPageHref);
                            hasNextPage = true;
                            nextPage.InvokeMember("click");
                            Thread.Sleep(1000);
                        }
                    } while (hasNextPage);

                    if (foundedBulletin != null)
                        break;
                }
                if(foundedBulletin != null)
                {
                    Tools.WebWorker.NavigatePage(foundedBulletin.Url);
                    Thread.Sleep(1500);

                    var warningElement = Tools.WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("className") != null && q.GetAttribute("className").Contains("item-view-warning"));
                    if (warningElement != null)
                    {
                        if (warningElement.InnerText.Contains("Сейчас это объявление проверяется модераторами"))
                        {
                            foundedBulletin.State = (int)BulletinState.OnModeration;
                        }
                    }
                    result = EnumHelper.GetValue<BulletinState>(foundedBulletin.State);
                }
            });

            return result;
        }

        List<BulletinPackage> GetBulletinPages(string state)
        {
            var result = new List<BulletinPackage>();
            DCT.Execute(data =>
            {
                var s = Tools.WebWorker.WebDocument.Body.OuterHtml;
                var titleDivs = Tools.WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                            .Where(q => q.GetAttribute("className").Contains("profile-item-description"));

                foreach (var d in titleDivs)
                {
                    var bulletin = new BulletinPackage
                    {
                        State = (int)GetStateFromTabString(state)
                    };
                    GelChildrenRecursively(d, bulletin);
                    result.Add(bulletin);
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

        void GelChildrenRecursively(HtmlElement element, BulletinPackage bulletin)
        {
            DCT.Execute(data =>
            {
                if (!element.CanHaveChildren && element.Children.Count == 0) return;

                foreach (HtmlElement ch in element.Children)
                {
                    if (ch.TagName.ToLower() == "span" && ch.GetAttribute("className").Contains("profile-item-views-count"))
                    {
                        if (!string.IsNullOrEmpty(bulletin.Views)) continue;

                        var regex = new Regex(@"(?<count>\d+)");

                        var match = regex.Match(ch.InnerText);
                        if (match.Success)
                        {
                            bulletin.Views = match.Groups["count"].Value;
                        }

                    }
                    if (ch.TagName.ToLower() == "a" && ch.GetAttribute("name").Contains("item_"))
                    {
                        bulletin.Url = ch.GetAttribute("href");
                        bulletin.Title = ch.InnerText;
                    }
                    GelChildrenRecursively(ch, bulletin);
                }
            });
        }

        HtmlElement GetByTag(string tag, Func<HtmlElement, bool> func)
        {
            HtmlElement result = null;

            DCT.Execute(d =>
            {
                result = Tools.WebWorker.WebDocument.GetElementsByTagName(tag).Cast<HtmlElement>().FirstOrDefault(func);
            });
            return result;
        }


        HtmlElement GetChildElement(HtmlElement element, Func<HtmlElement, bool> func)
        {
            HtmlElement result = null;
            DCT.Execute(d =>
            {
                if (!element.CanHaveChildren && element.Children.Count == 0) return;

                foreach (HtmlElement ch in element.Children)
                {
                    if(func(ch))
                    {
                        result = ch;
                        return;
                    }
                    result = GetChildElement(ch, func);
                }
            });
            return result;
        }


    }
}
