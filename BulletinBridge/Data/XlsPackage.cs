using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinBridge.Data
{
    public class XlsPackage
    {
        public string Url { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
