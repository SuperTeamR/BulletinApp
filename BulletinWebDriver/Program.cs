using BulletinWebDriver.Containers;
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
using BulletinBridge.Models;
using BulletinWebDriver.Containers.BoardRealizations;
using BulletinWebDriver.ServiceHelper;
using BulletinBridge.Data;

namespace BulletinWebDriver
{
    class Program
    {
        private static BoardContainer BoardContainer = new BoardContainer();

        static void Main(string[] args)
        {
            Bootstrapper.Current.Run();
            DCT.Context._SessionInfo.HashUID = "Engine";
            DCT.Context._SessionInfo.SessionUID = "Engine";
            ConsoleHelper.SendMessage($"Connect to {new HubServiceClient().Address}");
#if DEBUG
            //var num = OnlineSimHelper.CreateNumberAvito();
#endif

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
                            var task = DriverTaskHelper.Next();
                            if (task != null)
                                BoardContainer.Execute(task);
                            else
                                Thread.Sleep(5000);
                            break;
                        case "TestActivate":
                            TestActivate();
                            break;
                        case "TestInstanceActivate":
                            TestInstanceActivate();
                            break;
                        case "TestGetStatistics":
                            TestGetStatistics();
                            break;
                        case "TestInstanceStatistics":
                            TestInstanceStatistics();
                            break;
                        case "TestCollectMessages":
                            TestCollectMessages();
                            break;
                        case "TestForwardNumber":
                            TestForwardNumber();
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

        static void TestActivate()
        {
            var avito = new Avito();
            var task = new TaskAccessCheckCache
            {
                Login = "kirill.shleif@mail.ru",
                Password = "OnlineHelp59"
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                avito.ActivateBulletins(browser, task);
            }, null, 100);
        }
        static void TestInstanceActivate()
        {
            var avito = new Avito();
            var task = new TaskInstanceActivationCache
            {
                Login = "Kazah.Shleif_Novg@mail.ru",
                Password = "OnlineHelp59",
                Url = @"https://www.avito.ru/podolsk/telefony/apple_iphone_6_16gb_zolotoy_1717723420"
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                avito.ActivateBulletin(browser, task);
            }, null, 100);
        }
        static void TestGetStatistics()
        {
            var avito = new Avito();
            var task = new TaskAccessCheckCache
            {
                AccessId = new Guid("BBC4B038-7309-4299-BB59-D8F0119EB7B5"),
                Login = "kirill.shleif@mail.ru",
                Password = "OnlineHelp59"
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                var stat = avito.GetAccessStatistics(browser, task);
                if (stat != null)
                {
                    var access = AccessHelper.GetAccess(task.AccessId);
                    access.Views = stat.Views;
                    access.Messages = stat.Messages;
                    access.Calls = stat.Calls;
                    AccessHelper.Save(access);
                }
            }, null, 100);
        }

        static void TestCollectMessages()
        {
            var avito = new Avito();
            var task = new TaskMessageCollectorCache
            {
                AccessId = new Guid("7D4D06F2-8C3A-4E7B-82A2-A815427BB7A2"),
                Login = "sasha.shleif@mail.ru",
                Password = "OnlineHelp59"
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                var msgs = avito.CollectMessages(browser, task);
                if (msgs.Any())
                {
                    var access = AccessHelper.GetAccess(task.AccessId);
                    access.LastMessage = msgs.Max(q => q.PublicationDate);
                    AccessHelper.Save(access);
                    MessageServiceHelper.Save(msgs);
                }
            }, null, 100);
        }

        static void TestInstanceStatistics()
        {
            var avito = new Avito();
            var task = new TaskInstanceStatisticsCache
            {
                InstanceId = new Guid("8BB27650-462E-497E-B9A5-044AA1C66FAD"),
                Url = "https://www.avito.ru/podolsk/telefony/iphone_6s_64gb_seryy_kosmos_1259355638",
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                var stat = avito.GetInstanceStatistics(browser, task);
                if (stat != null)
                {
                    var instance = BulletinInstanceHelper.Get(task.InstanceId);
                    instance.Views = stat.Value;
                    BulletinInstanceHelper.Save(instance);
                }
            }, null, 100);
        }


        static void TestForwardNumber()
        {
            var avito = new Avito();
            var accessCache = new AccessCache
            {
                IsForwarding = true,
                Login = "Valera.Shleif.Classic@mail.ru",
                Password = "OnlineHelp59",
            };
            FirefoxHelper.ExecuteWithVisual(browser =>
            {
                browser.Navigate().GoToUrl("https://www.avito.ru/moskva/bytovaya_elektronika");
                avito.Registration(browser, accessCache);
            }, null, 100, true);
            //const string forwardNumber = "9264200230";
            //OnlineSimHelper.GetForward(forwardNumber);
        }

    }
}