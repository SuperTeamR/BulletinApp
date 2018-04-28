using BulletinBridge.Data;
using BulletinWebDriver;
using BulletinWebDriver.Tools;
using BulletinWebWorker.Containers.Base.Access;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

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
        AccessCache currentAccess { get; set; }
        public override bool TryAuth(AccessCache access)
        {
            var result = false;
            DCT.Execute(d =>
            {
                if(currentAccess == null || 
                (currentAccess.Login != access.Login && currentAccess.Password != access.Password))
                {
                    currentAccess = access;

                    Exit();
                    result = Auth();
                }
                else
                {
                    result = true;
                }
            });
            return result;
        }
        protected override bool Auth()
        {
            var result = false;
            DCT.Execute(d =>
            {
                WebDriver.NavigatePage(LoginUrl);
                if (WebDriver.GetTitle().Contains("Доступ с вашего IP-адреса временно ограничен")
                    || WebDriver.GetTitle().Contains("Доступ временно заблокирован"))
                {
                    WebDriver.RestartDriver();
                    throw new Exception("Блокировка по IP - требуется смена прокси");
                }
                WebDriver.Find("input", "data-marker", "login-form/login", e => e.SendKeys(currentAccess.Login));
                WebDriver.Find("input", "data-marker", "login-form/password", e => e.SendKeys(currentAccess.Password));
                WebDriver.Find("button", "data-marker", "login-form/submit", e => WebDriver.JsClick(e));
                Thread.Sleep(2000);

                var a = WebDriver.FindMany(By.TagName("a")).FirstOrDefault(q => q.Text == "Мои объявления");
                if (a != null)
                    result = true;
            }, continueExceptionMethod: (d, e) =>
            {
                result = Auth();
            });
            if(!result)
            {
                result = Auth();
            }
            return result;
        }
        protected override void Exit()
        {
            DCT.Execute(data =>
            {
                WebDriver.NavigatePage("https://www.avito.ru/profile/exit");
            });
        }
    }
}