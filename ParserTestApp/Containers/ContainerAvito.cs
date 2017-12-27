using ParserTestApp.Containers.Base;
using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using mshtml;

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
                        if (passwordForm != null) passwordForm.InnerText = Password;

                        var signIn = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                               .FirstOrDefault(btn => btn.GetAttribute("value") == "Войти" && btn.GetAttribute("type") == "submit");

                        if (signIn != null)
                            signIn.InvokeMember("click");
                    }
                });
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
            throw new NotImplementedException();
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
                        var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                        var button = buttons.FirstOrDefault(btn => btn.InnerText == @"Продолжить с пакетом «Обычная продажа»");
                        if (button != null)
                            button.InvokeMember("Click");

                        //InvokeScript(WebWorker.WebDocument);
                    }
                });

                //WebWorker.WaitPage(doc =>
                //{
                    

                //});

                //WebWorker.DownloadPage("https://www.avito.ru/additem/confirm", (doc) =>
                //{

                //});

                }, _DCTGroup.ContainerAvito);
        }

        public override void UpdateBulletin(int bulletinId)
        {
            throw new NotImplementedException();
        }



        void InvokeScript(HtmlDocument doc)
        {
            String script =
                @"function triggerClickOnButton(){
                  var buttons = document.getElementsByTagName('button');
                  for(var i = 0; i < buttons.length; i++){
                    if(buttons[i].value == 'Продолжить с пакетом «Обычная продажа»')
                        buttons[i].click();
                    }
                  }";
            InjectScript(doc, script);
            doc.InvokeScript("triggerClickOnButton");
        }

        void InjectScript(HtmlDocument doc, String scriptText)
        {
            HtmlElement head = doc.GetElementsByTagName("head")[0];
            HtmlElement scriptEl = doc.CreateElement("script");
            IHTMLScriptElement element = (IHTMLScriptElement)scriptEl.DomElement;
            element.text = scriptText;
            head.AppendChild(scriptEl);
        }
    }
}
