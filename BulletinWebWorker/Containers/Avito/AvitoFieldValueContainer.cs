using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
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
                var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.HtmlId);
                if (form == null)
                {
                    //Временное рабочее решение после того, как борда поменяла у ряда категорий поле "Вид объявлений" с combobox на radiobutton
                    if (fieldPackage.Tag == "select")
                    {
                        if (fieldPackage.Options != null && fieldPackage.Options.Length > 0)
                        {

                            var code = Regex.Match(fieldPackage.HtmlId, @"\d+").Value;
                            form = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                               .Where(q => q.GetAttribute("name") == $"params[{code}]")
                               .FirstOrDefault(q => q.GetAttribute("checked") != null);
                            var formValue = form.GetAttribute("value");
                            var option = fieldPackage.Options.FirstOrDefault(q => q.Value == formValue);
                            if (option != null)
                                result = option.Text;

                        }
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
                var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.HtmlId);
                if (form == null)
                {
                    //Временное рабочее решение после того, как борда поменяла у ряда категорий поле "Вид объявлений" с combobox на radiobutton
                    if(fieldPackage.Tag == "select")
                    {
                        if (fieldPackage.Options != null && fieldPackage.Options.Length > 0)
                        {

                            var option = fieldPackage.Options.FirstOrDefault(q => q.Text == value);
                            var code = option.Value;
                            form = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                              .FirstOrDefault(q => q.GetAttribute("value") == code);
                            form.InvokeMember("click");
                        }

                    }
                    return;
                }


                if(!string.IsNullOrEmpty(name) && name.Contains("Фотографии"))
                {
                    var values = value.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    foreach(var v in values)
                    {
                        SetImage(form, v);
                        Thread.Sleep(6000);
                    }
                }
                else
                {
                    switch (fieldPackage.Tag)
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
        void SetImage(HtmlElement form, string value)
        {
            DCT.Execute(data =>
            {
                form.Focus();
                var sendKeyTask = Task.Delay(500).ContinueWith((_) =>
                {
                    SendKeys.SendWait(@value);
                    SendKeys.SendWait("{Enter}");
                });
                form.InvokeMember("click");
            });
        }
        void SetInput(HtmlElement form, string value)
        {
            DCT.Execute(data =>
            {
                form.SetAttribute("value", value);
            });
        }

        void SetSelect(HtmlElement form, string value, FieldPackage package)
        {
            DCT.Execute(data =>
            {
                if (package.Options != null && package.Options.Length > 0)
                {
                    var option = package.Options.FirstOrDefault(q => q.Text == value);
                    var code = option.Value;
                    form.SetAttribute("value", code);
                }
            });
        }
        void SetRadio(HtmlElement form, string value, FieldPackage package)
        {
            DCT.Execute(data =>
            {
                if (package.Options != null && package.Options.Length > 0)
                {
                    var option = package.Options.FirstOrDefault(q => q.Text == value);
                    var code = option.Value;
                    form.SetAttribute("value", code);
                }
            });
        }

    }
}
