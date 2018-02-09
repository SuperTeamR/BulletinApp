using BulletinExample.Core;
using BulletinExample.Logic.Containers.Base.Field;
using BulletinExample.Logic.Containers.Base.Group;
using BulletinExample.Logic.Data;
using BulletinExample.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinExample.Logic.Containers.Avito
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Контейнер, управляющий получение и заданием значением поля </summary>
    ///
    /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal class AvitoFieldContainer : FieldContainerBase
    {
        public override Guid Uid => BoardIds.Avito;


        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получить для контейнера поля группы </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="signature">    The signature. </param>
        ///-------------------------------------------------------------------------------------------------
        public override void LoadFieldsFromGroup(GroupSignature signature)
        {
            DCT.Execute(data =>
            {
                var groupContainer = GroupContainerList.Get(Uid);
                var group = groupContainer.GetGroupPackage(signature.GetHash());

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
        public override void SetFieldValue(string name, string value)
        {
            DCT.Execute(data =>
            {
                var fieldPackage = Fields.FirstOrDefault(q => q.Value.Id == name).Value;

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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получаем значение поля по классификатору. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="name"> Классификатор поля. </param>
        ///
        /// <returns>   Значение поля. </returns>
        ///-------------------------------------------------------------------------------------------------
        public override string GetFieldValue(string name)
        {
            var result = string.Empty;
            DCT.Execute(data =>
            {
                var fieldPackage = Fields.FirstOrDefault(q => q.Value.Id == name).Value;
                var attribute = fieldPackage.HasId ? "id" : "name";
                var form = WebWorker.WebDocument.GetElementsByTagName(fieldPackage.Tag).Cast<HtmlElement>()
                                .FirstOrDefault(q => q.GetAttribute(attribute) == fieldPackage.Id);

                if (form == null) return;

                if (fieldPackage.Tag == "select" && fieldPackage.Options != null)
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
