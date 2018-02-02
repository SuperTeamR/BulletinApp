using BulletinExample.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Core
{
    public class GlobalObjects
    {
        public User CurrentUser { get; set; }
        public Access CurrentAccess { get; set; }
    }
}
