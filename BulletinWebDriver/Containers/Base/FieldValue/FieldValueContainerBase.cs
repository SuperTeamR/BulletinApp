using BulletinBridge.Data;
using System;
using System.Collections.Generic;

namespace BulletinWebDriver.Containers.Base.FieldValue
{
    abstract class FieldValueContainerBase
    {
        public abstract Guid Uid { get; }
        public abstract void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value);
        public abstract string GetFieldValue(Dictionary<string, FieldPackage> fields, string name);
    }
}