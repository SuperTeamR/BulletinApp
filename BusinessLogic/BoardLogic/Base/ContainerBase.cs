using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Base
{
    /// <summary>
    /// Базовый контейнер для управлениями данными
    /// Инициализирует, создает и получает данные
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class ContainerBase<T>
    {
        public string Uid { get; set; }
        public abstract IEnumerable<T> Initialize();
        public abstract IEnumerable<T> GetAll();
        public abstract T Get(string hash);
    }
}
