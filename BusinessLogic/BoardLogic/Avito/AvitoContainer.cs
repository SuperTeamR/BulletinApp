using BusinessLogic.BoardLogic.Base;
using BusinessLogic.BoardLogic.Base.FieldSetter;
using BusinessLogic.Data;
using CommonTools;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Boards
{
    internal class AvitoContainer : BoardContainerBase
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets URL of the start. </summary>
        ///
        /// <value> The start URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string StartUrl => @"www.avito.ru/";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets URL of the profile. </summary>
        ///
        /// <value> The profile URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string ProfileUrl => @"https://www.avito.ru/profile";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets URL of the login. </summary>
        ///
        /// <value> The login URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the login. </summary>
        ///
        /// <value> The login. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Login => "mostrerkilltest@gmail.com";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the password. </summary>
        ///
        /// <value> The password. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Password => "OnlineHelp59";

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Редактирует буллетин. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void EditBulletin(Bulletin bulletin)
        {
            _DCT.Execute(data =>
            {
                WebWorker.Execute(() =>
                {
                    WebWorker.NavigatePage(Path.Combine(bulletin.Url, "edit"));

                    var fieldSetter = FieldSetterContainerList.Get(Uid);
                    fieldSetter.LoadFieldsFromGroup(bulletin.Signature);

                    foreach (var pair in bulletin.Fields)
                    {
                        fieldSetter.SetField(pair.Key, pair.Value);
                    }


                    if (bulletin.State == "Активные")
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

                WebWorker.DownloadPage(Path.Combine(bulletin.Url, "edit", "confirm"), (doc) =>
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

                    Thread.Sleep(1000);
                });
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавляем буллетин на борду. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="group">        Выбранная группа. </param>
        /// <param name="dictionary">   Словарь полей. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void AddBulletin(Data.Group group, Dictionary<string, string> dictionary)
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        //1
                        //Категория - Услуги
                        if(!string.IsNullOrEmpty(group.Category1))
                        {
                            var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                           .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category1);
                            if (categoryRadio == null) return;
                            categoryRadio.InvokeMember("click");

                            Thread.Sleep(1000);
                        }




                        //2
                        //Услуги - Предложения услуг
                        if (!string.IsNullOrEmpty(group.Category2))
                        {
                            var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category2);
                            if (serviceRadio == null) return;
                            serviceRadio.InvokeMember("click");

                            Thread.Sleep(1000);
                        }

                        

                        //3
                        //Вид услуги - IT, интернет, телеком
                        if (!string.IsNullOrEmpty(group.Category3))
                        {
                            var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category3);
                            if (serviceTypeRadio == null) return;
                            serviceTypeRadio.InvokeMember("click");

                            Thread.Sleep(1000);
                        }

                       

                        //4
                        //Тип услуги - Cоздание и продвижение сайтов
                        if (!string.IsNullOrEmpty(group.Category4))
                        {
                            var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category4);
                            if (serviceTypeRadio2 == null) return;
                            serviceTypeRadio2.InvokeMember("click");

                            Thread.Sleep(1000);
                        }



                        var fieldSetter = FieldSetterContainerList.Get(Uid);
                        fieldSetter.LoadFieldsFromGroup(new GroupSignature(group.Category1));

                        var select = WebWorker.WebDocument.GetElementById("flt_param_2843");
                        if (select != null) select.SetAttribute("value", "20122");

                        foreach (var pair in dictionary)
                        {
                            fieldSetter.SetField(pair.Key, pair.Value);
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
                    }
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

            }, _DCTGroup.ContainerAvito);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Авторизация на основной борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void Auth()
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage(LoginUrl, (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        var e = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>();
                        var loginForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("name") == "login");
                        if (loginForm != null) loginForm.SetAttribute("value", Login);

                        var passwordForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "password");
                        if (passwordForm != null) passwordForm.SetAttribute("value", Password);

                        var signIn = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                               .FirstOrDefault(btn => btn.InnerText == "Войти" && btn.GetAttribute("type") == "submit");

                        if (signIn != null)
                            signIn.InvokeMember("click");
                    }
                });
                //Без принудительного ожидания даже с Application.DoEvents авторизация не сработает, если перейти на другую страницу
                WebWorker.JustWait(1);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Выход из профиля на основной борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void Exit()
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/profile/exit", null);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Удаляет буллетин. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void CloseBulletin()
        {

        }



        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Зап. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>   True if it succeeds, false if it fails. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override bool FillCaptcha()
        {
            return true;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets bulletin identifier. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetBulletinId()
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets bulletin state. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="url">  URL of the document. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void GetBulletinState(string url)
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage(ProfileUrl, (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        var e = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>();
                        var loginForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("name") == "login");
                        if (loginForm != null) loginForm.SetAttribute("value", Login);

                        var passwordForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "password");
                        if (passwordForm != null) passwordForm.SetAttribute("value", Password);

                        var signIn = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                               .FirstOrDefault(btn => btn.InnerText == "Войти" && btn.GetAttribute("type") == "submit");

                        if (signIn != null)
                            signIn.InvokeMember("click");
                    }
                });
                //Без принудительного ожидания даже с Application.DoEvents авторизация не сработает, если перейти на другую страницу
                WebWorker.JustWait(1);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the statistics. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetStats()
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Query if this object is account blocked. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>   True if account blocked, false if not. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override bool IsAccountBlocked()
        {
            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Query if this object is ban. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>   True if ban, false if not. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override bool IsBan()
        {
            return false;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Registries this object. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void Registry()
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Updates the bulletin. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void UpdateBulletin()
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Загружает все группы и поля Board. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process load groups in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public override IEnumerable<Data.Group> LoadGroups()
        {
            var result = new List<Data.Group>();
            _DCT.Execute(data =>
            {
                Exit();
                Auth();

                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    var groupContainer = GroupContainerList.Get(Uid);
                    result = groupContainer.Initialize().ToList();
                });
            });
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Enumerates initialize in this collection. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process initialize in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public override IEnumerable<Board> Initialize()
        {
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public override IEnumerable<Board> GetAll()
        {
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Generates the XLS from group. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="signature">    The signature. </param>
        ///
        /// <returns>   The XLS from group. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override string GenerateXlsFromGroup(GroupSignature signature)
        {
            _DCT.Execute(data =>
            {
                var groupContainer = GroupContainerList.Get(Uid);
                var group = groupContainer.Get(signature.GetHash());

                var fields = group.Fields;
                var names = fields.Select(q => q.Key).ToArray();

                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"bulletins[{group.ToString()}].xlsx"));
                if (xls.Exists)
                    xls.Delete();
                using (var package = new ExcelPackage(xls))
                {
                    var worksheet = package.Workbook.Worksheets.Add("Мои объявления");
                    for (var i = 0; i < names.Length; i++)
                    {
                        var cell = worksheet.Cells[1, i + 1];
                        cell.Style.Font.Size = 14;
                        cell.Value = names[i];
                        cell.AutoFitColumns();
                    }

                    package.Save();
                }

            });
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Adds from XLS. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void AddFromXls()
        {
            _DCT.Execute(data =>
            {
                var group = new Data.Group("Хобби и отдых", "Спорт и отдых", "Другое");
                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"bulletinsУслуги,Предложение_услуг.xlsx"));

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
        /// <summary>   Edit from XLS. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void EditFromXls()
        {
            _DCT.Execute(data =>
            {
                var signature = new GroupSignature("Хобби и отдых", "Спорт и отдых", "Другое");
                //var signature = new GroupSignature("Личные вещи", "Товары для детей и игрушки", "Игрушки");
                var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"edit_bulletins[{signature.ToString()}].xlsx"));

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
                            var header = worksheet.Cells[1, j + 1];
                            var cell = worksheet.Cells[i + 2, j + 1];
                            var key = header.Value as string;
                            var v = cell.Value != null ? cell.Value.ToString() : string.Empty;
                            dictionary.Add(key, v);
                        }
                        var urlCell = worksheet.Cells[i + 2, columnCount + 1];
                        var url = urlCell.Value as string;

                        var stateCell = worksheet.Cells[i + 2, columnCount + 2];
                        var state = stateCell.Value as string;

                        var bulletin = new Bulletin
                        {
                            Fields = dictionary,
                            Signature = signature,
                            State = state,
                            Url = url
                        };
                        EditBulletin(bulletin);

                    }
                }
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets a board using the given hash. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="hash"> The hash to get. </param>
        ///
        /// <returns>   A Board. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override Board Get(string hash)
        {
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets the bulletins. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public override void GetBulletins()
        {
            _DCT.Execute(data =>
            {
                Exit();
                Auth();

                var bulletins = new List<Bulletin>();
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
                    foreach(var tabState in tabStates)
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

                    bulletins = bulletins;
                });

                WebWorker.Execute(() =>
                {
                    var groupContainer = GroupContainerList.Get(null);
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
                        
                        var group = groupContainer.Get(bulletin.Signature.GetHash());

                        if (group == null) continue;

                        var fieldSetter = FieldSetterContainerList.Get(Uid);
                        fieldSetter.LoadFieldsFromGroup(bulletin.Signature);

                        var dictionary = new Dictionary<string, string>();
                        foreach (var field in group.Fields)
                        {
                            var value = fieldSetter.GetField(field.Key);
                            dictionary.Add(field.Key, value);
                        }
                        bulletin.Fields = dictionary;
                    }

                    
                });

                WebWorker.Execute(() =>
                {
                    var groups = bulletins.Where(q => q.Fields != null && q.Fields.Count > 0).GroupBy(q => q.Signature.ToString());
                    foreach (var groupedBulletins in groups)
                    {
                        var xls = new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), $"edit_bulletins[{groupedBulletins.Key}].xlsx"));
                        if (xls.Exists)
                            xls.Delete();
                        var firstBulletin = groupedBulletins.FirstOrDefault();
                        using (var package = new ExcelPackage(xls))
                        {
                            var worksheet = package.Workbook.Worksheets.Add("Мои объявления");
                            var keys = firstBulletin.Fields.Keys;
                            var count = 0;
                            foreach (var k in keys)
                            {
                                var cell = worksheet.Cells[1, count + 1];
                                cell.Style.Font.Size = 14;
                                cell.Value = k;
                                cell.AutoFitColumns();
                                count++;
                            }
                            var row = 2;
                            foreach (var bulletin in groupedBulletins)
                            {
                                var column = 0;
                                foreach (var k in keys)
                                {
                                    var cell = worksheet.Cells[row, column + 1];
                                    var field = bulletin.Fields[k];
                                    cell.Value = field;
                                    column++;
                                }

                                worksheet.Column(column + 1).Hidden = true;
                                var urlCell = worksheet.Cells[row, column + 1];
                                urlCell.Value = bulletin.Url;

                                worksheet.Column(column + 2).Hidden = true;
                                var stateCell = worksheet.Cells[row, column + 2];
                                stateCell.Value = bulletin.State;
                                row++;
                            }

                            package.Save();
                        }
                        
                    }
                });

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

        List<Bulletin> GetBulletinPages(string state)
        {
            var result = new List<Bulletin>();
            _DCT.Execute(data =>
            {
                var s = WebWorker.WebDocument.Body.OuterHtml;
                var titleDivs = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                            .Where(q => q.GetAttribute("className").Contains("profile-item-description"));

                foreach (var d in titleDivs)
                {
                    var bulletin = new Bulletin
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

        void GelChildrenRecursively(HtmlElement element, Bulletin bulletin)
        {
            _DCT.Execute(data =>
            {
                if (!element.CanHaveChildren && element.Children.Count == 0) return;

                foreach(HtmlElement ch in element.Children)
                {
                    if(ch.TagName.ToLower() == "span" && ch.GetAttribute("className").Contains("profile-item-views-count"))
                    {
                        if (!string.IsNullOrEmpty(bulletin.Views)) continue;

                        var regex = new Regex(@"(?<count>\d+)");

                        var match = regex.Match(ch.InnerText);
                        if (match.Success)
                        {
                            bulletin.Views = match.Groups["count"].Value;
                        }
                            
                    }
                    if(ch.TagName.ToLower() == "a" && ch.GetAttribute("name").Contains("item_"))
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
