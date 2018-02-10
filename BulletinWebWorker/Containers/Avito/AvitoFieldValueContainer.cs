using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.FieldValue;
using BulletinWebWorker.Tools;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinWebWorker.Containers.Avito
{
    internal class AvitoFieldValueContainer : FieldValueContainerBase
    {
        public override Guid Uid => BoardIds.Avito;

        public override void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value)
        {
            DCT.Execute(d =>
            {
                var fieldPackage = fields.FirstOrDefault(q => q.Value.Id == name).Value;

                if (fieldPackage == null) return;

                var attribute = fieldPackage.HasId ? "id" : "name";
                var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.Id);
                if (form == null) return;

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
    }
}
