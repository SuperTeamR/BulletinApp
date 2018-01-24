using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public abstract class BulletinContainerBase
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

        /// <summary>
        /// Выгрузка объявлений из файла
        /// </summary>
        public abstract void LoadFromFile();

        /// <summary>
        /// Тестовый метод - выполняет авторизацию и публикацию
        /// </summary>
        /// <returns></returns>
        public bool Execute()
        {
            var result = false;
            _DCT.Execute(data =>
            {
                ExitProfile();
                Authorization();
                PublishBulletin(0);
                //LoadFromFile();

                //ExitProfile();
                //Authorization();
                //PublishBulletin(0);

                //EditBulletin(0);
                //DisableBulletin(0);
                //UpdateBulletin(0);
            }, _DCTGroup.BulletinContainerBase);
            return result;
        }
        #endregion
    }
}
