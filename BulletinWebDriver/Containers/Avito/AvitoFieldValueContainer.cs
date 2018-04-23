using BulletinBridge.Data;
using BulletinWebDriver.Containers.Base.FieldValue;
using BulletinWebDriver.Tools;
using FessooFramework.Tools.DCT;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace BulletinWebDriver.Containers.Avito
{
    class AvitoFieldValueContainer : FieldValueContainerBase
    {
        public override Guid Uid => BoardIds.Avito;
        public override string GetFieldValue(Dictionary<string, FieldPackage> fields, string name)
        {
            var result = string.Empty;
            DCT.Execute(d =>
            {
                var fieldPackage = fields.FirstOrDefault(q => q.Key == name).Value;
                if (fieldPackage == null) return;

                var attribute = fieldPackage.HasId ? "id" : "name";
                var form = WebDriver.FindMany(By.TagName(fieldPackage.Tag)).FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.HtmlId);

                if(form == null)
                {
                    if(fieldPackage.Tag == "select"
                        && fieldPackage.Options != null && fieldPackage.Options.Length > 0)
                    {
                        var code = Regex.Match(fieldPackage.HtmlId, @"\d+").Value;
                        form = WebDriver.FindMany(By.TagName("input")).Where(q => q.GetAttribute("name") == $"params[]{code}").FirstOrDefault(q => q.GetAttribute("checked") != null);
                        var formValue = form.GetAttribute("value");
                        var option = fieldPackage.Options.FirstOrDefault(q => q.Value == formValue);
                        if (option != null)
                            result = option.Text;
                        
                    }
                    return;
                }

                if(fieldPackage.Tag == "select" && fieldPackage.Options != null)
                {
                    var selected = form.GetAttribute("value");
                    var optionTag = fieldPackage.Options.FirstOrDefault(q => q.Value == selected);
                    if (optionTag != null)
                        result = optionTag.Text;
                }
                else
                {
                    result = form.GetAttribute("value");
                }
            });
            return result;
        }
        public override void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value)
        {
            DCT.Execute(d =>
            {
                var fieldPackage = fields.FirstOrDefault(q => q.Key == name).Value;

                if (fieldPackage == null) return;

                var attribute = fieldPackage.HasId ? "id" : "name";
                var form = WebDriver.FindMany(By.TagName(fieldPackage.Tag)).FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.HtmlId);

                if(form == null)
                {
                    if(fieldPackage.Tag == "select" && fieldPackage.Options != null && fieldPackage.Options.Length > 0)
                    {
                        var option = fieldPackage.Options.FirstOrDefault(q => q.Text == value);
                        var code = option.Value;
                        form = WebDriver.FindMany(By.TagName("input")).FirstOrDefault(q => q.GetAttribute("value") == code);
                        form.Click();
                    }
                }
                if(!name.Contains("Фотографии"))
                {
                    switch(fieldPackage.Tag)
                    {
                        case "input":
                        case "textarea":
                            SetInput(form, value);
                            break;
                        case "select":
                            SetSelect(form, value, fieldPackage);
                            break;
                    }

                    Thread.Sleep(500);
                }
            });
        }
        void SetInput(IWebElement element, string value)
        {
            DCT.Execute(d =>
            {
                element.SendKeys(value);
            });
        }
        void SetSelect(IWebElement element, string value, FieldPackage package)
        {
            DCT.Execute(d =>
            {
                if (package.Options != null && package.Options.Length > 0)
                {
                    var options = package.Options.FirstOrDefault(q => q.Text == value);
                    var code = options.Value;
                    element.SendKeys(code);
                }
            });
        }
    }
}