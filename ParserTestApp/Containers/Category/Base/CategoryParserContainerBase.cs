using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ParserTestApp.Containers.Category.Base
{
    public abstract class CategoryParserContainerBase
    {
        HtmlDocument document;
        ParsingBlockStage nextStage;
        Dictionary<string, InputPackage> dictionary = new Dictionary<string, InputPackage>();
        Group group;

        public CategoryParserContainerBase(HtmlDocument document)
        {
            this.document = document;
        }


        public void Execute(Group group)
        {
            this.group = group;
            ParseMainComponent();
            ParseLocationComponent();
            ParseComissionComponent();
            //ParsePhotoComponent();
            ParseVideoComponent();
            ParseParametersComponent();
            SaveAsXml();
        }

        public void SaveAsXml()
        {
            var s = Serialize(new GroupPackage(group, dictionary));
        }
        protected virtual void ParseMainComponent()
        {
            //Title
            var nameLabel = GetByTag("label", q => q.InnerText == "Название объявления");
            var nameInput = GetByTag("input", q => q.GetAttribute("id") == "item-edit__title");
            dictionary.Add(nameLabel.InnerText, InputPackage.Create("item-edit__title", "input", true));

            //Description
            var descLabel = GetByTag("label", q => q.InnerText == "Описание объявления");
            var descInput = GetByTag("textarea", q => q.GetAttribute("id") == "item-edit__description");
            dictionary.Add(descLabel.InnerText, InputPackage.Create("item-edit__description", "textarea", true));
        }
        protected virtual void ParseLocationComponent()
        {
            var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__location"));
            if(divBlock != null)
                HandleBlock(divBlock, "form-fieldset__label");
        }

        protected virtual void ParseComissionComponent()
        {
            var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__commission"));
            if (divBlock != null)
                HandleBlock(divBlock, "form-fieldset__label");
        }

        protected virtual void ParsePhotoComponent()
        {
            var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__photos"));
            if (divBlock != null)
                HandleBlock(divBlock, "form-fieldset__label");
        }


        protected virtual void ParseVideoComponent()
        {
            var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__video"));
            if (divBlock != null)
                HandleBlock(divBlock, "form-fieldset__label");
        }

        protected virtual void ParseParametersComponent()
        {
            var divBlock = GetByTag("div", q => q.GetAttribute("className").Contains("js-component__parameters"));
            if (divBlock != null)
                HandleBlock(divBlock, "form-fieldset__label");
        }

        protected virtual HtmlElement GetByTag(string tag, Func<HtmlElement, bool> query)
        {
           return document.GetElementsByTagName(tag).Cast<HtmlElement>().FirstOrDefault(query);
        }

        protected virtual void HandleBlock(HtmlElement element, string labelClass)
        {
            nextStage = ParsingBlockStage.None;

            var dictionary = new Dictionary<string, InputPackage>();
            var currentLabel = string.Empty;

            HandleElement(element, currentLabel, labelClass);
        }

        protected void HandleElement(HtmlElement element, string currentLabel, string labelClass)
        {
            if (!element.CanHaveChildren) return;
            foreach (HtmlElement ch in element.Children)
            {
                if (nextStage == ParsingBlockStage.None || nextStage == ParsingBlockStage.Label)
                {
                    if (ch.GetAttribute("className").Contains(labelClass))
                    {
                        currentLabel = ch.InnerText;
                        nextStage = ParsingBlockStage.Input;
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
                            dictionary.Add(currentLabel, InputPackage.Create(id, ch.TagName.ToLower(), true));
                        }
                        else
                        {
                            var name = ch.GetAttribute("name");
                            dictionary.Add(currentLabel, InputPackage.Create(name, ch.TagName.ToLower(), false));
                        }
                        nextStage = ParsingBlockStage.Label;
                    }
                }

                HandleElement(ch, currentLabel, labelClass);
            }
        }

        

        string Serialize(object obj)
        {
            var result = string.Empty;
            var serializer = new DataContractSerializer(obj.GetType());
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriterSettings = new System.Xml.XmlWriterSettings()
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    OmitXmlDeclaration = false,
                    Indent = true,
                    
                };
                using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(memoryStream, xmlWriterSettings))
                    serializer.WriteObject(xw, obj);
                result = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            return result;
        }


        private XElement DictToXml
                  (Dictionary<string, InputPackage> inputDict, string elmName, string valuesName)
        {
            var outElm = new XElement(elmName);

            Dictionary<string, InputPackage>.KeyCollection keys = inputDict.Keys;

            var inner = new XElement(valuesName);

            foreach (string key in keys)
            {
                inner.Add(new XAttribute("key", key));
                inner.Add(new XAttribute("value", inputDict[key]));
            }

            outElm.Add(inner);

            return outElm;
        }


    }

    public enum ParsingBlockStage
    {
        None = 0,
        Label = 1,
        Input = 2
    }

    [DataContract]
    public class InputPackage
    {
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string Tag { get; set; }

        [DataMember]
        public bool HasId { get; set; }

        InputPackage(string id, string tag, bool hasId)
        {
            Id = id;
            Tag = tag;
            HasId = hasId;
        }

        public static InputPackage Create(string id, string tag, bool hasId = true)
        {
            return new InputPackage(id, tag, hasId);
        }
    }

    [DataContract]
    public class GroupPackage
    {
        [DataMember]
        public Group Group { get; set; }
        [DataMember]
        public Dictionary<string, InputPackage> Fields { get; set; }

        public GroupPackage(Group group, Dictionary<string, InputPackage> fields)
        {
            Group = group;
            Fields = fields;
        }
    }
}
