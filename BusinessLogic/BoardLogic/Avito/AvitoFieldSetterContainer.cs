using BusinessLogic.BoardLogic.Base;
using BusinessLogic.BoardLogic.Base.FieldSetter;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Fields
{
    internal class AvitoFieldSetterContainer : FieldSetterContainerBase
    {
        public override void SetField(string name, string value)
        {
            var groupContainer = GroupContainerList.Get(Uid);
            var fieldPackage = groupContainer.GetFieldPackage(name);

            var attribute = fieldPackage.HasId ? "id" : "name";
            var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                            .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.Id);

            if (form == null) return;

            switch(fieldPackage.Tag)
            {
                case "input":
                case "textarea":
                    SetInput(form, value);
                    break;
            }
        }
        void SetInput(HtmlElement form, string value)
        {
            form.SetAttribute("value", value);
        }
    }
}
