using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base
{
    internal abstract class FieldSetterContainerBase
    {
        public string Uid { get; set; }
        public abstract void SetField(string name, string value);
    }
}
