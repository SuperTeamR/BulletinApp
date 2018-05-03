using System;
using OpenQA.Selenium;

namespace BulletinCommand.Extensions
{
    public static class WebElementExtensions
    {
        public static IWebElement FindElementSafe(this IWebDriver driver, By by)
        {
            try
            {
                return driver.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static IWebElement FindElementSafe(this IWebElement element, By by)
        {
            try
            {
                return element.FindElement(by);
            }
            catch (NoSuchElementException)
            {
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static bool Exists(this IWebElement element)
        {
            if (element == null)
            { return false; }
            return true;
        }
    }
}