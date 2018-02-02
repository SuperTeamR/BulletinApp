using BusinessLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusinessLogic.BoardLogic.Base.FieldParser
{
    /// <summary>
    /// Основной контейнер, инкапсулирующий специфичную логику парсинга полей
    /// </summary>
    internal abstract class FieldParserContainerBase
    {
        public string Uid { get; set; }
        public abstract FieldPackage Parse(string label, HtmlElement element);
    }
}
