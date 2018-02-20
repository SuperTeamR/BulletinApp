using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{
    class AvitoAccessContainer : AccessContainerBase
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


        AccessPackage currentAccess { get; set; }


        public override bool TryAuth(AccessPackage access)
        {
            var result = false;
            DCT.Execute(d =>
            {
                if(currentAccess == null || 
                (currentAccess.Login == access.Login && currentAccess.Password == access.Password))
                {
                    currentAccess = access;

                    Exit();
                    result = Auth();
                }
            });
            return result;
        }

        protected override bool Auth()
        {
            var result = false;
            DCT.Execute(d =>
            {
            WebWorker.NavigatePage(LoginUrl);
            //WebWorker.DownloadPage(LoginUrl, (doc) =>
            //{
                    if (WebWorker.WebDocument != null)
                    {
                        var e = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>();
                        var loginForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("name") == "login");
                        if (loginForm != null) loginForm.SetAttribute("value", currentAccess.Login);

                        var passwordForm = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "password");
                        if (passwordForm != null) passwordForm.SetAttribute("value", currentAccess.Password);

                        var signIn = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                               .FirstOrDefault(btn => btn.InnerText == "Войти" && btn.GetAttribute("type") == "submit");

                        if (signIn != null)
                            signIn.InvokeMember("click");
                    }
               // });
                //Без принудительного ожидания даже с Application.DoEvents авторизация не сработает, если перейти на другую страницу
                WebWorker.JustWait(2);
                result = true;
            });
            return result;
        }

        protected override void Exit()
        {
            DCT.Execute(data =>
            {
                WebWorker.NavigatePage("https://www.avito.ru/profile/exit");
            });
        }
    }
}
