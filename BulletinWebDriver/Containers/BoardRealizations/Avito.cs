using BulletinBridge.Models;
using BulletinWebDriver.Core;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebDriver.Containers.BoardRealizations
{
    public class Avito : BoardElement
    {
        #region Property
        public override string URL { get => "https://www.avito.ru"; }
        public override IEnumerable<string> IPExceptionsString => new[] { "Доступ с вашего IP-адреса временно ограничен", "Доступ временно заблокирован" };
        public override IEnumerable<string> BlockedExceptionsString => new[] { "Учётная запись заблокирована по причине", "Доступ заблокирован" };
        public override int PageNavigationTimeout => 3000;
        #endregion
        #region Constructor
        public Avito() : base("Avito")
        {
        }
        #endregion
        #region Default
        public override bool Auth(FirefoxDriver driver, string login, string password)
        {
            WaitExecute(driver);
            Find(driver, "a", "data-marker", "header/login-button", e => JsClick(driver, e));
            WaitExecute(driver);
            Find(driver, "input", "data-marker", "login-form/login", e => e.SendKeys(login));
            Find(driver, "input", "data-marker", "login-form/password", e => e.SendKeys(password));
            WaitExecute(driver);
            Find(driver, "button", "data-marker", "login-form/submit", e => JsClick(driver, e));
            WaitExecute(driver);
            if (driver.PageSource.Contains("href=\"/profile/exit\""))
            {
                if (!driver.PageSource.Contains(""))
                    return true;
            }
            return false;
        }
        #endregion
        #region Realization
        public override bool CheckAccess(FirefoxDriver driver, TaskAccessCheckCache taskModel)
        {
            return Auth(driver, taskModel.Login, taskModel.Password);
            //return Auth(driver, "RyboMan02@gmail.com", "Zcvb208x");
        }

        public override bool InstancePublication(FirefoxDriver driver, TaskInstancePublicationCache taskModel)
        {
            if (!Auth(driver, taskModel.Login, taskModel.Password))
                return false;
            ///Переход добавлеине + заполенине полей
            ///!!! Проверка аккаунта на квоту и блокировка
            //PrepareBulletin(bulletin);

            //Выбор типа объявления + кнопка продолжение
            //ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));

            //Публикация
            //Publicate(bulletin);

            //Получить URL для буллетина
            //GetUrl(bulletin);
            return true;
        }
        #endregion
    }
}
