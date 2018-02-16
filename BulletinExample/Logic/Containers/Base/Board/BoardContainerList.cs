using BulletinExample.Core;
using BulletinExample.Logic.Containers.Avito;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinExample.Logic.Containers.Base.Board
{
    internal static class BoardContainerList
    {
        #region Property
        private static List<BoardContainerBase> containerList = new List<BoardContainerBase>();
        #endregion
        #region Constructor
        static BoardContainerList()
        {
            Add(new AvitoBoardContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(BoardContainerBase container)
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
        public static BoardContainerBase Get(Guid uid)
        {
            BoardContainerBase result = null;
            DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            });
            return result;
        }
        #endregion
    }
}
