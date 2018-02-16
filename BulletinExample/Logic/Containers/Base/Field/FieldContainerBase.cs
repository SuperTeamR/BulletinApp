using BulletinExample.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Containers.Base.Field
{
    internal abstract class FieldContainerBase
    {
        protected Dictionary<string, FieldPackage> Fields { get; set; }
        public abstract Guid Uid { get; }
        public abstract void LoadFieldsFromGroup(GroupSignature signature);
        public abstract string GetFieldValue(string name);
        public abstract void SetFieldValue(string name, string value);
    }
}
