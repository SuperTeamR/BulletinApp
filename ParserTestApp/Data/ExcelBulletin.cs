using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Data
{
    public class ExcelBulletin
    {
        public Dictionary<string, string> Fields { get; set; } = new Dictionary<string, string>();
    }
}
