using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using BulletinBridge.Models;
using BulletinCommand.Extensions;
using BulletinEngine.Core;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;

namespace BulletinCommand.Helpers
{
    public static class BulletinCollector
    {
        private static int maxPages = 10;
        //private static string imagePattern = "background-image: url\\(\"//([\\s,\\S,\\n].*?)\"\\);";
        private static string imagePattern = "background-image:\\surl\\(\\\"(.+?)\\\"\\)";
        public static IEnumerable<BulletinSheetCache> GetBulletinsByQuery(string initialUrl, string query)
        {
            var bulletins = new List<BulletinSheetCache>();
            BCT.Execute(d =>
            {
                var regex = new Regex(imagePattern);
                FirefoxDriverHelper.ExecuteWithVisual(b =>
                {
                    for (var i = 0; i < maxPages; i++)
                    {
                        var url = string.Format($"{initialUrl}?q={query}");
                        if (i > 0)
                        {
                            if (b.Url.Contains("telefony"))
                            {

                                url = string.Format($"{initialUrl}//moskva/telefony/{query.ToLower()}?p={i + 1}&sgtd=8");
                            }
                            else
                            {

                                url = string.Format($"{initialUrl}?p={i + 1}&q={query}&sgtd=8");
                            }
                        }

                        b.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);
                        b.Navigate().GoToUrl(url);
                        Thread.Sleep(5000);
                        b.Manage().Timeouts().ImplicitWait = new TimeSpan(0);

                        var elements = b.FindElementsByClassName("item_table");
                        foreach (var e in elements)
                        {
                            var descBlock = e.FindElement(By.ClassName("item-description-title-link"));
                            var bulletinUrl = descBlock.GetAttribute("href");
                            var bulletinTitle = descBlock.Text;

                            var priceBlock = e.FindElement(By.ClassName("about"));
                            var bulletinPrice = priceBlock.Text;
                            var dateBlock = e.FindElement(By.ClassName("c-2"));
                            var bulletinDate = dateBlock.Text;
                            var bulletinImages = new List<string>();
                            if (e.FindElementSafe(By.CssSelector("a.photo-wrapper")).Exists())
                            {
                                var wrapper = e.FindElement(By.CssSelector("a.photo-wrapper"));
                                var img = wrapper.FindElementSafe(By.TagName("img"));
                                if (img != null)
                                {
                                    var src = img.GetAttribute("src");
                                    bulletinImages.Add(src);
                                }
                            }
                            else
                            {
                                if (e.FindElementSafe(By.XPath(".//div[contains(@class, 'item-slider-image')]")).Exists())
                                {
                                    var imageBlocks = e.FindElements(By.XPath(".//div[contains(@class, 'item-slider-image')]"));
                                    if (imageBlocks.Count > 0)
                                    {
                                        var srcs = imageBlocks.Select(q => q.GetAttribute("data-srcpath")).ToList();
                                        var srcIsNull = srcs.FirstOrDefault() == null;
                                        if (srcIsNull)
                                        {

                                            var rawUrls = imageBlocks.Select(q => q.GetAttribute("style")).ToList();
                                            foreach (var rawUrl in rawUrls)
                                            {
                                                Match match = regex.Match(rawUrl);
                                                if (match.Success)
                                                {
                                                    var handledUrl = match.Groups[1].Value;
                                                    handledUrl = handledUrl.Replace("\"", "");
                                                    bulletinImages.Add("http:" + handledUrl);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bulletinImages = srcs.Select(q => "http:" + q).ToList();
                                        }
                                    }
                                }
                            }

                            var cache = new BulletinSheetCache
                            {
                                Url = bulletinUrl,
                                Title = bulletinTitle,
                                Price = bulletinPrice,
                                Images = bulletinImages.ToArray(),
                            };
                            bulletins.Add(cache);
                        }
                    }
                }, Timeout: 10);
            });
            return bulletins;
        }


        public static IEnumerable<BulletinSheetCache> GetBulletinsBySheets(IEnumerable<BulletinSheetCache> caches)
        {
            BCT.Execute(d =>
            {
                FirefoxDriverHelper.ExecuteWithVisual(b =>
                {
                    foreach (var cache in caches)
                    {
                        if (cache.IsHandled) continue;

                        b.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 10);

                        b.Navigate().GoToUrl(cache.Url);
                        Thread.Sleep(3000);

                        b.Manage().Timeouts().ImplicitWait = new TimeSpan(0, 0, 0);
                        var bulletinDescription = string.Empty;
                        var descriptionBlock = b.FindElementSafe(By.ClassName("item-description-html"));
                        if (descriptionBlock.Exists())
                        {
                            bulletinDescription = descriptionBlock.Text;
                        }
                        else
                        {
                            var simpleDescriptionBlock = b.FindElementSafe(By.ClassName("item-description-text"));
                            if (simpleDescriptionBlock.Exists())
                            {
                                bulletinDescription = simpleDescriptionBlock.Text;
                            }
                        }

                        var bulletinViews = 0;
                        var viewBlock = b.FindElementSafe(By.ClassName("title-info-views"));
                        if (viewBlock != null)
                        {
                            bulletinViews = Int32.Parse(Regex.Match(viewBlock.Text, "\\d+").Value);
                        }

                        var categoriesBlocks = b.FindElementsByClassName("js-breadcrumbs-link").ToArray();
                        var rawCategories = new string[5];

                        //Skip city in category block
                        for (var i = 1; i < categoriesBlocks.Length; i++)
                        {
                            var categoryElement = categoriesBlocks[i];
                            rawCategories[i - 1] = categoryElement.Text;
                        }
                        cache.Category1 = rawCategories[0];
                        cache.Category2 = rawCategories[1];
                        cache.Category3 = rawCategories[2];
                        cache.Category4 = rawCategories[3];
                        cache.Category5 = rawCategories[4];

                        cache.Views = bulletinViews;
                        cache.Description = bulletinDescription;
                        cache.IsHandled = true;
                    }
                }, Timeout: 10);
            });
            return caches;
        }
    }
}