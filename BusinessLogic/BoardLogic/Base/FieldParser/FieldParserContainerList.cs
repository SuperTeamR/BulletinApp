using BusinessLogic.BoardLogic.Avito;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base.FieldParser
{
    internal static class FieldParserContainerList
    {
        #region Property
        private static List<FieldParserContainerBase> containerList = new List<FieldParserContainerBase>();
        #endregion

        #region Constructor
        static FieldParserContainerList()
        {
            Add(new AvitoFieldParserContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(FieldParserContainerBase container)
        {
            _DCT.Execute((data) =>
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
        public static FieldParserContainerBase Get(string uid)
        {
            FieldParserContainerBase result = null;
            _DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
