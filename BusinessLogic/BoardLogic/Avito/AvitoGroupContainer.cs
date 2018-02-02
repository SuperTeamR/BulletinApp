///-------------------------------------------------------------------------------------------------
// file:	BoardLogic\Avito\AvitoGroupContainer.cs
//
// summary:	Implements the avito group container class
///-------------------------------------------------------------------------------------------------

using BusinessLogic.BoardLogic.Base;
using BusinessLogic.BoardLogic.Base.FieldParser;
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
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   An avito group container. </summary>
    ///
    /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal class AvitoGroupContainer : GroupContainerBase
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the fields. </summary>
        ///
        /// <value> The fields. </value>
        ///-------------------------------------------------------------------------------------------------

        Dictionary<string, FieldPackage> fields { get; set; } = new Dictionary<string, FieldPackage>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the next stage. </summary>
        ///
        /// <value> The next stage. </value>
        ///-------------------------------------------------------------------------------------------------

        ParsingBlockStage nextStage { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the field parser. </summary>
        ///
        /// <value> The field parser. </value>
        ///-------------------------------------------------------------------------------------------------

        FieldParserContainerBase fieldParser { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Default constructor. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        public AvitoGroupContainer()
        {
            fieldParser = FieldParserContainerList.Get(Uid);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем Group по хэшу. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="hash"> Хэш на основе города и категорий. </param>
        ///
        /// <returns>   A Data.Group. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override Data.Group Get(string hash)
        {
            var group = default(Data.Group);
            _DCT.Execute(data =>
            {
                var xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "groups7.xml"));
                var rawGroups = DataSerializer.Deserialize<IEnumerable<Data.Group>>(xml).ToList();
                group = rawGroups.FirstOrDefault(q => q.GetHash() == hash);

            });
            return group;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем все группы. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

        public override IEnumerable<Data.Group> GetAll()
        {
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Выгружаем дерево категорий и конвертируем в группы 1. Рекурсивное чтение дерева категорий 2.
        /// Формирование списка группа из распарсенного дерева категорий 3. Для каждой группы рекурсивный
        /// поиск и сохранение полей.
        /// </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process initialize in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------

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
                    //    if(node.Name == "Хобби и отдых")
                    //    {

                    //    }
                    //    LoadCategoryTrees(node, elementValue);
                    //}
                    //var rawGroups = tree.ToGroupCollection().ToList();

                    //var sGroups = DataSerializer.Serialize(rawGroups);

                    var xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "groups5.xml"));
                    var rawGroups = DataSerializer.Deserialize<IEnumerable<Data.Group>>(xml).ToList();

                    var dictionary = new Dictionary<string, int>();
                    foreach (var group in rawGroups)
                    {
                        foreach (var f in group.Fields)
                        {
                            if (dictionary.ContainsKey(f.Key))
                            {
                                dictionary[f.Key]++;
                            }
                            else
                            {
                                dictionary.Add(f.Key, 1);
                            }
                        }
                    }
                    var list = dictionary.ToList();
                    list.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                    var csv = string.Join(Environment.NewLine, list.ToArray());
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

                        fields.Clear();
                        ParseMainComponent();
                        ParseLocationComponent();
                        ParseComissionComponent();
                        ParsePhotoComponent();
                        ParseVideoComponent();
                        ParseParametersComponent();
                        group.Fields = new Dictionary<string, FieldPackage>(fields);
                    }

                    result = rawGroups;

                    var sResult = DataSerializer.Serialize(result);
                }
            });
            return result;
        }


        /// <summary>   List of identifiers for the checked. </summary>
        List<string> checkedIds = new List<string>();

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Loads category trees. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="parentNode">   The parent node. </param>
        /// <param name="parentValue">  The parent value. </param>
        /// <param name="prevValue">    (Optional) The previous value. </param>
        ///-------------------------------------------------------------------------------------------------

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
                else if(byDataId)
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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse comission component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseComissionComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__commission"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse location component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseLocationComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__location"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse main component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseMainComponent()
        {
            _DCT.Execute(data =>
            {
                //Title
                var nameLabel = GetByTag("label", q => q.InnerText == "Название объявления");
                var nameInput = GetByTag("input", q => q.GetAttribute("id") == "item-edit__title");
                fields.Add(nameLabel.InnerText, FieldPackage.Create("item-edit__title", "input", true));

                //Description
                var descLabel = GetByTag("label", q => q.InnerText == "Описание объявления");
                var descInput = GetByTag("textarea", q => q.GetAttribute("id") == "item-edit__description");
                fields.Add(descLabel.InnerText, FieldPackage.Create("item-edit__description", "textarea", true));
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse parameters component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseParametersComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__parameters"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse photo component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParsePhotoComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__photos")
                 && !q.GetAttribute("className").Contains("is-hidden"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse video component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseVideoComponent()
        {
            _DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__video"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets by tag. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="tag">      The tag. </param>
        /// <param name="query">    The query. </param>
        ///
        /// <returns>   Html element. </returns>
        ///-------------------------------------------------------------------------------------------------

        HtmlElement GetByTag(string tag, Func<HtmlElement, bool> query)
        {
            var result = default(HtmlElement);
            _DCT.Execute(data =>
            {
                result = WebWorker.WebDocument.GetElementsByTagName(tag).Cast<HtmlElement>().FirstOrDefault(query);
            });
            return result;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Handles the block. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="element">      The element. </param>
        /// <param name="labelClass">   The label class. </param>
        /// <param name="hiddenClass">  The hidden class. </param>
        ///-------------------------------------------------------------------------------------------------

        void HandleBlock(HtmlElement element, string labelClass, string hiddenClass)
        {
            _DCT.Execute(data =>
            {
                nextStage = ParsingBlockStage.None;

                var dictionary = new Dictionary<string, FieldPackage>();
                var currentLabel = string.Empty;

                HandleElement(element, currentLabel, labelClass, hiddenClass);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Handles the element. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="element">      The element. </param>
        /// <param name="currentLabel"> The current label. </param>
        /// <param name="labelClass">   The label class. </param>
        /// <param name="hiddenClass">  The hidden class. </param>
        ///-------------------------------------------------------------------------------------------------

        void HandleElement(HtmlElement element, string currentLabel, string labelClass, string hiddenClass)
        {
            if (!element.CanHaveChildren) return;

            _DCT.Execute(data =>
            {
                foreach (HtmlElement ch in element.Children)
                {
                    if (ch.GetAttribute("className").Contains(hiddenClass)) continue;
                    if (nextStage == ParsingBlockStage.None || nextStage == ParsingBlockStage.Label)
                    {
                        if (ch.GetAttribute("className").Contains(labelClass))
                        {
                            if (ch.CanHaveChildren && ch.Children.Count > 0 && string.IsNullOrEmpty(ch.InnerText))
                            {
                                var span = ch.Children[0];
                                currentLabel = span.InnerText;
                            }
                            else
                            {
                                currentLabel = ch.InnerText;
                            }
                            nextStage = ParsingBlockStage.Input;
                        }
                    }
                    else
                    {
                        var fieldPackage = fieldParser.Parse(currentLabel, ch);
                        if(fieldPackage != null)
                        {
                            nextStage = ParsingBlockStage.Label;
                            fields.Add(currentLabel, fieldPackage);
                        }
                    }
                    HandleElement(ch, currentLabel, labelClass, hiddenClass);
                }
            });
        }
    }
}
