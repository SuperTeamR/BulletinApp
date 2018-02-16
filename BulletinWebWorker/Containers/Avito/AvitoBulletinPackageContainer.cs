using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
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
                WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach (var bulletin in packages)
                    {
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            Thread.Sleep(2000);
                            //Стадия заполнения
                            WebWorker.NavigatePage("https://www.avito.ru/additem");
                            //1
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category1))
                            {
                                var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                               .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category1);
                                if (categoryRadio == null) return;
                                categoryRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //2
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category2))
                            {
                                var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category2);
                                if (serviceRadio == null) return;
                                serviceRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //3
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category3))
                            {
                                var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category3);
                                if (serviceTypeRadio == null) return;
                                serviceTypeRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //4
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category4))
                            {
                                var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category4);
                                if (serviceTypeRadio2 == null) return;
                                serviceTypeRadio2.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //5
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category5))
                            {
                                var serviceTypeRadio3 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category5);
                                if (serviceTypeRadio3 == null) return;
                                serviceTypeRadio3.InvokeMember("click");
                                Thread.Sleep(1000);
                            }

                            foreach (var pair in bulletin.ValueFields)
                            {
                                var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                                fieldValueContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                            }

                            //Продолжить с пакетом «Обычная продажа»
                            var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "pack1");
                            if (radioButton != null) radioButton.InvokeMember("click");


                            var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                            var pack = "Продолжить с пакетом «Обычная продажа»";
                            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                            if (button != null)
                                button.InvokeMember("click");

                            Thread.Sleep(1000);


                            //Стадия публикации
                            WebWorker.NavigatePage("https://www.avito.ru/additem/confirm");
                            //Снимаем галочки
                            var servicePremium = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-premium");
                            if (servicePremium != null)
                                servicePremium.InvokeMember("click");
                            var serviceVip = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-vip");
                            if (serviceVip != null)
                                serviceVip.InvokeMember("click");
                            var serviceHighlight = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-highlight");
                            if (serviceHighlight != null)
                                serviceHighlight.InvokeMember("click");

                            //Подтверждаем
                            var text = "Продолжить";
                            var buttonContinue = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                            if (buttonContinue != null)
                                buttonContinue.InvokeMember("click");
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
                WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        WebWorker.NavigatePage(bulletin.Url);

                        foreach (var pair in bulletin.ValueFields)
                        {
                            var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                            fieldValueContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                        }

                        if (bulletin.State == (int)BulletinState.Edited)
                        {
                            //Продолжить с пакетом «Обычная продажа»
                            var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "pack1");
                            if (radioButton != null) radioButton.InvokeMember("click");

                            var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                            var pack = "Продолжить без пакета";
                            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                            if (button != null)
                                button.InvokeMember("click");
                        }
                        else
                        {
                            var button = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "submit" && q.InnerText == "Продолжить");

                            if (button != null)
                                button.InvokeMember("click");
                        }
                    }
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
        public override IEnumerable<BulletinPackage> GetBulletins(AccessPackage access)
        {
            var result = Enumerable.Empty<BulletinPackage>();
            DCT.Execute(d =>
            {
                var tabStates = new List<TabState>();
                var bulletins = new List<BulletinPackage>();
                WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    if (accessContainer.TryAuth(access))
                    {
                        WebWorker.NavigatePage(ProfileUrl);

                        var tabs = WebWorker.WebDocument.GetElementsByTagName("li").Cast<HtmlElement>()
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

                                    tabStates.Add(new TabState
                                    {
                                        Title = tabState,
                                        Href = tabUrl,
                                    });
                                }
                            }
                        }
                    }
                });

                WebWorker.Execute(() =>
                {
                    foreach (var tabState in tabStates)
                    {
                        WebWorker.NavigatePage(tabState.Href);

                        var nextPages = new List<string>();
                        nextPages.Add(tabState.Href);

                        var hasNextPage = true;
                        do
                        {
                            var bulletinsOnPage = GetBulletinPages(tabState.Title);
                            bulletins.AddRange(bulletinsOnPage);

                            var nextPage = WebWorker.WebDocument.GetElementsByTagName("a").Cast<HtmlElement>()
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
                });
                WebWorker.Execute(() =>
                {
                    foreach (var bulletin in bulletins)
                    {
                        var url = Path.Combine(bulletin.Url, "edit");
                        WebWorker.NavigatePage(url);
                        Thread.Sleep(1500);

                        var groupElement = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("className").Contains("form-category-path"));

                        if (groupElement == null) continue;

                        var categories = groupElement.InnerText.Split('/').Select(q => q.Trim()).ToArray();
                        bulletin.Signature = new GroupSignature(categories);
                    }
                    result = bulletins;
                });
            });
            return result;
        }

        List<BulletinPackage> GetBulletinPages(string state)
        {
            var result = new List<BulletinPackage>();
            DCT.Execute(data =>
            {
                var s = WebWorker.WebDocument.Body.OuterHtml;
                var titleDivs = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                            .Where(q => q.GetAttribute("className").Contains("profile-item-description"));

                foreach (var d in titleDivs)
                {
                    var bulletin = new BulletinPackage
                    {
                        State = (int)GetStateFromString(state)
                    };
                    GelChildrenRecursively(d, bulletin);
                    result.Add(bulletin);
                }
            });
            return result;
        }


        BulletinState GetStateFromString(string state)
        {
            switch(state)
            {
                case "Активные":
                    return BulletinState.Publicated;
                case "Блокированные":
                    return BulletinState.Blocked;
                case "Отклоненные":
                    return BulletinState.Rejected;
                default:
                    return BulletinState.Error;
            }
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gel children recursively. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="element">  The element. </param>
        /// <param name="bulletin"> The bulletin. </param>
        ///-------------------------------------------------------------------------------------------------

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
    }
}
