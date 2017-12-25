using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public abstract class BulletinContainerBase : IBulletinContainer
    {
        #region Property

        #region Тестовые свойства
        public abstract string StartUrl { get; }
        public abstract string LoginUrl { get; }
        public abstract string Login { get; }
        public abstract string Password { get; } 
        #endregion

        public string UID { get; set; }
        public bool HasAuth { get; set; }
        #endregion
        #region Constructor
        #endregion
        #region Methods

        /// <summary>
        /// Авторизация
        /// </summary>
        public abstract void Authorization();
        /// <summary>
        /// Выход из профиля
        /// </summary>
        public abstract void ExitProfile();
        /// <summary>
        /// Публикация нового объявления
        /// </summary>
        public abstract void PublishBulletin(int bulletinId);
        /// <summary>
        /// Редактировать объявление
        /// </summary>
        public abstract void EditBulletin(int bulletinId);
        /// <summary>
        /// Редактировать объявление
        /// </summary>
        public abstract void UpdateBulletin(int bulletinId);
        /// <summary>
        /// Отключить объявление
        /// </summary>
        public abstract void DisableBulletin(int bulletinId);
        /// <summary>
        /// Собрать статистику по просмотрам
        /// </summary>
        public abstract void GetViewStatistics(int bulletinId);
        /// <summary>
        /// Собрать сообщения
        /// </summary>
        public abstract void GetMessages(int bulletinId);

        public bool Execute()
        {
            var result = false;
            _DCT.Execute(data =>
            {
                Authorization();
            }, _DCTGroup.BulletinContainerBase);
            return result;
        }
        #endregion
    }
}
