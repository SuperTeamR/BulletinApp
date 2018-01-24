using ParserTestApp.Containers.Category.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParserTestApp.Containers.Category
{
    public class AvitoParserContainer : CategoryParserContainerBase
    {
        public AvitoParserContainer(HtmlDocument document) : base(document)
        {
        }
    }
}
