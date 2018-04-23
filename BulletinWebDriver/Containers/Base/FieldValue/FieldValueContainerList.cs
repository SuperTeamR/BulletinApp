using BulletinWebDriver.Containers.Avito;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinWebDriver.Containers.Base.FieldValue
{
    static class FieldValueContainerList
    {
        #region Property
        static List<FieldValueContainerBase> containerList = new List<FieldValueContainerBase>();
        #endregion
        #region Constructor
        static FieldValueContainerList()
        {
            Add(new AvitoFieldValueContainer());
        }
        #endregion
        #region Methods
        public static void Add(FieldValueContainerBase container)
        {
            DCT.Execute(d =>
            {
                if (containerList.Any(q => container.Uid == q.Uid))
                    throw new Exception("FieldValueContainer уже добавлен" + container.Uid);
                containerList.Add(container);
            });
        }

        public static FieldValueContainerBase Get(Guid uid)
        {
            FieldValueContainerBase result = null;
            DCT.Execute(data =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}