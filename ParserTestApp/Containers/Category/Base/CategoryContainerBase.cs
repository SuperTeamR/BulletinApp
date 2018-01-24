using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserTestApp.Containers.Category.Base
{
    internal abstract class CategoryContainerBase
    {
        public Group Category { get; private set; }
        HtmlDocument document;
        Dictionary<string, Action<string>> fieldActions = new Dictionary<string, Action<string>>();
        
        public void SetField(string field, string value)
        {
            Action<string> action;
            if(!fieldActions.TryGetValue(field, out action))
            {
                throw new Exception("Действие для заполнения поля не найдено");
            }
            action(value);

        }
        public CategoryContainerBase(Group category, HtmlDocument document)
        {
            this.Category = category;
            this.document = document;
            InitializeActions();
        }
        protected virtual void InitializeActions()
        {
            AddAction("Адрес", (v) =>
            {
                var form = document.GetElementsByTagName("input").Cast<HtmlElement>()
                           .FirstOrDefault(q => q.GetAttribute("id") == "flt_param_address");
                if (form != null) form.SetAttribute("value", v);
            });
            AddAction("Название объявления", (v) =>
            {
                var form = document.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__title");
                if (form != null) form.SetAttribute("value", v);
            });
            AddAction("Описание объявления", (v) =>
            {
                var form = document.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__description");
                if (form != null) form.SetAttribute("value", v);
            });
            AddAction("Плата за услугу", (v) =>
            {
                var form = document.GetElementsByTagName("input").Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute("id") == "item-edit__price");
                if (form != null) form.SetAttribute("value", v);
            });
        }
        protected void AddAction(string field, Action<string> action)
        {
            fieldActions.Add(field, action);
        }


        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            CategoryContainerBase c = (CategoryContainerBase)obj;
            var second = c.Category;
            return (Category.Category1 == second.Category1)
                && (Category.Category2 == second.Category2)
                && (Category.Category3 == second.Category3)
                && (Category.Category4 == second.Category4)
                && (Category.Category5 == second.Category5);

        }
    }
}
