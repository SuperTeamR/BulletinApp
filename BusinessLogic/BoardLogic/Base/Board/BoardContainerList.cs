using BusinessLogic.BoardLogic.Boards;
using BusinessLogic.BoardLogic.Enums;
using CommonTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base
{
    internal static class BoardContainerList
    {
        #region Property
        private static List<BoardContainerBase> containerList = new List<BoardContainerBase>();
        #endregion
        #region Constructor
        static BoardContainerList()
        {
            Add(new AvitoContainer());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(BoardContainerBase container)
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
        public static BoardContainerBase Get(string uid)
        {
            BoardContainerBase result = null;
            _DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Uid == uid);
            }, _DCTGroup.BulletinContainerList);
            return result;
        }


        /// <summary>
        /// Запускаем работу всех контейнеров
        /// </summary>
        public static void ExecuteAll()
        {
            _DCT.Execute((data) =>
            {
                //foreach (var container in containerList)
                //    container.Execute();
            }, _DCTGroup.BulletinContainerList);
        }


        internal static void Execute(string uid, ContainerCommand command, int bulletinId)
        {
            _DCT.Execute((data) =>
            {
                var container = Get(uid);
                if (container != null)
                    ExecuteCommand(container, command, bulletinId);
            }, _DCTGroup.BulletinContainerList);
        }

        static void ExecuteCommand(BoardContainerBase container, ContainerCommand command, int bulletinId)
        {
            _DCT.Execute((data) =>
            {
                switch (command)
                {
                    case ContainerCommand.Publish:
                        container.AddBulletin(null, null);
                        break;
                    case ContainerCommand.Edit:
                        container.EditBulletin(null);
                        break;
                    case ContainerCommand.Update:
                        container.UpdateBulletin();
                        break;
                    case ContainerCommand.Disable:
                        container.CloseBulletin();
                        break;
                    case ContainerCommand.ShowStatistics:
                        container.GetStats();
                        break;
                }
            }, _DCTGroup.BulletinContainerList);
        }
        #endregion


    }
}
