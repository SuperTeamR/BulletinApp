using BulletinWebDriver.Tools;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinWebDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            DCT.Execute(d =>
            {
                Bootstrapper.Current.Run();

                //TestConnection();

                WebWorkerManager.Execute();
                Console.ReadLine();
            });
        }

        static void TestConnection()
        {
            try
            {
                WebDriver.NavigatePage("http://yandex.ru");

                Thread.Sleep(2000);


                WebDriver.NavigatePage("https://www.avito.ru/profile/login?next=%2Fprofile");

                WebDriver.DoAction(By.Name("login"), e => e.SendKeys("Slava.Shleif@gmail.com"));
                WebDriver.DoAction(By.Name("password"), e => e.SendKeys("OnlineHelp59"));

                WebDriver.DoAction(By.ClassName("login-form-submit"), e => e.Click());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Невалидный прокси");
                WebDriver.RestartDriver();
                TestConnection();
            }
          
        }
    }
}
