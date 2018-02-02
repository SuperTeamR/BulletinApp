using BulletinExample.Core;
using BulletinExample.Logic.Containers.Base.FieldParser;
using BulletinExample.Logic.Containers.Base.Group;
using BulletinExample.Logic.Data;
using BulletinExample.Tools;
using Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BulletinExample.Logic.Containers.Avito
{
    internal class AvitoGroupContainer : GroupContainerBase
    {
        public override Guid Uid => BoardIds.Avito;

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
        enum ParsingBlockStage
        {
            None = 0,
            Label = 1,
            Input = 2
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets or sets the field parser. </summary>
        ///
        /// <value> The field parser. </value>
        ///-------------------------------------------------------------------------------------------------

        FieldParserContainerBase fieldParser { get; set; }

        public AvitoGroupContainer()
        {
            fieldParser = FieldParserContainerList.Get(Uid);
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Получаем пакет группы с полям </summary>
        ///
        /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
        ///
        /// <param name="hash"> The hash. </param>
        ///
        /// <returns>   The group package. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override GroupPackage GetGroupPackage(string hash)
        {
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем сигнатуру группы (просто категории) </summary>
        ///
        /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
        ///
        /// <param name="hash"> The hash. </param>
        ///
        /// <returns>   The group signature. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override GroupSignature GetGroupSignature(string hash)
        {
            GroupSignature result = null;
            DCT.Execute(data =>
            {

                var group = data.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                if (group != null)
                {
                    var chosenCategories = new List<string>();
                    var groupedCategories = data.Db1.GroupedCategories.Where(q => q.GroupId == group.Id).Select(q => q.CategoryId).ToArray();
                    var categories = data.Db1.CategoryTemplates.Where(q => groupedCategories.Contains(q.Id)).ToArray();
                    var topCategory = categories.FirstOrDefault(q => q.ParentId == Guid.Empty);
                    var nothandledCategories = categories.Where(q => q.Id != topCategory.Id).ToList();
                    var parentId = topCategory.Id;
                    chosenCategories.Add(topCategory.Name);
                    while (nothandledCategories.Count > 0)
                    {
                        for (var i = 0; i < nothandledCategories.Count; i++)
                        {
                            if (nothandledCategories[i].Id == parentId)
                            {
                                parentId = nothandledCategories[i].Id;
                                chosenCategories.Add(nothandledCategories[i].Name);
                                nothandledCategories.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    result = new GroupSignature(chosenCategories.ToArray());
                }
            });
            return result;
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
        public override void Reinitialize()
        {
            DCT.Execute(data =>
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
                //var sResult2 = DataSerializer.Serialize(rawGroups);
                var xml = File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(), "groups2.xml"));
                var rawGroups = DataSerializer.Deserialize<IEnumerable<GroupPackage>>(xml).ToList();


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

                var sResult = DataSerializer.Serialize(rawGroups);
                //Fill groups
                foreach (var group in rawGroups)
                {
                    var hash = group.GetHash();
                    var dbGroup = data.Db1.Groups.FirstOrDefault(q => q.Hash == hash);
                    if(dbGroup == null)
                    {
                        dbGroup = new Group
                        {
                            BoardId = Uid,
                            Hash = group.GetHash(),
                            HasRemoved = false,
                            LastChangeId = null,
                        };
                        data.Db1.Groups.Add(dbGroup);
                        data.Db1.SaveChanges();
                    }

                    for(var i = 0; i < group.Categories.Length; i++)
                    {
                        if (group.Categories[i] == null) break;
                        var parentId = Guid.Empty;
                        if(i > 0)
                        {
                            var parentName = group.Categories[i - 1];
                            var parent = data.Db1.CategoryTemplates.FirstOrDefault(q => q.Name == parentName);
                            if (parent != null)
                                parentId = parent.Id;
                        }
                        var categoryName = group.Categories[i];
                        var dbCategory = data.Db1.CategoryTemplates.FirstOrDefault(q => q.Name == categoryName && q.ParentId == parentId);
                        if(dbCategory == null)
                        {
                            dbCategory = new CategoryTemplate
                            {
                                BoardId = Uid,
                                Name = group.Categories[i],
                                HasRemoved = false,
                                LastChangeId = null,
                                ParentId = parentId
                            };
                            data.Db1.CategoryTemplates.Add(dbCategory);
                            data.Db1.SaveChanges();
                        }

                        var dbGroupedCategory = new GroupedCategory
                        {
                            CategoryId = dbCategory.Id,
                            GroupId = dbGroup.Id,
                            HasRemoved = false,
                            LastChangeId = null
                        };
                        data.Db1.GroupedCategories.Add(dbGroupedCategory);
                        data.Db1.SaveChanges();
                    }
                    foreach(var f in group.Fields)
                    {
                        var dbField = data.Db1.FieldTemplates.FirstOrDefault(q => q.Name == f.Key);
                        if(dbField == null)
                        {
                            dbField = new FieldTemplate
                            {
                                Name = f.Key,
                                Attribute = f.Value.HasId ? "id" : "name",
                                Tag = f.Value.Tag,
                                IsImage = false,
                                IsDynamic = false,
                                LastChangeId = null,
                                HasRemoved = false,
                            };
                            data.Db1.FieldTemplates.Add(dbField);
                            data.Db1.SaveChanges();
                        }

                        var dbGroupedField = new GroupedField
                        {
                            FieldId = dbField.Id,
                            GroupId = dbGroup.Id,
                            HasRemoved = false,
                            LastChangeId = null
                        };
                        data.Db1.GroupedFields.Add(dbGroupedField);
                        data.Db1.SaveChanges();
                    }
                }
               
            });
        }

        void FillCategories(CategoryTree category, Guid parentId)
        {
            var dbCategory = new CategoryTemplate
            {
                BoardId = Uid,
                Name = category.Name,
                HasRemoved = false,
                LastChangeId = null,
                ParentId = parentId
            };
            DCT.Context.Db1.CategoryTemplates.Add(dbCategory);
            DCT.Context.Db1.SaveChanges();

            foreach (var c in category.Children)
                FillCategories(c, dbCategory.Id);
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
            DCT.Execute(data =>
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
                else if (byDataId)
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
        /// <summary>   Parse main component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------
        void ParseMainComponent()
        {
            DCT.Execute(data =>
            {
                //Title
                var nameLabel = GetByTag("label", q => q.InnerText == "Название объявления");
                var nameInput = GetByTag("input", q => q.GetAttribute("id") == "item-edit__title");
                if(nameLabel != null && nameLabel != null)
                    fields.Add(nameLabel.InnerText, FieldPackage.Create("item-edit__title", "input", true));

                //Description
                var descLabel = GetByTag("label", q => q.InnerText == "Описание объявления");
                var descInput = GetByTag("textarea", q => q.GetAttribute("id") == "item-edit__description");
                if (descLabel != null && descInput != null)
                    fields.Add(descLabel.InnerText, FieldPackage.Create("item-edit__description", "textarea", true));
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse location component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseLocationComponent()
        {
            DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__location"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse comission component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseComissionComponent()
        {
            DCT.Execute(data =>
            {
                var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__commission"));
                if (divBlock != null)
                    HandleBlock(divBlock, "form-fieldset__label", "is-hidden");
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Parse parameters component. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------

        void ParseParametersComponent()
        {
            DCT.Execute(data =>
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
            DCT.Execute(data =>
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
            DCT.Execute(data =>
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
            DCT.Execute(data =>
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
            DCT.Execute(data =>
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

            DCT.Execute(data =>
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
                        if (fieldPackage != null)
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
