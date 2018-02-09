using BulletinExample.Core;
using BulletinExample.Entity.Data;
using BulletinExample.Logic.Containers.Base.Board;
using BulletinExample.Logic.Containers.Base.Field;
using BulletinExample.Logic.Containers.Base.Group;
using BulletinExample.Logic.Data;
using BulletinExample.Tools;
using Data.Enums;
using FessooFramework.Tools.Helpers;
using FessooFramework.Tools.Repozitory;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BulletinExample.Logic.Containers.Avito
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Контейнер, управляющий группой категорий </summary>
    ///
    /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
    ///-----------------------------------------------------------------------------------------------------------------------------------------------

    internal class AvitoBoardContainer : BoardContainerBase
    {
        public override Guid Uid => BoardIds.Avito;
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Стандарный url для авторизации </summary>
        ///
        /// <value> The login URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Стандартный url для входа в профиль  </summary>
        ///
        /// <value> The profile URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string ProfileUrl => @"https://www.avito.ru/profile";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Выход из профиля на борде </summary>
        ///
        /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void Exit()
        {
            DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/profile/exit", null);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Авторизация на борде </summary>
        ///
        /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override bool Auth()
        {
            var result = false;
            DCT.Execute(data =>
            {
                var access = data.Db1.Accesses.FirstOrDefault(q => q.BoardId == Uid && q.UserId == data.Objects.CurrentUser.Id && q.State == (int)AccessState.Activated);
                if (access == null) return;

                data.Objects.CurrentAccess = access;

                WebWorker.DownloadPage(LoginUrl, (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        var e = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>();
                        var loginForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("name") == "login");
                        if (loginForm != null) loginForm.SetAttribute("value", access.Login);

                        var passwordForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "password");
                        if (passwordForm != null) passwordForm.SetAttribute("value", access.Password);

                        var signIn = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                               .FirstOrDefault(btn => btn.InnerText == "Войти" && btn.GetAttribute("type") == "submit");

                        if (signIn != null)
                            signIn.InvokeMember("click");
                    }
                });
                //Без принудительного ожидания даже с Application.DoEvents авторизация не сработает, если перейти на другую страницу
                WebWorker.JustWait(2);
                result = true;
            });
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Загружает все группы и поля Board. </summary>
        ///
        /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void ReloadGroups()
        {
            DCT.Execute(data =>
            {
                Exit();
                Auth();

                Thread.Sleep(1000);

                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    var groupContainer = GroupContainerList.Get(Uid);
                    groupContainer.Reinitialize();
                });
            });
        }


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем список буллетинов учетки и сохраняем новые в БД </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetBulletins()
        {
            DCT.Execute(data =>
            {
                Exit();
                Auth();
                var context = DCT.Context;
                var bulletins = new List<BulletinPackage>();
                var tabStates = new List<TabState>();
                WebWorker.Execute(() =>
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
                });
                WebWorker.Execute(() =>
                {
                    foreach (var tabState in tabStates)
                    {
                        WebWorker.NavigatePage(tabState.Href);

                        var nextPages = new List<string>
                        {
                            tabState.Href
                        };

                        var hasNextPage = true;
                        do
                        {
                            var result = GetBulletinPages(tabState.Title);
                            bulletins.AddRange(result);

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
                    var groupContainer = GroupContainerList.Get(Uid);
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

                        var fieldSetter = FieldContainerList.Get(Uid);
                        fieldSetter.LoadFieldsFromGroup(bulletin.Signature);

                        var dictionary = new Dictionary<string, string>();
                        foreach (var field in fieldSetter.Fields)
                        {
                            var value = fieldSetter.GetFieldValue(field.Value.Id);
                           dictionary.Add(field.Key, value);
                        }
                        bulletin.Fields = dictionary;

                    }
                });

                WebWorker.Execute(() =>
                {
                    DCT.Execute(d =>
                    {
                        foreach (var bulletin in bulletins)
                        {
                            var url = bulletin.Url;
                            var bs = d.Db1.BulletinInstances.ToArray();
                            var dbBulletinInstance = d.Db1.BulletinInstances.FirstOrDefault(q => q.Url == url);
                            if (dbBulletinInstance == null)
                            {
                                var dbBulletin = new Bulletin
                                {
                                    UserId = d.Objects.CurrentUser.Id,
                                };
                                d.Db1.Bulletins.Add(dbBulletin);
                                d.Db1.SaveChanges();

                                var groupId = Guid.Empty;
                                if(bulletin.Signature != null)
                                {
                                    var groupHash = bulletin.Signature.GetHash();

                                    var dbGroup = d.Db1.Groups.FirstOrDefault(q => q.Hash == groupHash);
                                    groupId = dbGroup.Id;
                                }
                               
                                dbBulletinInstance = new BulletinInstance
                                {
                                    GroupId = groupId,
                                    AccessId = d.Objects.CurrentAccess.Id,
                                    BoardId = Uid,
                                    BulletinId = dbBulletin.Id,
                                    HasRemoved = false,
                                    LastChangeId = null,
                                    Url = bulletin.Url,
                                    State = 0,
                                };
                                d.Db1.BulletinInstances.Add(dbBulletinInstance);
                                d.Db1.SaveChanges();

                                if (bulletin.Fields == null) continue;

                                foreach(var field in bulletin.Fields)
                                {
                                    var dbFieldTemplate = d.Db1.FieldTemplates.FirstOrDefault(q => q.Name == field.Key);
                                    if(dbFieldTemplate != null)
                                    {
                                        var dbBulletinField = d.Db1.BulletinFields.FirstOrDefault(q => q.BulletinInstanceId == dbBulletinInstance.Id && q.FieldId == dbFieldTemplate.Id);
                                        if(dbBulletinField == null)
                                        {
                                            dbBulletinField = new BulletinField
                                            {
                                                BulletinInstanceId = dbBulletinInstance.Id,
                                                FieldId = dbFieldTemplate.Id,
                                                Value = field.Value,
                                            };
                                            d.Db1.BulletinFields.Add(dbBulletinField);
                                            d.Db1.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    });
                });
                //WebWorker.Execute(() =>
                //{
                //    var groupContainer = GroupContainerList.Get(Uid);
                //    foreach (var bulletin in bulletins)
                //    {
                //        var url = Path.Combine(bulletin.Url, "edit");
                //        WebWorker.NavigatePage(url);
                //        Thread.Sleep(1500);

                //        var groupElement = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                //            .FirstOrDefault(q => q.GetAttribute("className").Contains("form-category-path"));

                //        if (groupElement == null) continue;

                //        var categories = groupElement.InnerText.Split('/').Select(q => q.Trim()).ToArray();
                //        bulletin.Signature = new GroupSignature(categories);

                //        var group = groupContainer.Get(bulletin.Signature.GetHash());

                //        if (group == null) continue;

                //        var fieldSetter = FieldContainerList.Get(Uid);
                //        fieldSetter.LoadFieldsFromGroup(bulletin.Signature);

                //        var dictionary = new Dictionary<string, string>();
                //        foreach (var field in group.Fields)
                //        {
                //            var value = fieldSetter.GetField(field.Key);
                //            dictionary.Add(field.Key, value);
                //        }
                //        bulletin.Fields = dictionary;
                //    }


                    //});

                    //WebWorker.Execute(() =>
                    //{
                    //    var groups = bulletins.Where(q => q.Fields != null && q.Fields.Count > 0).GroupBy(q => q.Signature.ToString());
                    //    foreach (var groupedBulletins in groups)
                    //    {
                    //        var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"edit_bulletins[{groupedBulletins.Key}].xlsx"));
                    //        if (xls.Exists)
                    //            xls.Delete();
                    //        var firstBulletin = groupedBulletins.FirstOrDefault();
                    //        using (var package = new ExcelPackage(xls))
                    //        {
                    //            var worksheet = package.Workbook.Worksheets.Add("Мои объявления");
                    //            var keys = firstBulletin.Fields.Keys;
                    //            var count = 0;
                    //            foreach (var k in keys)
                    //            {
                    //                var cell = worksheet.Cells[1, count + 1];
                    //                cell.Style.Font.Size = 14;
                    //                cell.Value = k;
                    //                cell.AutoFitColumns();
                    //                count++;
                    //            }
                    //            var row = 2;
                    //            foreach (var bulletin in groupedBulletins)
                    //            {
                    //                var column = 0;
                    //                foreach (var k in keys)
                    //                {
                    //                    var cell = worksheet.Cells[row, column + 1];
                    //                    var field = bulletin.Fields[k];
                    //                    cell.Value = field;
                    //                    column++;
                    //                }

                    //                worksheet.Column(column + 1).Hidden = true;
                    //                var urlCell = worksheet.Cells[row, column + 1];
                    //                urlCell.Value = bulletin.Url;

                    //                worksheet.Column(column + 2).Hidden = true;
                    //                var stateCell = worksheet.Cells[row, column + 2];
                    //                stateCell.Value = bulletin.State;
                    //                row++;
                    //            }

                    //            package.Save();
                    //        }

                    //    }
                    //});
                });
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Генерирует xls для буллетинов. </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetXlsBulletins()
        {
            DCT.Execute(d =>
            {
                Exit();
                Auth();

                WebWorker.Execute(() =>
                {
                    DCT.Execute(data =>
                    {
                        var bulletins = data.Db1.BulletinInstances.Where(q => q.AccessId == data.Objects.CurrentAccess.Id).ToArray();

                        var groups = bulletins.Where(q => q.GroupId != Guid.Empty).GroupBy(q => q.GroupId);
                        foreach (var groupedBulletins in groups)
                        {
                            var dbGroup = data.Db1.Groups.FirstOrDefault(q => q.Id == groupedBulletins.Key);
                            var groupContainer = GroupContainerList.Get(Uid);
                            var groupPackage = groupContainer.GetGroupPackage(dbGroup.Hash);

                            var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"edit_bulletins[{groupPackage}].xlsx"));
                            if (xls.Exists)
                                xls.Delete();

                            using (var package = new ExcelPackage(xls))
                            {
                                var worksheet = package.Workbook.Worksheets.Add("Мои объявления");
                                var pairs = groupPackage.Fields;


                                var count = 0;
                                foreach (var pair in pairs)
                                {
                                    var cell = worksheet.Cells[1, count + 2];
   
                                    cell.Style.Font.Size = 14;
                                    cell.Value = pair.Key;
                                    cell.AutoFitColumns();
                                    count++;
                                }
                                var row = 2;
                                foreach (var bulletin in groupedBulletins)
                                {
                                    var dbFields = data.Db1.BulletinFields.Where(q => q.BulletinInstanceId == bulletin.Id).ToArray();
                                    var column = 0;
                                    foreach (var f in groupPackage.Fields)
                                    {
                                        var name = f.Key;
                                        var fieldTemplate = data.Db1.FieldTemplates.FirstOrDefault(q => q.Name == name);
                                        var bulletinField = data.Db1.BulletinFields.FirstOrDefault(q => q.FieldId == fieldTemplate.Id && q.BulletinInstanceId == bulletin.Id);
                                        var cell = worksheet.Cells[row, column + 2];

                                        var options = f.Value.Options;
                                        if (options != null && options.Length > 0)
                                        {
                                            var validation = worksheet.DataValidations.AddListValidation(cell.Address);
                                            validation.ShowErrorMessage = true;
                                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                                            validation.ErrorTitle = "An invalid value was entered";
                                            validation.Error = "Select a value from the list";
                                            for (var i = 0; i < options.Length; i++)
                                            {
                                                validation.Formula.Values.Add(options[i].Text);
                                            }
                                        }
                                        cell.Value = bulletinField.Value;
                                        column++;
                                    }

                                    worksheet.Column(1).Hidden = true;
                                    var urlCell = worksheet.Cells[row, 1];
                                    urlCell.Value = bulletin.Id;

                                    row++;
                                }
                                package.Save();
                            }
                        }
                    });
                });
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Генерирует пустую xls для группы. </summary>
        ///
        /// <remarks>   SV Milovanov, 06.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetXlsGroup(GroupSignature signature)
        {
            DCT.Execute(d =>
            {
                var groupContainer = GroupContainerList.Get(Uid);
                var groupPackage = groupContainer.GetGroupPackage(signature.GetHash());

                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"new_bulletins[{groupPackage}].xlsx"));
                if (xls.Exists)
                    xls.Delete();

                using (var package = new ExcelPackage(xls))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Мои объявления");
                    var pairs = groupPackage.Fields;


                    var count = 0;
                    foreach (var pair in pairs)
                    {
                        var cell = worksheet.Cells[1, count + 2];

                        cell.Style.Font.Size = 14;
                        cell.Value = pair.Key;
                        cell.AutoFitColumns();

                        var options = pair.Value.Options;
                        if (options != null && options.Length > 0)
                        {
                            var optCells = worksheet.Cells[2, count + 2, 100, count + 2];
                            worksheet.DataValidations.Clear();
                            var validation = worksheet.DataValidations.AddListValidation(optCells.Address);
                            validation.ShowErrorMessage = true;
                            validation.ErrorStyle = ExcelDataValidationWarningStyle.warning;
                            validation.ErrorTitle = "An invalid value was entered";
                            validation.Error = "Select a value from the list";
                            for (var i = 0; i < options.Length; i++)
                            {
                                validation.Formula.Values.Add(options[i].Text);
                            }
                        }
                        count++;
                    }
                    package.Save();
                }

            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетина через xls </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void AddBulletinsFromXls()
        {
            DCT.Execute(data =>
            {
                var group = new GroupSignature("Хобби и отдых", "Спорт и отдых", "Другое");
                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"bulletins[{group}].xlsx"));

                using (var package = new ExcelPackage(xls))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    var endOfColumns = false;
                    var columnCount = 0;
                    while (!endOfColumns)
                    {
                        if (!string.IsNullOrEmpty(worksheet.Cells[1, columnCount + 1].Value as string))
                            columnCount++;
                        else
                            endOfColumns = true;
                    }

                    var endOfBulletins = false;
                    var bulletinCount = 0;
                    var row = 0;
                    while (!endOfBulletins)
                    {
                        endOfBulletins = true;
                        for (var i = 0; i < columnCount; i++)
                        {
                            if (!string.IsNullOrEmpty(worksheet.Cells[row + 2, i + 1].Value as string))
                            {
                                endOfBulletins = false;
                                bulletinCount++;
                                row++;
                                continue;
                            }  
                        }
                        
                    }

                    Exit();
                    Auth();

                    for (var i = 0; i < bulletinCount; i++)
                    {
                        var dictionary = new Dictionary<string, string>();
                        for (var j = 0; j < columnCount; j++)
                        {
                            var header = worksheet.Cells[1, j + 1];
                            var cell = worksheet.Cells[i + 2, j + 1];
                            var key = header.Value as string;
                            var v = cell.Value != null ? cell.Value.ToString() : string.Empty;
                            dictionary.Add(key, v);
                        }

                        AddBulletin(group, dictionary);

                    }
                }
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавление буллетина </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///
        /// <param name="signature">    Сигнатура группы </param>
        /// <param name="fields">       Словарь полей </param>
        ///-------------------------------------------------------------------------------------------------

        public void AddBulletin(GroupSignature signature, Dictionary<string, string> fields)
        {
            DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    //1
                    if (!string.IsNullOrEmpty(signature.Category1))
                    {
                        var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                       .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category1);
                        if (categoryRadio == null) return;
                        categoryRadio.InvokeMember("click");
                        Thread.Sleep(1000);
                    }
                    //2
                    if (!string.IsNullOrEmpty(signature.Category2))
                    {
                        var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category2);
                        if (serviceRadio == null) return;
                        serviceRadio.InvokeMember("click");
                        Thread.Sleep(1000);
                    }
                    //3
                    if (!string.IsNullOrEmpty(signature.Category3))
                    {
                        var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category3);
                        if (serviceTypeRadio == null) return;
                        serviceTypeRadio.InvokeMember("click");
                        Thread.Sleep(1000);
                    }
                    //4
                    if (!string.IsNullOrEmpty(signature.Category4))
                    {
                        var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category4);
                        if (serviceTypeRadio2 == null) return;
                        serviceTypeRadio2.InvokeMember("click");
                        Thread.Sleep(1000);
                    }
                    //5
                    if (!string.IsNullOrEmpty(signature.Category5))
                    {
                        var serviceTypeRadio3 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == signature.Category5);
                        if (serviceTypeRadio3 == null) return;
                        serviceTypeRadio3.InvokeMember("click");
                        Thread.Sleep(1000);
                    }

                    var fieldSetter = FieldContainerList.Get(Uid);
                    fieldSetter.LoadFieldsFromGroup(signature);

                    foreach (var pair in fields)
                    {
                        var template = fieldSetter.Fields.FirstOrDefault(q => q.Key == pair.Key);
                        fieldSetter.SetFieldValue(template.Key, pair.Value);
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
                });
                WebWorker.DownloadPage("https://www.avito.ru/additem/confirm", (doc) =>
                {
                    ////Снимаем галочки
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

                    ////Подтверждаем
                    var text = "Продолжить";
                    var buttonContinue = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                    if (buttonContinue != null)
                        buttonContinue.InvokeMember("click");
                });
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Редактирует буллетины из xls. </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void EditBulletinsFromXls()
        {
            DCT.Execute(data =>
            {
                var group = new GroupSignature("Хобби и отдых", "Охота и рыбалка");
                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"edit_bulletins[{group}].xlsx"));

                using (var package = new ExcelPackage(xls))
                {
                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    var endOfColumns = false;
                    var columnCount = 0;
                    while (!endOfColumns)
                    {
                        if (!string.IsNullOrEmpty(worksheet.Cells[1, columnCount + 2].Value as string))
                            columnCount++;
                        else
                            endOfColumns = true;
                    }

                    var endOfBulletins = false;
                    var bulletinCount = 0;
                    while (!endOfBulletins)
                    {
                        if (!string.IsNullOrEmpty(worksheet.Cells[bulletinCount + 2, 1].Value as string))
                            bulletinCount++;
                        else
                            endOfBulletins = true;
                    }

                    Exit();
                    Auth();
                    for (var i = 0; i < bulletinCount; i++)
                    {
                        var dictionary = new Dictionary<string, string>();
                        for (var j = 0; j < columnCount; j++)
                        {
                            var header = worksheet.Cells[1, j + 2];
                            var cell = worksheet.Cells[i + 2, j + 2];
                            var key = header.Value as string;
                            var v = cell.Value != null ? cell.Value.ToString() : string.Empty;
                            dictionary.Add(key, v);
                        }
                        var guidCell = worksheet.Cells[i + 2, 1];
                        var guid = guidCell.Value as string;

                        EditBulletin(Guid.Parse(guid), dictionary);
                    }
                }
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Отредактировать буллетин </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///
        /// <param name="bulletinId">   Идентификатор буллетина </param>
        /// <param name="fields">       Словарь полей </param>
        ///-------------------------------------------------------------------------------------------------

        public void EditBulletin(Guid bulletinId, Dictionary<string, string> fields)
        {
            DCT.Execute(data =>
            {
                var dbBulletinInstance = data.Db1.BulletinInstances.FirstOrDefault(q => q.Id == bulletinId);
                if (dbBulletinInstance != null)
                {
                    var dbGroup = data.Db1.Groups.FirstOrDefault(q => q.Id == dbBulletinInstance.GroupId);
                    var groupContainer = GroupContainerList.Get(Uid);
                    var groupPackage = groupContainer.GetGroupPackage(dbGroup.Hash);

                    WebWorker.Execute(() =>
                    {
                        DCT.Execute(d =>
                        {
                            WebWorker.NavigatePage(Path.Combine(dbBulletinInstance.Url, "edit"));
                            Thread.Sleep(1000);

                            var fieldSetter = FieldContainerList.Get(Uid);
                            fieldSetter.LoadFieldsFromGroup(new GroupSignature(groupPackage.Categories));

                            foreach (var pair in fields)
                            {
                                var template = fieldSetter.Fields.FirstOrDefault(q => q.Key == pair.Key);
                                fieldSetter.SetFieldValue(template.Value.Id, pair.Value);
                            }

                            if (EnumHelper.GetValue<BulletinInstanceState>(dbBulletinInstance.State) != BulletinInstanceState.Publicated)
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

                                Thread.Sleep(1000);
                            }
                            else
                            {
                                var button = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                                    .FirstOrDefault(q => q.GetAttribute("type") == "submit" && q.InnerText == "Продолжить");
                                if (button != null)
                                    button.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                        });
                    });
                }
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets bulletin pages. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="state">    The state. </param>
        ///
        /// <returns>   The bulletin pages. </returns>
        ///-------------------------------------------------------------------------------------------------

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
                        State = state
                    };
                    GelChildrenRecursively(d, bulletin);
                    result.Add(bulletin);
                }
            });
            return result;
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
