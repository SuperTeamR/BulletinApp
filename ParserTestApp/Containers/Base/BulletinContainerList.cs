using ParserTestApp.Containers.Enums;
using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public static class BulletinContainerList
    {
        #region Property
        private static List<IBulletinContainer> containerList = new List<IBulletinContainer>();
        #endregion
        #region Constructor
        static BulletinContainerList()
        {
            Add(new ContainerAvito());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(IBulletinContainer container)
        {
            _DCT.Execute((data) =>
            {
                if (containerList.Any(q => container.UID == q.UID))
                    throw new Exception("IBulletinContainer уже добавлен, UID контейнера " + container.UID);
                containerList.Add(container);
            }, _DCTGroup.BulletinContainerList);
        }
        /// <summary>
        /// Получаем контейнер по UID
        /// </summary>
        /// <param name="container"></param>
        public static IBulletinContainer Get(string uid)
        {
            IBulletinContainer result = null;
            _DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.UID == uid);
            }, _DCTGroup.BulletinContainerList);
            return null;
        }


        /// <summary>
        /// Запускаем работу всех контейнеров
        /// </summary>
        internal static void ExecuteAll()
        {
            _DCT.Execute((data) =>
            {
                foreach (var container in containerList)
                    container.Execute();
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

        static void ExecuteCommand(IBulletinContainer container, ContainerCommand command, int bulletinId)
        {
            _DCT.Execute((data) =>
            {
                switch (command)
                {
                    case ContainerCommand.Publish:
                        container.PublishBulletin(bulletinId);
                        break;
                    case ContainerCommand.Edit:
                        container.EditBulletin(bulletinId);
                        break;
                    case ContainerCommand.Update:
                        container.UpdateBulletin(bulletinId);
                        break;
                    case ContainerCommand.Disable:
                        container.DisableBulletin(bulletinId);
                        break;
                    case ContainerCommand.ShowStatistics:
                        container.GetViewStatistics(bulletinId);
                        break;
                }
            }, _DCTGroup.BulletinContainerList);
        }
        #endregion


    }
}
