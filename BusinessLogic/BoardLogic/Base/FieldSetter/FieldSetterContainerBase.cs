using BusinessLogic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base
{
    internal abstract class FieldSetterContainerBase
    {
        protected Dictionary<string, FieldPackage> Fields { get; set;
        }
        public string Uid { get; set; }
        public abstract void LoadFieldsFromGroup(GroupSignature signature);
        public abstract string GetField(string name);
        public abstract void SetField(string name, string value);
    }
}
