using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base;
using BulletinWebWorker.Containers.Base.Access;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{
    internal class AvitoBulletinPackageContainer : BulletinPackageContainerBase
    {
        public override Guid Uid => BoardIds.Avito;

        public override void AddBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);
                    foreach (var bulletin in packages)
                    {
                        if (accessContainer.TryAuth(bulletin.Access))
                        {
                            //Стадия заполнения
                            WebWorker.NavigatePage("https://www.avito.ru/additem");
                            //1
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category1))
                            {
                                var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                               .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category1);
                                if (categoryRadio == null) return;
                                categoryRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //2
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category2))
                            {
                                var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category2);
                                if (serviceRadio == null) return;
                                serviceRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //3
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category3))
                            {
                                var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category3);
                                if (serviceTypeRadio == null) return;
                                serviceTypeRadio.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //4
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category4))
                            {
                                var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category4);
                                if (serviceTypeRadio2 == null) return;
                                serviceTypeRadio2.InvokeMember("click");
                                Thread.Sleep(1000);
                            }
                            //5
                            if (!string.IsNullOrEmpty(bulletin.Signature.Category5))
                            {
                                var serviceTypeRadio3 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == bulletin.Signature.Category5);
                                if (serviceTypeRadio3 == null) return;
                                serviceTypeRadio3.InvokeMember("click");
                                Thread.Sleep(1000);
                            }

                            foreach (var pair in bulletin.ValueFields)
                            {
                                var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                                fieldValueContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                            }

                            //Продолжить с пакетом «Обычная продажа»
                            var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "pack1");
                            if (radioButton != null) radioButton.InvokeMember("click");


                            var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                            var pack = "Продолжить с пакетом «Обычная продажа»";
                            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                            if (button != null)
                                button.InvokeMember("click");

                            Thread.Sleep(1000);


                            //Стадия публикации
                            WebWorker.NavigatePage("https://www.avito.ru/additem/confirm");
                            //Снимаем галочки
                            var servicePremium = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-premium");
                            if (servicePremium != null)
                                servicePremium.InvokeMember("click");
                            var serviceVip = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-vip");
                            if (serviceVip != null)
                                serviceVip.InvokeMember("click");
                            var serviceHighlight = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "service-highlight");
                            if (serviceHighlight != null)
                                serviceHighlight.InvokeMember("click");

                            //Подтверждаем
                            var text = "Продолжить";
                            var buttonContinue = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>().FirstOrDefault(btn => btn.InnerText == text);
                            if (buttonContinue != null)
                                buttonContinue.InvokeMember("click");
                        }
                    }
                });
            });
        }

        public override void EditBulletins(IEnumerable<BulletinPackage> packages)
        {
            DCT.Execute(d =>
            {
                WebWorker.Execute(() =>
                {
                    var fieldValueContainer = FieldValueContainerList.Get(Uid);
                    var accessContainer = AccessContainerList.Get(Uid);

                    foreach (var bulletin in packages)
                    {
                        WebWorker.NavigatePage(bulletin.Url);

                        foreach (var pair in bulletin.ValueFields)
                        {
                            var template = bulletin.AccessFields.FirstOrDefault(q => q.Key == pair.Key);
                            fieldValueContainer.SetFieldValue(bulletin.AccessFields, template.Key, pair.Value);
                        }

                        if (bulletin.State == (int)BulletinState.Edited)
                        {
                            //Продолжить с пакетом «Обычная продажа»
                            var radioButton = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("id") == "pack1");
                            if (radioButton != null) radioButton.InvokeMember("click");

                            var buttons = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>();
                            var pack = "Продолжить без пакета";
                            var button = buttons.FirstOrDefault(btn => btn.InnerText == pack);
                            if (button != null)
                                button.InvokeMember("click");
                        }
                        else
                        {
                            var button = WebWorker.WebDocument.GetElementsByTagName("button").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "submit" && q.InnerText == "Продолжить");

                            if (button != null)
                                button.InvokeMember("click");

                        }
                    }
                });
            });
        }
    }
}
