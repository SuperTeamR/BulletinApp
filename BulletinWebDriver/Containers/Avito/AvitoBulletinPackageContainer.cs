using BulletinBridge.Data;
using BulletinWebDriver;
using BulletinWebDriver.ServiceHelper;
using BulletinWebDriver.Tools;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using FessooFramework.Tools.DCT;
using FessooFramework.Tools.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{

    class AvitoBulletinPackageContainer : BulletinPackageContainerBase
    {
        public override Guid Uid => BoardIds.Avito;

        public string ProfileUrl => @"https://www.avito.ru/profile";

        public override void AddBulletins(IEnumerable<TaskCache> tasks)
        {

            DCT.Execute(d =>
            {
                var accessContainer = AccessContainerList.Get(Uid);

                foreach (var task in tasks)
                {
                    var bulletin = task.BulletinPackage;
                    if (accessContainer.TryAuth(bulletin.Access))
                    {
                        PrepareBulletin(bulletin);
                        ContinueAddOrEdit(EnumHelper.GetValue<BulletinState>(bulletin.State));
                        Publicate(bulletin);
                        GetUrl(bulletin);
                    }
                }
                DCT.ExecuteAsync(d2 =>
                {
                    foreach (var task in tasks)
                    {
                        var bulletin = task.BulletinPackage;
                        if (string.IsNullOrEmpty(bulletin.Url))
                        {
                            bulletin.State = (int)BulletinState.Error;
                            task.State = (int)TaskCacheState.Error;
                        }
                        else
                        {
                            bulletin.State = (int)BulletinState.OnModeration;
                            task.State = (int)TaskCacheState.Completed;
                        }
                    }
                    ServerHelper.SendDoneTasks(tasks);
                });
            });
        }

        void PrepareBulletin(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                WebDriver.UpdateActions();
                WebDriver.NavigatePage("https://www.avito.ru/additem");
                Thread.Sleep(2000);
                ChooseCategories(bulletin.Signature);

                if (!SetValueFields(bulletin))
                {
                    PrepareBulletin(bulletin);
                }
            }, continueExceptionMethod: (d, e) => 
            {
                PrepareBulletin(bulletin);
            });
        }

        
        void ChooseCategories(GroupSignature signature)
        {
            DCT.Execute(d =>
            {
               foreach(var category in signature.GetCategories())
               {
                    if (!string.IsNullOrEmpty(category))
                    {
                        WebDriver.JsClick(By.CssSelector($"input[title='{category}']"));
                        Thread.Sleep(1000);
                    }
                        
               }
            });
        }

        bool SetValueFields(BulletinPackage bulletin)
        {
            var result = false;
            DCT.Execute(d =>
            {
                var accessFiels = bulletin.AccessFields;
                var valueFields = bulletin.ValueFields;
                var bulletinTypeField = accessFiels["Вид объявления "];
                var bulletinTypeOption = bulletinTypeField.Options.FirstOrDefault(q => q.Text == "Продаю свое");

                var bulletinTypeCode = bulletinTypeOption.Value;
                var bulletinTitleCode = accessFiels["Название объявления"].HtmlId;
                var bulletinDescCode = accessFiels["Описание объявления"].HtmlId;
                var bulletinPriceCode = accessFiels["Цена"].HtmlId;
                var bulletinImageCode = accessFiels["Фотографии"].HtmlId;

                var bulletinTitleText = valueFields["Название объявления"];
                var bulletinDescText = valueFields["Описание объявления"];
                var bulletinPriceText = valueFields["Цена"];
                var bulletinImageText = valueFields.ContainsKey("Фотографии") ? valueFields["Фотографии"] : string.Empty;

                WebDriver.JsClick(By.CssSelector($"input[value='{bulletinTypeCode}']"));
                WebDriver.DoAction(By.CssSelector($"input[id='{bulletinTitleCode}']"), e => e.SendKeys(bulletinTitleText));
                WebDriver.DoAction(By.CssSelector($"textarea[id='{bulletinDescCode}']"), e => e.SendKeys(bulletinDescText));
                WebDriver.DoAction(By.CssSelector($"input[id='{bulletinPriceCode}']"), e => e.SendKeys(bulletinPriceText));

                if(!string.IsNullOrEmpty(bulletinImageText))
                {
                    var images = bulletinImageText.Split(new[] { "\r\n" }, StringSplitOptions.None);

                    foreach (var image in images)
                    {
                        if (!WebDriver.Wait(WebDriver.NoAttribute(By.CssSelector($"input[name='{bulletinImageCode}']"), "disabled"), 20))
                            return;

                        WebDriver.JsClick(By.CssSelector($"input[name='{bulletinImageCode}']"),
                         (e) =>
                         {
                             Thread.Sleep(1000);
                             SendKeys.SendWait(image);
                         });
                        SendKeys.SendWait("{ENTER}");
                        Thread.Sleep(10000);

                        if (!WebDriver.Wait(WebDriver.ElementExists(By.CssSelector($"input[name='{bulletinImageCode}']")), 20))
                            return;
                    }

                    var addedImagesCount = WebDriver.FindMany(By.ClassName("form-uploader-item")).Where(q => q.GetAttribute("data-state") == "active").Count();

                    if (addedImagesCount != images.Length)
                        return;
                }

                result = true;
            });
            if(!result)
            {
                SendKeys.SendWait("{ESC}");
            }
            return result;
            
        }

        void ContinueAddOrEdit(BulletinState state)
        {
            DCT.Execute(d =>
            {
                if(state == BulletinState.Edited)
                {
                 
                }
                else if (state == BulletinState.WaitPublication || state == BulletinState.WaitRepublication || state == BulletinState.Created)
                {
                    //WebDriver.DoClick(By.Id("pack1"));
                    var button = WebDriver.FindMany(By.ClassName("button-origin")).FirstOrDefault(q => q.Text == "Продолжить с пакетом «Обычная продажа»");
                    WebDriver.JsClick(button);
                }
                else
                {
                 
                }
               

            });
        }

        void Publicate(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                if(bulletin.State == (int)BulletinState.WaitPublication || bulletin.State == 0)
                {
                    WebDriver.NavigatePage("https://www.avito.ru/additem/confirm");
                }
                else if (bulletin.State == (int)BulletinState.Edited)
                {
                    
                }

                //Снимаем галочки
                WebDriver.JsClick(By.Id("service-premium"));
                WebDriver.JsClick(By.Id("service-vip"));
                WebDriver.JsClick(By.Id("service-highlight"));

                //Подтверждаем
                var button = WebDriver.FindMany(By.TagName("button")).FirstOrDefault(q => q.Text == "Продолжить");
                WebDriver.JsClick(button);
            });
        }
        
        void GetUrl(BulletinPackage bulletin)
        {
            DCT.Execute(d =>
            {
                var a = WebDriver.Find(By.XPath("//*[@class='content-text']/p/a"));
                var href = a.GetAttribute("href");
                bulletin.Url = href;
            });
        }


        public void CheckModerationState(IEnumerable<TaskCache> tasks)
        {
            DCT.Execute(d =>
            {


            });
        }
        BulletinState CheckBulletinState(string url)
        {
            var result = BulletinState.Error;

            DCT.Execute(d =>
            {
                WebDriver.NavigatePage(ProfileUrl);

                var tabStates = new List<TabState>();
                var tabs = WebDriver.FindMany(By.ClassName("tabs-item"));
                tabStates.Add(new TabState
                {
                    Title = "Активные",
                    Href = ProfileUrl,
                });

                foreach(var tab in tabs)
                {
                    var a = tab.FindElement(By.XPath("/a"));
                    var tabUrl = a.GetAttribute("href");
                    var tabState = a.Text;

                    if (tabState == "Удаленные") continue;

                    tabStates.Add(new TabState
                    {
                        Title = tabState,
                        Href = tabUrl,
                    });
                }

            });
            return result;
        }
    }
}
