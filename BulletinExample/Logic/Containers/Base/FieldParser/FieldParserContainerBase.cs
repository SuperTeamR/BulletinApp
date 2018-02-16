using BulletinExample.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BulletinExample.Logic.Containers.Base.FieldParser
{
    /// <summary>
    /// Контейнер, инкапсулирующий специфичную логику парсинга полей
    /// </summary>
    internal abstract class FieldParserContainerBase
    {
        public abstract Guid Uid { get; }

        public abstract FieldPackage Parse(string label, HtmlElement element);
    }
}
