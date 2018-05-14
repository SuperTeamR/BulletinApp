using System;
using BulletinClient.Core;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace BulletinClient.Helpers
{
    public static class FirefoxHelper
    {
        public static void ExecuteWithVisual(Action<FirefoxDriver> executeAction, int Timeout = 20, bool withImages = false)
        {
            DCT.Execute(c =>
            {
                FirefoxOptions options = new FirefoxOptions();
                //options.AddArguments("--headless");
                options.Profile = new FirefoxProfile();
                options.Profile.SetPreference("dom.disable_beforeunload", true);
                options.Profile.SetPreference("dom.popup_maximum", 0);
                options.Profile.SetPreference("privacy.popups.showBrowserMessage", false);
                options.Profile.SetPreference("pdfjs.disabled", true);

                if (!withImages)
                {
                    options.Profile.SetPreference("permissions.default.image", 2);
                    options.Profile.SetPreference("dom.ipc.plugins.enabled.libflashplayer.so", "false");
                }

                FirefoxDriverService service = FirefoxDriverService.CreateDefaultService();
                service.SuppressInitialDiagnosticInformation = false;
                service.HideCommandPromptWindow = true;


                try
                {
                    var currentDriver = new FirefoxDriver(service, options, TimeSpan.FromSeconds(Timeout));
                    var manager = currentDriver.Manage();
                    manager.Timeouts().ImplicitWait = TimeSpan.FromSeconds(Timeout);
                    manager.Timeouts().PageLoad = TimeSpan.FromSeconds(Timeout);
                    executeAction?.Invoke(currentDriver);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.SendMessage($"FIREFOXDRIVER CRASHED {Environment.NewLine} {ex.ToString()}");
                }
            
            }, continueExceptionMethod: (ex, c) => { });
        }
    }
}