using BulletinWebWorker.Containers.Avito;
using BulletinWebWorker.Containers.Fake;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers.Base.BulletinPackage
{
    static class BulletinPackageContainerList
    {
        #region Property
        private static List<BulletinPackageContainerBase> containerList = new List<BulletinPackageContainerBase>();
        #endregion
        #region Constructor
        static BulletinPackageContainerList()
        {
            Add(new AvitoBulletinPackageContainer());
            Add(new FakeBulletinPackageContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(BulletinPackageContainerBase container)
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
        public static BulletinPackageContainerBase Get(Guid uid)
        {
            BulletinPackageContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
