using BulletinExample.Core;
using BulletinExample.Logic.Containers.Avito;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Containers.Base.Field
{
    internal static class FieldContainerList
    {
        #region Property
        private static List<FieldContainerBase> containerList = new List<FieldContainerBase>();
        #endregion

        #region Constructor
        static FieldContainerList()
        {
            Add(new AvitoFieldContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(FieldContainerBase container)
        {
            DCT.Execute((data) =>
            {
                if (containerList.Any(q => container.Uid == q.Uid))
                    throw new Exception("FieldParserContainerBase уже добавлен, UID контейнера " + container.Uid);
                containerList.Add(container);
            });
        }
        /// <summary>
        /// Получаем контейнер по UID
        /// </summary>
        /// <param name="container"></param>
        public static FieldContainerBase Get(Guid uid)
        {
            FieldContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
