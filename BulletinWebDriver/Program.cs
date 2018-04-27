using BulletinWebDriver.Core;
using BulletinWebDriver.Helpers;
using BulletinWebDriver.Tools;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace BulletinWebDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            Bootstrapper.Current.Run();
            DCT.Context._SessionInfo.HashUID = "Engine";
            DCT.Context._SessionInfo.SessionUID = "Engine";

            var command = "TaskExecute";
            if (args != null && args.Any())
                command = args[0];

            while (true)
            {
                DCT.Execute(c =>
                {
                    switch (command)
                    {
                        case "TaskExecute":
                            TaskHelper.Execute();
                            Thread.Sleep(60000);
                            break;
                        default:
                            ConsoleHelper.SendMessage("Not found command");
                            Console.ReadLine();
                            break;
                    }
                });
            }
            //DCT.Execute(d =>
            //{
            //    //TestProxies();
            //    WebWorkerManager.Execute();
            //    //TestConnection();
            //    //TestClick();

            //    Console.ReadLine();
            //});
        }
        //static void TestConnection()
        //{
        //    //WebDriver.NavigatePage("http://yandex.ru");


        //    WebDriver.NavigatePage("https://www.avito.ru/profile/login?next=%2Fprofile");
        //    if (WebDriver.GetTitle().Contains("Доступ с вашего IP-адреса временно ограничен"))
        //    {
        //        WebDriver.RestartDriver();
        //        throw new Exception("Блокировка по IP - требуется смена прокси");
        //    }
        //    WebDriver.Find("input", "data-marker", "login-form/login", e => e.SendKeys("Slava.Shleif@gmail.com"));
        //    WebDriver.Find("input", "data-marker", "login-form/password", e => e.SendKeys("OnlineHelp59"));
        //    WebDriver.Find("button", "data-marker", "login-form/submit", e => WebDriver.JsClick(e));
        //}

        //static void TestProxies()
        //{
        //    DCT.ExecuteAsync(d =>
        //    {
        //        var count = 0;
        //        while (true)
        //        {
        //            var nextProxy = CollectorModels.Service.ProxyClientHelper.Next();
        //            if (nextProxy == null)
        //            {
        //                Console.WriteLine("Checked proxy is null");
        //                continue;
        //            }
        //            Console.WriteLine($"Proxy {count}|Avito:{nextProxy.Avito}|Https:{nextProxy.Https}|Http:{nextProxy.Http}|Google:{nextProxy.Google}|Ping:{nextProxy.PingLast}|={nextProxy.Address}:{nextProxy.Port}");
        //            count++;
        //        }
        //    });
        //}

        //static void TestClick()
        //{
        //    DCT.Execute(d =>
        //    {
        //        WebDriver.IsDisableProxy = true;
        //        WebDriver.GetTimeouts().PageLoad = new TimeSpan(0, 0, 0, 20);

        //        Auth();

        //        var url = "https://www.avito.ru/podolsk/telefony/iphone_6s_16gb_space_gray_1411832298";
        //        WebDriver.NavigatePage(Path.Combine(url, "edit"));

        //        var element = WebDriver.FindMany(By.ClassName("packages-tab-name")).FirstOrDefault(q => q.Text == "Без пакета");
        //        var internalActions = new Actions(WebDriver.driver);
        //        internalActions.MoveToElement(element);
        //        ((IJavaScriptExecutor)WebDriver.driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);

        //        element = WebDriver.FindMany(By.ClassName("packages-tab-name")).FirstOrDefault(q => q.Text == "Без пакета");
        //        internalActions.Click(element).Build().Perform();
        //    });
        //}
        //static bool Auth()
        //{
        //    var result = false;
        //    DCT.Execute(d =>
        //    {
        //        WebDriver.NavigatePage("https://www.avito.ru/profile/login?next=%2Fprofile");
        //        if (WebDriver.GetTitle().Contains("Доступ с вашего IP-адреса временно ограничен")
        //            || WebDriver.GetTitle().Contains("Доступ временно заблокирован"))
        //        {
        //            WebDriver.RestartDriver();
        //            throw new Exception("Блокировка по IP - требуется смена прокси");
        //        }
        //        WebDriver.Find("input", "data-marker", "login-form/login", e => e.SendKeys("Slava.Shleif@gmail.com"));
        //        WebDriver.Find("input", "data-marker", "login-form/password", e => e.SendKeys("OnlineHelp59"));
        //        WebDriver.Find("button", "data-marker", "login-form/submit", e => WebDriver.JsClick(e));
        //        Thread.Sleep(2000);

        //        var a = WebDriver.FindMany(By.TagName("a")).FirstOrDefault(q => q.Text == "Мои объявления");
        //        if (a != null)
        //            result = true;
        //    }, continueExceptionMethod: (d, e) =>
        //    {
        //        result = Auth();
        //    });
        //    if (!result)
        //    {
        //        result = Auth();
        //    }
        //    return result;
        //}
    }
}