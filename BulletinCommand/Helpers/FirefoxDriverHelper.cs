using System;
using CollectorModels;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace BulletinCommand.Helpers
{
    public static class FirefoxDriverHelper
    {
        public static void ExecuteWithVisual(Action<FirefoxDriver> executeAction, ProxyCardCheckCache proxyCache = null, int Timeout = 20)
        {
            DCT.Execute(c =>
            {
                FirefoxOptions options = new FirefoxOptions();
                if (proxyCache != null)
                {
                    var proxy = new Proxy();
                    proxy.HttpProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    proxy.FtpProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    proxy.SslProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    options.Proxy = proxy;
                }
                //options.AddArguments("--headless");
                options.Profile = new FirefoxProfile();
                options.Profile.SetPreference("dom.disable_beforeunload", true);
                options.Profile.SetPreference("dom.popup_maximum", 0);
                options.Profile.SetPreference("privacy.popups.showBrowserMessage", false);
                options.Profile.SetPreference("pdfjs.disabled", true);

                options.Profile.SetPreference("permissions.default.image", 2);
                options.Profile.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", "false");
                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = false;
                service.HideCommandPromptWindow = true;

                using (var currentDriver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(Timeout)))
                {
                    try
                    {
                        var manager = currentDriver.Manage();
                        manager.Timeouts().ImplicitWait = TimeSpan.FromSeconds(Timeout);
                        manager.Timeouts().PageLoad = TimeSpan.FromSeconds(Timeout);
                        executeAction?.Invoke(currentDriver);
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.SendMessage($"FIREFOXDRIVER CRASHED {Environment.NewLine} {ex.ToString()}");
                    }
                    try
                    {
                        currentDriver.Close();
                        currentDriver.Quit();
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.SendMessage($"FIREFOXDRIVER CLOSE AND QUIT CRASHED {Environment.NewLine} {ex.ToString()}");
                    }
                }
            }, continueExceptionMethod:(ex,c)=> { });
        }
        public static void ExecuteOne(Action<FirefoxDriver> executeAction, ProxyCardCheckCache proxyCache = null, int Timeout = 10)
        {
            DCT.Execute(c =>
            {
                FirefoxOptions options = new FirefoxOptions();
                if (proxyCache != null)
                {
                    var proxy = new Proxy();
                    proxy.HttpProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    proxy.FtpProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    proxy.SslProxy = $"{proxyCache.Address}:{proxyCache.Port}";
                    options.Proxy = proxy;
                }
                options.AddArguments("--headless");
                options.Profile = new FirefoxProfile();
                options.Profile.SetPreference("dom.disable_beforeunload", true);
                options.Profile.SetPreference("dom.popup_maximum", 0);
                options.Profile.SetPreference("privacy.popups.showBrowserMessage", false);
                options.Profile.SetPreference("pdfjs.disabled", true);

                options.Profile.SetPreference("permissions.default.image", 2);
                options.Profile.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", "false");
                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = false;
                service.HideCommandPromptWindow = true;

                using (var currentDriver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(Timeout)))
                {
                    try
                    {
                        var manager = currentDriver.Manage();
                        manager.Timeouts().ImplicitWait = TimeSpan.FromSeconds(Timeout);
                        manager.Timeouts().PageLoad = TimeSpan.FromSeconds(Timeout);
                        executeAction?.Invoke(currentDriver);
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.SendMessage($"FIREFOXDRIVER CRASHED {Environment.NewLine} {ex.ToString()}");
                    }
                    try
                    {
                        currentDriver.Close();
                        currentDriver.Quit();
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.SendMessage($"FIREFOXDRIVER CLOSE AND QUIT CRASHED {Environment.NewLine} {ex.ToString()}");
                    }
                }
            });
        }

        public static string GetPage(string URL)
        {
            var result = "";
            ExecuteOne(driver =>
            {
                driver.Navigate().GoToUrl(URL);
                result = driver.PageSource;
            });
            return result;
        }

    }
}