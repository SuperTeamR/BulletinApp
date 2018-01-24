using BusinessLogic.BoardLogic.Base;
using BusinessLogic.Data;
using CommonTools;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Boards
{
    internal class AvitoContainer : BoardContainerBase
    {
        public string StartUrl => @"www.avito.ru/";

        public string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";

        public string Login => "t3st59@1cbit.ru";

        public string Password => "123QWEasd";


        public override void AddBulletin()
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        //GetCategoryTrees();

                        var group = new Data.Group("Услуги", "Предложение услуг", "IT, интернет, телеком", "Cоздание и продвижение сайтов");
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
        public override void Exit()
        {
            _DCT.Execute(data =>
            {
                WebWorker.DownloadPage("https://www.avito.ru/profile/exit", null);
            });
        }

        public override void CloseBulletin()
        {

        }

        public override void EditBulletin()
        {

        }



        public override bool FillCaptcha()
        {
            return true;
        }

        public override void GetBulletinId()
        {

        }

        public override void GetBulletinState()
        {

        }



        public override void GetStats()
        {

        }

        public override bool IsAccountBlocked()
        {
            return false;
        }

        public override bool IsBan()
        {
            return false;
        }

        public override void Registry()
        {

        }

        public override void UpdateBulletin()
        {

        }

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




        public override IEnumerable<Board> Initialize()
        {
            return null;
        }

        public override IEnumerable<Board> GetAll()
        {
            return null;
        }

        public override Board Get()
        {
            return null;
        }
    }
}
