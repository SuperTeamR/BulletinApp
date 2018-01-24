using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Data;

namespace BusinessLogic.BoardLogic.Base
{
    /// <summary>
    /// Основной контейнер работы с уровнем данных - Group
    /// </summary>
    internal abstract class GroupContainerBase : ContainerBase<Data.Group>
    {

        public abstract FieldPackage GetFieldPackage(string key);
    }
}
