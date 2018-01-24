using ExcelDataReader;
using ParserTestApp.Containers.Base;
using ParserTestApp.Containers.Category;
using ParserTestApp.Data;
using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace ParserTestApp.Containers
{
    public class ContainerAvito : BulletinContainerBase
    {
        public override string StartUrl => @"www.avito.ru/";

        public override string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";

        public override string Login => "t3st59@1cbit.ru";

        public override string Password => "123QWEasd";

        /// <summary>
        /// 
        /// </summary>
        public override void Authorization()
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


            }, _DCTGroup.ContainerAvito);
        }

        public override void DisableBulletin(int bulletinId)
        {
            _DCT.Execute(data =>
            {
                var bulletinPage = "https://www.avito.ru/moskva/predlozheniya_uslug/sozdanie_supersayta_1008446335";
                var doPage = $"{bulletinPage}/do";

                WebWorker.DownloadPage(doPage, (doc) =>
                {
                    //Удалить объявление
                    var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("value") == "delete");
                    if (radioButton != null) radioButton.InvokeMember("click");


                    //Далее →
                    var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var button = buttons.FirstOrDefault(btn => btn.InnerText == "Далее →");
                    if (button != null)
                        button.InvokeMember("click");

                    WebWorker.JustWait(1);

                    //Нашёл на Avito
                    var deleteButtons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var count = deleteButtons.Count();
                    var deleteButton = deleteButtons.FirstOrDefault(btn => btn.InnerText == "Нашёл на Avito");
                    if (deleteButton != null)
                        deleteButton.InvokeMember("click");

                });
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulletinId"></param>
        public override void EditBulletin(int bulletinId)
        {
            _DCT.Execute(data =>
            {
                var bulletinPage = "https://www.avito.ru/moskva/predlozheniya_uslug/sozdanie_supersayta_1008446335";
                var editPage = $"{bulletinPage}/edit";

                WebWorker.DownloadPage(editPage, (doc) =>
                {
                    //Редактирование описания
                    var descriptionForm = WebWorker.WebDocument.GetElementsByTagName("textarea").Cast<HtmlElement>().FirstOrDefault();
                    if (descriptionForm != null)
                    {
                        var description = descriptionForm.GetAttribute("value");
                        descriptionForm.SetAttribute("value", description + "!");
                    }

                    //Редактирование картинки
                    var uploaderForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("name") == "image");
                    if(uploaderForm != null)
                    {
                        uploaderForm.Focus();

                        Task.Delay(250).ContinueWith((a) =>
                        {
                            SendKeys.SendWait("https://portal.1cbit.ru/images/Logo.fw.png" + "{ENTER}");
                        });

                        uploaderForm.InvokeMember("click");

                        Thread.Sleep(1000);
                    }
                    //Продолжить без пакета
                    var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("id") == "pack3");
                    if (radioButton != null) radioButton.InvokeMember("click");

                    var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var pack = "Продолжить без пакета";
                    var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                    if (button != null)
                        button.InvokeMember("click");
                });

                WebWorker.JustWait(1);

                var confirmPage = $"{editPage}/confirm";
                WebWorker.DownloadPage(confirmPage, (doc) =>
                {
                    //Снимаем галочки
                    var servicePremium = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("id") == "service-premium");
                    if (servicePremium != null)
                        servicePremium.InvokeMember("click");

                    //    //По умолчанию галочка снята
                    //var serviceUp = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    // .FirstOrDefault(q => q.GetAttribute("id") == "service-up-x2");
                    //  if (serviceUp != null)
                    //    serviceUp.InvokeMember("click");

                    //Подтверждаем
                    var text = "Продолжить";
                    var buttonContinue = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                    if (buttonContinue != null)
                        buttonContinue.InvokeMember("click");
                });
            });
        }

        public override void ExitProfile()
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/profile/exit", null);
            }, _DCTGroup.ContainerAvito);
        }

        public override void GetMessages(int bulletinId)
        {
            throw new NotImplementedException();
        }

        public override void GetViewStatistics(int bulletinId)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Тест
        /// </summary>
        /// <remarks>Моя ремарка</remarks>
        /// <example>
        /// <code>
        /// var i = 5;
        /// 
        /// </code>
        /// </example>
        /// <param name="bulletinId"></param>
        public override void PublishBulletin(int bulletinId)
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        //GetCategoryTrees();

                        var group = new Group("Услуги", "Предложение услуг", "IT, интернет, телеком", "Cоздание и продвижение сайтов");
                        //1
                        //Категория - Услуги
                        var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category1);
                        if (categoryRadio == null) return;
                        categoryRadio.InvokeMember("click");

                        //2
                        //Услуги - Предложения услуг
                        var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category2);
                        if (serviceRadio == null) return;
                        serviceRadio.InvokeMember("click");

                        //3
                        //Вид услуги - IT, интернет, телеком
                        var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category3);
                        if (serviceTypeRadio == null) return;
                        serviceTypeRadio.InvokeMember("click");

                        //4
                        //Тип услуги - Cоздание и продвижение сайтов
                        var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category4);
                        if (serviceTypeRadio2 == null) return;
                        serviceTypeRadio2.InvokeMember("click");

                        var parser = new AvitoParserContainer(WebWorker.WebDocument);
                        parser.Execute(group);
                        parser.SaveAsXml();


                        //Заполнение заявки
                        var address = "ул Нижегородская, 29-33с1";
                        var addressForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "flt_param_address");
                        if (addressForm != null) addressForm.SetAttribute("value", address);

                        var title = "Создание суперсайта";
                        var titleForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__title");
                        if (titleForm != null) titleForm.SetAttribute("value", title);

                        var description = "Создаем сайты любой сложности быстро, качественно и дешево";
                        var descriptionForm = WebWorker.WebDocument.GetElementsByTagName("textarea").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__description");
                        if (descriptionForm != null) descriptionForm.SetAttribute("value", description);

                        var price = "1000";
                        var priceForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__price");
                        if (priceForm != null) priceForm.SetAttribute("value", price);

                        //Продолжить с пакетом «Обычная продажа»
                        var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "pack3");
                        if (radioButton != null) radioButton.InvokeMember("click");


                        var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                        var pack = "Продолжить с пакетом «Обычная продажа»";
                        var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                        if (button != null)
                            button.InvokeMember("click");
                    }
                });

                WebWorker.DownloadPage("https://www.avito.ru/additem/confirm", (doc) =>
                {
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
                });

            }, _DCTGroup.ContainerAvito);
        }


        void GetCategoryTrees(CategoryTree parentNode, string parentValue, string prevValue = null)
        {
           
            var byDataId = false;
            if(string.IsNullOrEmpty(parentValue))
            {
                var div = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                    .LastOrDefault(q => q.GetAttribute("className") == "form-category js-form-category_param");
                if(div != null)
                {
                    parentValue = div.GetAttribute("data-param-id");
                    byDataId = true;
                }
            }

            if (string.IsNullOrEmpty(parentValue) || parentValue == prevValue) return;


            var level = byDataId
                ? WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("name") == $"params[{parentValue}]")
                : WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("data-parent-id") == parentValue);

            foreach (var element in level)
            {
                //var elementValue = element.GetAttribute("value");
                var node = new CategoryTree(element.GetAttribute("title"));
                parentNode.AddChild(node);

                element.InvokeMember("click");
                Thread.Sleep(500);
                GetCategoryTrees(node, null, parentValue);
            }
        }


        void GetCategoryTrees()
        {
           
            var tree = new List<CategoryTree>();
            var root = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("name") == "root_category_id");

            foreach (var element in root)
            {
                var elementValue = element.GetAttribute("value");
                var node = new CategoryTree(element.GetAttribute("title"));
                tree.Add(node);

                element.InvokeMember("click");
                Thread.Sleep(500);
                GetCategoryTrees(node, elementValue);
            }

            var serializer = new XmlSerializer(typeof(CategoryTree));
            using (var sw = new StringWriter())
            {
                using (var writer = XmlWriter.Create(sw))
                {
                    serializer.Serialize(writer, tree);
                    var xml = sw.ToString(); 
                }
            }

        }

        public override void UpdateBulletin(int bulletinId)
        {
            _DCT.Execute(data =>
            {
                var bulletinPage = "https://www.avito.ru/moskva/predlozheniya_uslug/sozdanie_supersayta_1008446335";
                var doPage = $"{bulletinPage}/do";

                WebWorker.DownloadPage(doPage, doc =>
                {
                    //Поднять объявление в поиске
                    var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .FirstOrDefault(q => q.GetAttribute("value") == "3");
                    if (radioButton != null) radioButton.InvokeMember("click");

                    //Далее →
                    var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                    var button = buttons.FirstOrDefault(btn => btn.InnerText == "Далее →");
                    if (button != null)
                        button.InvokeMember("click");

                    WebWorker.JustWait(1);

                    //Выберите способ оплаты:
                    //Кошелек Avito
                    //Банковская карта
                    //Сбербанк-Онлайн
                    //Яндекс.Деньги
                    //QIWI-кошелек
                    //WebMoney
                    //Оплата наличными
                    //SMS

                });
            });
           

        }


        public override void LoadFromFile()
        {
            //_DCT.Execute(data =>
            //{
                var package = new ExcelBulletinPackage();
                var filePath = @"E:\_NewWorkspace\BulletinApp\BulletinApp\ParserTestApp\bin\Debug\Выгрузка.xlsx";
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet();
                        var table = result.Tables[0];
                        var stage = XlsStage.None;
                        foreach (DataRow row in table.Rows)
                        {
                            if (stage == XlsStage.None && !string.IsNullOrEmpty(row[0].ToString()))
                                stage = XlsStage.ReadingCategories;
                            else if (stage == XlsStage.ReadingCategories && string.IsNullOrEmpty(row[0].ToString()))
                                stage = XlsStage.ReadingFields;
                            else if (stage == XlsStage.ReadingFields)
                                stage = XlsStage.ReadingBulletins;

                            if (stage == XlsStage.ReadingCategories)
                            {
                                var board = new ExcelCategory();
                                board.Board = row[table.Columns[0]].ToString();
                                board.Category1 = row[table.Columns[1]].ToString();
                                board.Category2 = row[table.Columns[2]].ToString();
                                board.Category3 = row[table.Columns[3]].ToString();
                                board.Category4 = row[table.Columns[4]].ToString();
                                board.Category5 = row[table.Columns[5]].ToString();

                                package.Categories.Add(board);
                            }
                            if (stage == XlsStage.ReadingFields)
                            {
                                var columnsCount = 0;
                                foreach (DataColumn col in table.Columns)
                                {
                                    if (columnsCount == 0)
                                    {
                                        columnsCount++;
                                        continue;
                                    }
                                    package.Fields.Add(row[col].ToString());

                                    columnsCount++;
                                }
                            }

                            if (stage == XlsStage.ReadingBulletins)
                            {
                                var bulletin = new ExcelBulletin();

                                var columnsCount = 0;
                                foreach (DataColumn col in table.Columns)
                                {
                                    if (columnsCount == 0)
                                    {
                                        columnsCount++;
                                        continue;
                                    }
                                    var field = package.Fields[columnsCount - 1];
                                    bulletin.Fields.Add(field, row[col].ToString());
                                    columnsCount++;
                                }
                                package.Bulletins.Add(bulletin);
                            }
                        }
                    }
                }

                var currentBoard = package.Categories.FirstOrDefault();

                foreach(var bulletin in package.Bulletins)
                {
                    WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                    {
                        if (WebWorker.WebDocument != null)
                        {
                           var body = WebWorker.WebDocument.Body;
                           foreach(HtmlElement el in body.Children)
                            {
                                Console.WriteLine(el.TagName);
                            }
                           var divs = WebWorker.WebDocument.GetElementsByTagName("div"); 
                            

                            //1
                            //Категория - Услуги
                            var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == currentBoard.Category1);
                            if (categoryRadio == null) return;
                            categoryRadio.InvokeMember("click");

                            //2
                            //Услуги - Предложения услуг
                            var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == currentBoard.Category2);
                            if (serviceRadio == null) return;
                            serviceRadio.InvokeMember("click");

                            //3
                            //Вид услуги - IT, интернет, телеком
                            var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == currentBoard.Category3);
                            if (serviceTypeRadio == null) return;
                            serviceTypeRadio.InvokeMember("click");

                            //4
                            //Тип услуги - Cоздание и продвижение сайтов
                            var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == currentBoard.Category4);
                            if (serviceTypeRadio2 == null) return;
                            serviceTypeRadio2.InvokeMember("click");

                            //Заполнение заявки из списка полей
                            foreach (var bulletinField in package.Fields)
                            {
                                var label = WebWorker.WebDocument.GetElementsByTagName("label").Cast<HtmlElement>()
                                   .FirstOrDefault(q => q.InnerText == bulletinField);
                                if(label != null)
                                {
                                    var inputFounded = false;
                                    var maxDepth = 10;
                                    var currentDepth = 0;
                                    var element = label;
                                    while (!inputFounded)
                                    {
                                        var parent = element.Parent;
                                        
                                        var parentChildren = parent.Children.Count;
                                        var fChild = parent.FirstChild;
                                        var parentSibling = parent.Parent.NextSibling;

                                        var children = element.FirstChild;
                                        var sibling = element.NextSibling;

                                        if(sibling != null && sibling.TagName == "input")
                                        {
                                            sibling.SetAttribute("value", bulletin.Fields[bulletinField]);
                                            inputFounded = true;
                                        }
                                        else if(sibling != null)
                                        {
                                            element = sibling;
                                        }
                                        else if(sibling == null)
                                        {
                                            break;
                                        }
                                        if (currentDepth >= maxDepth)
                                            break;
                                        currentDepth++;
                                    }
                                }
                            }

                            //Продолжить с пакетом «Обычная продажа»
                            var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "pack3");
                            if (radioButton != null) radioButton.InvokeMember("click");


                            var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                            var pack = "Продолжить с пакетом «Обычная продажа»";
                            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                            if (button != null)
                                button.InvokeMember("click");
                        }
                    });

                    WebWorker.DownloadPage("https://www.avito.ru/additem/confirm", (doc) =>
                    {
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
                    });
                }
            //}, _DCTGroup.ContainerAvito);



        }
    }

    public enum XlsStage
    {
        None = 0,
        ReadingCategories = 1,
        ReadingFields = 2,
        ReadingBulletins = 3
    }
}
