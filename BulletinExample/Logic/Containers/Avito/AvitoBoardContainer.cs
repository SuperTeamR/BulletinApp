using BulletinExample.Core;
using BulletinExample.Logic.Containers.Base.Board;
using BulletinExample.Logic.Containers.Base.Group;
using BulletinExample.Tools;
using Data.Enums;
using System;
using System.Linq;
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
                var access = data.Db1.Accesses.FirstOrDefault(q => q.BoardId == Uid && q.UserId == data.Objects.CurrentUser.Id && q.State == (int)AccessState.Activated);
                if (access == null) return;

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
    }
}
