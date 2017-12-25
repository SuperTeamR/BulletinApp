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
    public class ContainerAvito : ParseContainerBase
    {
        public override string StartUrl => @"www.avito.ru/";

        public override string LoginUrl => @"https://www.avito.ru/profile/login?next=%2Fprofile";

        public override string Login => "t3st59@yandex.ru";

        public override string Password => "123QWEasd";


        public override void Authorization()
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

                    if (signIn != null) signIn.InvokeMember("click");
                }
            });
        }
    }
}
