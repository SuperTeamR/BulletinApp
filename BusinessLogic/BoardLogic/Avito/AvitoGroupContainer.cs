using BusinessLogic.BoardLogic.Base;
using BusinessLogic.BoardLogic.Enums;
using BusinessLogic.Data;
using BusinessLogic.Tools;
using CommonTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Group
{
    internal class AvitoGroupContainer : GroupContainerBase
    {
        Dictionary<string, FieldPackage> Fields { get; set; } = new Dictionary<string, FieldPackage>();
        ParsingBlockStage NextStage { get; set; }

        public override Data.Group Get()
        {
            return null;
        }

        public override IEnumerable<Data.Group> GetAll()
        {
            return null;
        }

        /// <summary>
        /// Выгружаем дерево категорий и конвертируем в группы
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<Data.Group> Initialize()
        {
            List<Data.Group> result = new List<Data.Group>();
            _DCT.Execute(data =>
            {
                if (WebWorker.WebDocument != null)
                {
                    //var tree = new List<CategoryTree>();
                    //var root = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                    //    .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("name") == "root_category_id");

                    //foreach (var element in root)
                    //{
                    //    var elementValue = element.GetAttribute("value");
                    //    var node = new CategoryTree(element.GetAttribute("title"));
                    //    tree.Add(node);
                    //    element.InvokeMember("click");
                    //    Thread.Sleep(1000);
                    //    LoadCategoryTrees(node, elementValue);
                    //}
                    //var rawGroups = tree.ToGroupCollection().ToList();

                    //var sGroups = DataSerializer.Serialize(rawGroups);

                    var xml = File.ReadAllText(@"D:\Projects\Job\BulletinApp\BulletinApp\WebBrowserHostTest\bin\Debug\groups.xml");
                    var rawGroups = DataSerializer.Deserialize<IEnumerable<Data.Group>>(xml).ToList();

                    //var dictionary = new Dictionary<string, int>();
                    //foreach (var group in rawGroups)
                    //{
                    //    foreach (var f in group.Fields)
                    //    {
                    //        if (dictionary.ContainsKey(f.Key))
                    //        {
                    //            dictionary[f.Key]++;
                    //        }
                    //        else
                    //        {
                    //            dictionary.Add(f.Key, 1);
                    //        }
                    //    }
                    //}
                    //dictionary.OrderByDescending(q => q.Value);
                    //rawGroups = rawGroups.Where(q => q.Category1 == "Услуги").ToList();
                    foreach (var group in rawGroups)
                    {
                        Console.WriteLine(group);

                        //1
                        if (!string.IsNullOrEmpty(group.Category1))
                        {
                            var categoryRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                           .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category1);
                            if (categoryRadio == null) return;
                            categoryRadio.InvokeMember("click");
                        }
                        Thread.Sleep(1000);
                        //2
                        if (!string.IsNullOrEmpty(group.Category2))
                        {
                            var serviceRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category2);
                            if (serviceRadio == null) return;
                            serviceRadio.InvokeMember("click");
                        }
                        Thread.Sleep(1000);
                        //3
                        if (!string.IsNullOrEmpty(group.Category3))
                        {
                            var serviceTypeRadio = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category3);
                            if (serviceTypeRadio == null) return;
                            serviceTypeRadio.InvokeMember("click");
                        }
                        Thread.Sleep(1000);
                        //4
                        if (!string.IsNullOrEmpty(group.Category4))
                        {
                            var serviceTypeRadio2 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category4);
                            if (serviceTypeRadio2 == null) return;
                            serviceTypeRadio2.InvokeMember("click");
                        }
                        Thread.Sleep(1000);
                        //5
                        if (!string.IsNullOrEmpty(group.Category5))
                        {
                            var serviceTypeRadio3 = WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("type") == "radio" && q.GetAttribute("title") == group.Category5);
                                if (serviceTypeRadio3 == null) return;
                            serviceTypeRadio3.InvokeMember("click");
                        }

                        Thread.Sleep(3000);

                        

                        Fields.Clear();
                        ParseMainComponent();
                        ParseLocationComponent();
                        ParseComissionComponent();
                        //ParsePhotoComponent();
                        ParseVideoComponent();
                        ParseParametersComponent();
                        group.Fields = new Dictionary<string, FieldPackage>(Fields);
                    }

                    result = rawGroups;

                    var sResult = DataSerializer.Serialize(result);
                }
            });
            return result;
        }


        List<string> checkedIds = new List<string>();
        void LoadCategoryTrees(CategoryTree parentNode, string parentValue, string prevValue = null)
        {
            _DCT.Execute(data =>
            {
                var byDataId = false;

                if (string.IsNullOrEmpty(parentValue))
                {
                    var div = WebWorker.WebDocument.GetElementsByTagName("div").Cast<HtmlElement>()
                        .LastOrDefault(q => q.GetAttribute("className") == "form-category js-form-category_param");
                    if (div != null)
                    {
                        parentValue = div.GetAttribute("data-param-id");
                        byDataId = true;
                    }
                }
                if (string.IsNullOrEmpty(parentValue) || parentValue == prevValue) return;

                if (!checkedIds.Contains(parentValue))
                    checkedIds.Add(parentValue);
                else
                    return;


                var level = byDataId
                    ? WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("name") == $"params[{parentValue}]")
                    : WebWorker.WebDocument.GetElementsByTagName("input").Cast<HtmlElement>()
                        .Where(q => q.GetAttribute("type") == "radio" && q.GetAttribute("data-parent-id") == parentValue);

                foreach (var element in level)
                {
                    //var elementValue = element.GetAttribute("value");
                    var node = new CategoryTree(element.GetAttribute("title"));
                    parentNode.AddChild(node);

                    element.InvokeMember("click");
                    Thread.Sleep(1000);
                    LoadCategoryTrees(node, null, parentValue);
                }
            });
        }


        void ParseComissionComponent()
        {
            _DCT.Execute(data =>
            {
                //Title
                var nameLabel = GetByTag("label", q => q.InnerText == "Название объявления");
                var nameInput = GetByTag("input", q => q.GetAttribute("id") == "item-edit__title");
                Fields.Add(nameLabel.InnerText, FieldPackage.Create("item-edit__title", "input", true));

                //Description
                var descLabel = GetByTag("label", q => q.InnerText == "Описание объявления");
                var descInput = GetByTag("textarea", q => q.GetAttribute("id") == "item-edit__description");
                Fields.Add(descLabel.InnerText, FieldPackage.Create("item-edit__description", "textarea", true));
            });
        }

        void ParseLocationComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__location"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label");
            });
        }

        void ParseMainComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__commission"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label");
            });
        }

        void ParseParametersComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__photos"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label");
            });
        }
        void ParsePhotoComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__video"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label");
            });
        }

        void ParseVideoComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__parameters"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label");
            });
        }


        HtmlElement GetByTag(string tag, Func<HtmlElement, bool> query)
        {
            return WebWorker.WebDocument.GetElementsByTagName(tag).Cast<HtmlElement>().FirstOrDefault(query);
        }
        void HandleBlock(HtmlElement element, string labelClass)
        {
            _DCT.Execute(data =>
            {
                NextStage = ParsingBlockStage.None;

                var dictionary = new Dictionary<string, FieldPackage>();
                var currentLabel = string.Empty;

                HandleElement(element, currentLabel, labelClass);
            });
        }
        void HandleElement(HtmlElement element, string currentLabel, string labelClass)
        {
            if (!element.CanHaveChildren) return;

            _DCT.Execute(data =>
            {
                foreach (HtmlElement ch in element.Children)
                {
                    if (NextStage == ParsingBlockStage.None || NextStage == ParsingBlockStage.Label)
                    {
                        if (ch.GetAttribute("className").Contains(labelClass))
                        {
                            currentLabel = ch.InnerText;
                            NextStage = ParsingBlockStage.Input;
                            var tagName = ch.TagName;
                        }
                    }
                    else
                    {
                        if (ch.TagName.ToLower() == "select" || ch.TagName.ToLower() == "input")
                        {
                            var id = ch.GetAttribute("id");
                            if (!string.IsNullOrEmpty(id))
                            {
                                Fields.Add(currentLabel, FieldPackage.Create(id, ch.TagName.ToLower(), true));
                            }
                            else
                            {
                                var name = ch.GetAttribute("name");
                                Fields.Add(currentLabel, FieldPackage.Create(name, ch.TagName.ToLower(), false));
                            }
                            NextStage = ParsingBlockStage.Label;
                        }
                    }
                    HandleElement(ch, currentLabel, labelClass);
                }
            });
        }

        public override FieldPackage GetFieldPackage(string key)
        {
            throw new NotImplementedException();
        }
    }
}
