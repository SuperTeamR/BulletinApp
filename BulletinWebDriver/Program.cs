using BulletinWebDriver.Tools;
using FessooFramework.Tools.DCT;
using System;

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
            WebDriver.NavigatePage("http://yandex.ru");
        }
    }
}