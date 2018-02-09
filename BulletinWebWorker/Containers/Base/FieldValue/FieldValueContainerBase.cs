using BulletinBridge.Data;
using System;
using System.Collections.Generic;

namespace BulletinWebWorker.Containers.Base.FieldValue
{
    internal abstract class FieldValueContainerBase
    {
        public abstract Guid Uid { get; }

        public abstract void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value);
    }
}
