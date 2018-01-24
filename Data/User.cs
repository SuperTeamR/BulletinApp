using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Аккаунт пользователя в BulletinSystem
    /// </summary>
    public class User
    {
        /// <summary>
        /// Логин пользователя
        /// </summary>
        public string Login { get; set; }
        /// <summary>
        /// Хэш пользователя
        /// </summary>
        public string Hash { get; set; }
    }
}
