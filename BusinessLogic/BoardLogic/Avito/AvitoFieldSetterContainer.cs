using BusinessLogic.BoardLogic.Base;
using BusinessLogic.BoardLogic.Base.FieldSetter;
using BusinessLogic.Data;
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
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Loads fields from group. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="signature">    The signature. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void LoadFieldsFromGroup(GroupSignature signature)
        {
            _DCT.Execute(data =>
            {
                var groupContainer = GroupContainerList.Get(Uid);
                var group = groupContainer.Get(signature.GetHash());
                Fields = group.Fields;
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Устанавливаем значение поля по классификатору. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="name">     Классификатор поля. </param>
        /// <param name="value">    Значение поля. </param>
        ///-------------------------------------------------------------------------------------------------

        public override void SetField(string name, string value)
        {
            _DCT.Execute(data =>
            {
                var fieldPackage = Fields.FirstOrDefault(q => q.Key == name).Value;

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
                }
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем значение поля по классификатору. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="name"> Классификатор поля. </param>
        ///
        /// <returns>   Значение поля. </returns>
        ///-------------------------------------------------------------------------------------------------

        public override string GetField(string name)
        {
            var result = string.Empty;
            _DCT.Execute(data =>
            {
                var fieldPackage = Fields.FirstOrDefault(q => q.Key == name).Value;
                var attribute = fieldPackage.HasId ? "id" : "name";
                var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.Id);

                if (form == null) return;

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
        void SetInput(HtmlElement form, string value)
        {
            _DCT.Execute(data =>
            {
                form.SetAttribute("value", value);
            });
        }
    }
}
