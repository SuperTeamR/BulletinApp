using ParserTestApp.Containers.Base;
using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserTestApp.Containers
{
    public class ContainerAvito : BulletinContainerBase
    {
        public override string StartUrl => @"www.avito.ru/";

        public override string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";

        public override string Login => "t3st59@1cbit.ru";

        public override string Password => "123QWEasd";


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
            throw new NotImplementedException();
        }

        public override void EditBulletin(int bulletinId)
        {
            throw new NotImplementedException();
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

        public override void PublishBulletin(int bulletinId)
        {
            _DCT.Execute(data =>
            {

                

                WebWorker.DownloadPage("https://www.avito.ru/additem", (doc) =>
                {
                    if (WebWorker.WebDocument != null)
                    {
                        //1
                        //Категория - Услуги
                        var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == "Услуги");
                        if (categoryRadio == null) return;
                        categoryRadio.InvokeMember("click");

                        //2
                        //Услуги - Предложения услуг
                        var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == "Предложение услуг");
                        if (serviceRadio == null) return;
                        serviceRadio.InvokeMember("click");

                        //3
                        //Вид услуги - IT, интернет, телеком
                        var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == "IT, интернет, телеком");
                        if (serviceTypeRadio == null) return;
                        serviceTypeRadio.InvokeMember("click");

                        //4
                        //Тип услуги - Cоздание и продвижение сайтов
                        var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == "Cоздание и продвижение сайтов");
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

        public override void UpdateBulletin(int bulletinId)
        {
            throw new NotImplementedException();
        }
    }
}
