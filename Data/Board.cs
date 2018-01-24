using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Борда буллетинов
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Наименование борды
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Путь к борде 
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Путь к авторизации
        /// </summary>
        public string UrlAuth { get; set; }
        /// <summary>
        /// Путь к регистрации
        /// </summary>
        public string UrlRegistration { get; set; }
        /// <summary>
        /// Путь к добавлению буллетина
        /// </summary>
        public string UrlAddBulletin { get; set; }

    }
}
