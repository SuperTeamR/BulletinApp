using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Data
{
    public class Bulletin
    {
        public string CategoryLevel1 { get; set; }
        public string CategoryLevel2 { get; set; }
        public string CategoryLevel3 { get; set; }
        public string CategoryLevel4 { get; set; }
        public string CategoryLevel5 { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public double Price { get; set; }

        public string[] Images { get; set; }
    }
}
