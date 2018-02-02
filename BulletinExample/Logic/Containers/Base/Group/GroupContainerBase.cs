using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Containers.Base.Group
{
    /// <summary>
    /// Основной контейнер работы с уровнем данных - Group
    /// </summary>
    internal abstract class GroupContainerBase
    {
        public abstract Guid Uid { get; }

        public abstract void Reinitialize();
    }
}
