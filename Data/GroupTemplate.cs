using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    /// <summary>
    /// Шаблон группы
    /// </summary>
    public class GroupTemplate
    {
        /// <summary>
        /// Идентификатор борды
        /// </summary>
        public int BoardId { get; set; }

        /// <summary>
        /// Хэш шаблона группы. Формируется на основе идентификатора борды и списка категорий
        /// </summary>
        public string Hash { get; set; }

    }
}
