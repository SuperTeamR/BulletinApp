using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Доступ к Board
    /// Если аккаунт больше не используется, помечаем как IsRemoved
    /// </summary>
    public class Access
    {
        /// <summary>
        /// Идентификатор Board
        /// </summary>
        public int BoardId { get; set; }
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// Логин для доступа
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Пароль для доступа
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Состояние доступа
        /// </summary>
        public AccessState State { get; set; }

        

    }

    public enum AccessState
    {
        Created = 0, //Создан
        Blocked = 1, //Заблочен администрацией
        DemandPay = 2, //Принудительный перевод на бизнес-аккаунт
        Closed = 3, //Закрыт
    }
}
