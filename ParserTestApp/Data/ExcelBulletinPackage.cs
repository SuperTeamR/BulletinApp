using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Data
{
    public class ExcelBulletinPackage
    {
        public List<ExcelCategory> Categories { get; set; } = new List<ExcelCategory>();
        public List<string> Fields { get; set; } = new List<string>();
        public List<ExcelBulletin> Bulletins { get; set; } = new List<ExcelBulletin>();
    }
}
