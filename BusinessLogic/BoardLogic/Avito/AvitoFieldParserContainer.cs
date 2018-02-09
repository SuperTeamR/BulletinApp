using BusinessLogic.BoardLogic.Base.FieldParser;
using BusinessLogic.Data;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Avito
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Контейнер, управляющий парсингом полей </summary>
    ///
    /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal class AvitoFieldParserContainer : FieldParserContainerBase
    {
        /// <summary>   Классификаторы, исключенные из записи списка их значений </summary>
        string[] dynamicSelects = { "Метро", "Город", "Район" }; 
        /// <summary>
        /// Парсинг элемента (input/select) в FieldPackage
        /// </summary>
        /// <param name="label">Классификатор поля</param>
        /// <param name="element">Элемент для парсинга</param>
        /// <returns></returns>
        public override FieldPackage Parse(string label, HtmlElement element)
        {
            FieldPackage result = null;
            _DCT.Execute(data =>
            {
                if (element.TagName.ToLower() == "select"
                        || element.TagName.ToLower() == "input")
                {
                    var optionList = new List<OptionTag>();
                    if (element.TagName.ToLower() == "select" && !dynamicSelects.Contains(label))
                    {
                        var options = element.Children;
                        foreach (HtmlElement o in options)
                        {
                            var oValue = o.GetAttribute("value");
                            var oText = o.InnerText;

                            optionList.Add(OptionTag.Create(oValue, oText));
                        }
                    }
                    var id = element.GetAttribute("id");
                    if (!string.IsNullOrEmpty(id))
                    {
                        result = FieldPackage.Create(id, element.TagName.ToLower(), true, optionList.ToArray());
                    }
                    else
                    {
                        var name = element.GetAttribute("name");
                        result = FieldPackage.Create(name, element.TagName.ToLower(), false, optionList.ToArray());
                    }
                }
            });
            return result;
        }
    }
}
