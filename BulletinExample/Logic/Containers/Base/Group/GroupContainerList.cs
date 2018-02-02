using BulletinExample.Core;
using BulletinExample.Logic.Containers.Avito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Containers.Base.Group
{
    internal static class GroupContainerList
    {
        #region Property
        private static List<GroupContainerBase> containerList = new List<GroupContainerBase>();
        #endregion
        #region Constructor
        static GroupContainerList()
        {
            Add(new AvitoGroupContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(GroupContainerBase container)
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
        public static GroupContainerBase Get(Guid uid)
        {
            GroupContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
