using BulletinWebWorker.Containers.Avito;
using BulletinWebWorker.Containers.Fake;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Base.FieldValue
{

    static class FieldValueContainerList
    {
        #region Property
        private static List<FieldValueContainerBase> containerList = new List<FieldValueContainerBase>();
        #endregion
        #region Constructor
        static FieldValueContainerList()
        {
            Add(new AvitoFieldValueContainer());
            Add(new FakeFieldValueContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(FieldValueContainerBase container)
        {
            DCT.Execute((data) =>
            {
                if (containerList.Any(q => container.Uid == q.Uid))
                    throw new Exception("IBulletinContainer уже добавлен, UID контейнера " + container.Uid);
                containerList.Add(container);
            });
        }
        /// <summary>
        /// Получаем контейнер по UID
        /// </summary>
        /// <param name="container"></param>
        public static FieldValueContainerBase Get(Guid uid)
        {
            FieldValueContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
