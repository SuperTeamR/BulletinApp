using BulletinExample.Core;
using BulletinExample.Entity.Data;
using BulletinExample.Logic.Containers.Base.Board;
using BulletinExample.Logic.Containers.Base.Group;
using BulletinExample.Logic.Data;
using BulletinExample.Tools;
using Data.Enums;
using FessooFramework.Tools.Helpers;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BulletinExample.Logic.Containers.Avito
{
    internal class AvitoBoardContainer : BoardContainerBase
    {
        public override Guid Uid => BoardIds.Avito;
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets URL of the login. </summary>
        ///
        /// <value> The login URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets URL of the profile. </summary>
        ///
        /// <value> The profile URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string ProfileUrl => @"https://www.avito.ru/profile";


        public override void Exit()
        {
            DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/profile/exit", null);
            });
        }

        public override bool Auth()
        {
            var result = false;
            DCT.Execute(data =>
            {
                var access = data.Db1.Accesses.FirstOrDefault(q => q.BoardId == Uid && q.UserId == data.Objects.CurrentUser.Id && q.State == (int)Entity.Data.AccessState.Activated);
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
        /// <summary>   Gets the bulletins. </summary>
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

                        var nextPages = new List<string>();
                        nextPages.Add(tabState.Href);

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

                                dbBulletinInstance = new BulletinInstance
                                {
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
