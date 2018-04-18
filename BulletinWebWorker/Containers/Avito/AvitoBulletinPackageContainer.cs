using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Helpers;
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

        public override void AddBulletins2(IEnumerable<TaskCache> tasks)
        {
            UiHelper.UpdateWorkState("Добавление списка буллетинов");
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach (var task in tasks)
                    {
                        var bulletin = task.BulletinPackage;
                        var name = bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                            Thread.Sleep(2000);

                            UiHelper.UpdateActionState("Переход на страницу - additem");
                            Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");

                            UiHelper.UpdateActionState("Выбор категорий");
                            ChooseCategories(bulletin.Signature);

                            UiHelper.UpdateActionState("Установка значений");
                            SetValueFields(bulletin, fieldValueContainer);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                            Thread.Sleep(1000);

                            Publicate(bulletin);
                            //
                            Thread.Sleep(10000);

                            GetUrl(bulletin);
                        }
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);

                    DCT.ExecuteAsync(d2 =>
                    {
                        foreach (var task in tasks)
                        {
                            var bulletin = task.BulletinPackage;
                            if (string.IsNullOrEmpty(bulletin.Url))
                            {
                                UiHelper.UpdateActionState("URL is NULL");
                                bulletin.State = (int)BulletinState.Error;
                                task.State = (int)TaskCacheState.Error;
                            }
                            else
                            {
                                UiHelper.UpdateActionState("URL найден");
                                bulletin.State = (int)BulletinState.OnModeration;
                                task.State = (int)TaskCacheState.Completed;
                            }
                            var name = bulletin.ValueFields["Название объявления"];
                            UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");
                            Thread.Sleep(1000);
                        }
                        UiHelper.UpdateActionState("Отправка коллбека");


                        SendResultRouter.TaskWorkDone(tasks);
                    });
                });
            });
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------
        public override void AddBulletins(IEnumerable<BulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Добавление списка буллетинов");
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach (var bulletin in packages)
                    {
                        var name = bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                            Thread.Sleep(2000);

                            UiHelper.UpdateActionState("Переход на страницу - additem");
                            Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");

                            UiHelper.UpdateActionState("Выбор категорий");
                            ChooseCategories(bulletin.Signature);

                            UiHelper.UpdateActionState("Установка значений");
                            SetValueFields(bulletin, fieldValueContainer);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                            Thread.Sleep(1000);

                            Publicate(bulletin);
                            //
                            Thread.Sleep(10000);

                            GetUrl(bulletin);
                        }
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);

                    DCT.ExecuteAsync(d2 =>
                    {
                        foreach (var b in packages)
                        {
                            if (string.IsNullOrEmpty(b.Url))
                            {
                                UiHelper.UpdateActionState("URL is NULL");
                                b.State = (int)BulletinState.Error;
                            }
                            else
                            {
                                UiHelper.UpdateActionState("URL найден");
                                b.State = (int)BulletinState.OnModeration;
                            }
                            var name = b.ValueFields["Название объявления"];
                            UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");
                            Thread.Sleep(1000);
                        }
                        UiHelper.UpdateActionState("Отправка коллбека");


                        SendResultRouter.BulletinWorkResult(packages);
                    });
                });
            });
        }

        /// <summary>
        /// Перепубликация буллетинов
        /// </summary>
        /// <param name="packages"></param>
        public override void RepublicateBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach(var bulletin in packages)
                    {
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            Thread.Sleep(2000);

                            //Edit a bulletin
                            string cachedName, cachedDesc;
                            var r = new Random();

                        
                            cachedName = ChangeField(bulletin, "Название объявления", r.Next(1000, 100000).ToString());
                            cachedDesc = ChangeField(bulletin, "Описание объявления", r.Next(1000, 100000).ToString());
                            //Костыль - притворяемся, что состояние буллетина Edit
                            var cachedState = bulletin.State;
                            bulletin.State = (int)BulletinState.Edited;

                            Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit"));
                            SetValueFields(bulletin, fieldValueContainer);
                           
                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                            Thread.Sleep(1000);

                            Publicate(bulletin);

                            Thread.Sleep(1000);

                            //Remove a bulletin
                            Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "do"));
                            Thread.Sleep(1000);
                            RemoveBulletin();

                            //Re-add a bulletin
                            //Возвращаем прежнее состояние
                            bulletin.State = cachedState;
                            ChangeField(bulletin, "Название объявления", cachedName);
                            ChangeField(bulletin, "Описание объявления", cachedDesc);

                            Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");
                            Thread.Sleep(1000);

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
                            if (string.IsNullOrEmpty(b.Url))
                            {
                                b.State = (int)BulletinState.Error;
                            }
                            else
                            {
                                b.State = (int)BulletinState.OnModeration;
                            }
                        }
                        SendResultRouter.BulletinWorkResult(packages);
                    });
                });
            });
        }


        string ChangeField(BulletinPackage bulletin, string fieldKey, string newValue)
        {
            var cached = string.Empty;
            if (bulletin.ValueFields.ContainsKey(fieldKey))
            {
                cached = bulletin.ValueFields[fieldKey];
                bulletin.ValueFields[fieldKey] = newValue;
            }
            return cached;
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
            UiHelper.UpdateWorkState("Редактирование буллетинов");
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        var name = bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                            Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit"));

                            UiHelper.UpdateActionState("Установка значений");
                            SetValueFields(bulletin, fieldValueContainer);

                            ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

                            Thread.Sleep(1000);

                            Publicate(bulletin);
                        }
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    foreach (var b in packages)
                    {
                        b.State = (int)BulletinState.OnModeration;
                        var name = b.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");
                    }
                    UiHelper.UpdateActionState("Отправка коллбека");
                    SendResultRouter.BulletinWorkResult(packages);
                });
            });
        }


        public override void CloneBulletins(IEnumerable<AggregateBulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Клонирование буллетинов");
            DCT.Execute(d =>
            {
                var createdBulletins = new List<BulletinPackage>();

                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var package in packages)
                    {
                        var name = package.Bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {package.Bulletin.State}");

                        UiHelper.UpdateActionState("Попытка авторизоваться");
                        var accesses = package.Accesses.ToArray();
                        foreach (var access in accesses)
                        {
                            if (accessContainer.TryAuth(access))
                            {
                                UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                                Thread.Sleep(2000);

                                UiHelper.UpdateActionState("Переход на страницу - additem");
                                Tools.WebWorker.NavigatePage("https://www.avito.ru/additem");

                                UiHelper.UpdateActionState("Выбор категорий");
                                ChooseCategories(package.Bulletin.Signature);

                                UiHelper.UpdateActionState("Установка значений");
                                SetValueFields(package.Bulletin, fieldValueContainer);

                                ContinueAddOrEdit(BulletinState.WaitPublication);

                                Thread.Sleep(1000);

                                Publicate(package.Bulletin);
                                //
                                Thread.Sleep(20000);

                                GetUrl(package.Bulletin);


                                var newBulletin = new BulletinPackage
                                {
                                    Access = access,
                                    BulletinId = package.Bulletin.BulletinId,
                                    Url = package.Bulletin.Url,
                                    Title = package.Bulletin.Title,
                                    State = package.Bulletin.State,
                                    Signature = package.Bulletin.Signature,
                                    ValueFields = package.Bulletin.ValueFields
                                };
                                createdBulletins.Add(newBulletin);
                            }
                        }
                    }

                  
                });
                Tools.WebWorker.Execute(() =>
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);
                    foreach (var p in packages)
                    {
                        if (string.IsNullOrEmpty(p.Bulletin.Url))
                        {
                            UiHelper.UpdateActionState("URL is NULL");
                            p.Bulletin.State = (int)BulletinState.Error;
                        }
                        else
                        {
                            UiHelper.UpdateActionState("URL найден");
                        }
                        var name = p.Bulletin.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {p.Bulletin.State}");
                        Thread.Sleep(1000);
                    }
                    UiHelper.UpdateActionState("Отправка коллбека");
                    SendResultRouter.BulletinWorkResult(createdBulletins);
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
            UiHelper.UpdateWorkState("Выгрузка списка буллетинов");

            var result = Enumerable.Empty<BulletinPackage>();
            DCT.Execute(d =>
            {
                var bulletins = new List<BulletinPackage>();

                Tools.WebWorker.Execute(() =>
                {
                    foreach(var access in accesses)
                    {
                        UiHelper.UpdateObjectState($"Access {access.Login}");
                        var r = GetBulletinList(access);
                        bulletins.AddRange(r);
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    UiHelper.UpdateActionState("Отправка коллбека");

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

                UiHelper.UpdateActionState("Попытка авторизоваться");
                if (accessContainer.TryAuth(access))
                {
                    UiHelper.UpdateActionState("Ожидание прогрузки страницы");
                    Thread.Sleep(2000);

                    UiHelper.UpdateActionState("Переход на страницу профиля");
                    Tools.WebWorker.NavigatePage(ProfileUrl);

                    UiHelper.UpdateActionState("Считывание списка буллетинов");
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
            UiHelper.UpdateWorkState("Выгрузка полей для списка буллетинов");

            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    var accessCollection = packages.Cast<BulletinPackage>().Where(q => q.Access != null).GroupBy(q => q.Access.Login).Select(q => new { Access = q.Key, Collection = q.ToList() }).ToList();
                    foreach (var a in accessCollection)
                    {
                        UiHelper.UpdateObjectState($"Access {a.Access}");

                        var bulletins = a.Collection;
                        foreach (var bulletin in bulletins)
                        {
                            UiHelper.UpdateActionState("Попытка авторизоваться");
                            Thread.Sleep(2000);

                            if (accessContainer.TryAuth(bulletin.Access))
                            {
                                Thread.Sleep(2000);

                                var url = Path.Combine(bulletin.Url, "edit");
                                UiHelper.UpdateActionState($"Переход на страницу {url}");
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
                    UiHelper.UpdateActionState("Отправка коллбека");
                    SendResultRouter.BulletinWorkResult(packages);
                });
            });
        }
        public override void CheckModerationState(IEnumerable<BulletinPackage> packages)
        {
            UiHelper.UpdateWorkState("Проверка статуса модерации");
            DCT.Execute(d =>
            {
                Tools.WebWorker.Execute(() =>
                {
                    foreach(var b in packages)
                    {
                        var name = b.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {b.State}");

                        var state = CheckBulletinState(b.Url);
                        b.State = (int)state;
                    }
                });
                Tools.WebWorker.Execute(() =>
                {
                    UiHelper.UpdateActionState("Проверка Url и установка состояний");
                    Thread.Sleep(1000);
                    foreach (var p in packages)
                    {
                        var name = p.ValueFields["Название объявления"];
                        UiHelper.UpdateObjectState($"Bulletin {name}, state = {p.State}");
                        Thread.Sleep(1000);
                    }

                    UiHelper.UpdateActionState("Отправка коллбека");
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
                    if (string.IsNullOrEmpty(pair.Value)) continue;
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
                    UiHelper.UpdateActionState("Выбор \"Продолжить без пакет\"");

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
                else if (state == BulletinState.WaitPublication || state == BulletinState.WaitRepublication)
                {
                    UiHelper.UpdateActionState("Выбор \"Продолжить с пакетом «Обычная продажа»\"");

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
                    UiHelper.UpdateActionState("Выбор \"Продолжить\"");

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
                    UiHelper.UpdateActionState("Переход на страницу - additem/confirm");

                    //Стадия публикации
                    Tools.WebWorker.NavigatePage("https://www.avito.ru/additem/confirm");
                }
                else if (bulletin.State == (int)BulletinState.Edited)
                {
                    UiHelper.UpdateActionState($"Переход на страницу - {Path.Combine(bulletin.Url, "edit", "confirm")}");

                    Tools.WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit", "confirm"));
                }

                UiHelper.UpdateActionState("Снятие премиум-галочек");
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


                UiHelper.UpdateActionState("Подтверждение публикации");
                //Подтверждаем
                var text = "Продолжить";
                var buttonContinue = Tools.WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                if (buttonContinue != null)
                    buttonContinue.InvokeMember("click");

            });
        }

        bool RemoveBulletin()
        {
            var result = false;
            DCT.Execute(d =>
            {
                var closeButton = GetByTag("input", e => e.GetAttribute("className").Contains("js-close-item"));
                if(closeButton != null)
                {
                    closeButton.InvokeMember("click");

                    var continueButton = GetByTag("button", e => e.InnerText.Contains("Далее"));
                    continueButton.InvokeMember("click");
                    Thread.Sleep(1000);

                    //Снятие с публикации
                    var reasonButton = GetByTag("button", e => e.InnerText.Contains("Другая причина"));
                    reasonButton.InvokeMember("click");
                    Thread.Sleep(1000);

                    //Окончательное удаление
                    var removeButton = GetByTag("button", e => e.GetAttribute("className").Contains("js-confirm-action"));
                    var sendKeyTask = Task.Delay(500).ContinueWith((_) =>
                    {
                        SendKeys.SendWait("{Enter}");
                    });
                    removeButton.InvokeMember("click");

                    result = true;
                }
                else
                {
                    //Окончательное удаление
                    var removeButton = GetByTag("button", e => e.GetAttribute("className").Contains("js-confirm-action"));
                    var sendKeyTask = Task.Delay(500).ContinueWith((_) =>
                    {
                        SendKeys.SendWait("{Enter}");
                    });
                    removeButton.InvokeMember("click");

                    result = true;
                }
                
            });
            return result;
        }

        void GetUrl(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                var divContent = GetByTag("div", e => e.GetAttribute("className").Contains("content-text"));
                var a = GetChildElement(divContent, e => e.TagName == "A");
                var href = a.GetAttribute("href");
                bulletin.Url = href;

                UiHelper.UpdateActionState("URL успешно считан");
            }, continueExceptionMethod: (d2, e) => 
            {
                UiHelper.UpdateActionState("URL is NULL");
                Thread.Sleep(1000);
            });
        }

        BulletinState CheckBulletinState(string url)
        {
            var result = BulletinState.Error;
            DCT.Execute(d =>
            {
                UiHelper.UpdateActionState("Проверка состояния");

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

                    UiHelper.UpdateActionState($"Новое состояние {(int)result}");
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


        #region Tests

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
                        if (form != null)
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

        public override void GetBulletinList2(IEnumerable<TaskCache> tasks)
        {
            throw new NotImplementedException();
        }

        public override void GetBulletinDetails2(IEnumerable<TaskCache> tasks)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
