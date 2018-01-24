using BusinessLogic.BoardLogic.Fields;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base.FieldSetter
{
    internal static class FieldSetterContainerList
    {
        #region Property
        private static List<FieldSetterContainerBase> containerList = new List<FieldSetterContainerBase>();
        #endregion
        #region Constructor
        static FieldSetterContainerList()
        {
            Add(new AvitoFieldSetterContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(FieldSetterContainerBase container)
        {
            _DCT.Execute((data) =>
            {
                if (containerList.Any(q => container.Uid == q.Uid))
                    throw new Exception("IBulletinContainer уже добавлен, UID контейнера " + container.Uid);
                containerList.Add(container);
            }, _DCTGroup.BulletinContainerList);
        }
        /// <summary>
        /// Получаем контейнер по UID
        /// </summary>
        /// <param name="container"></param>
        public static FieldSetterContainerBase Get(string uid)
        {
            FieldSetterContainerBase result = null;
            _DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
