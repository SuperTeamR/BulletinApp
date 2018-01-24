using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.BoardLogic.Enums
{
    internal enum ContainerCommand
    {
        None = 0,
        //Опубликовать
        Publish = 1,
        //Отредактировать
        Edit = 2,
        //Обновить/поднять в списке
        Update = 3,
        //Удалить/отключить
        Disable = 4,
        //Собрать статистику
        ShowStatistics = 5,
    }
}
