using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Data
{
    public class BulletinPackage
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string State { get; set; }
        public string Views { get; set; }

        public GroupSignature Signature { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
