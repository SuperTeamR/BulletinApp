using BulletinWebWorker.Containers.Avito;
using BulletinWebWorker.Containers.Fake;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Base.Access
{
    static class AccessContainerList
    {
        #region Property
        private static List<AccessContainerBase> containerList = new List<AccessContainerBase>();
        #endregion
        #region Constructor
        static AccessContainerList()
        {
            Add(new AvitoAccessContainer());
            Add(new FakeAccessContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(AccessContainerBase container)
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
        public static AccessContainerBase Get(Guid uid)
        {
            AccessContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
