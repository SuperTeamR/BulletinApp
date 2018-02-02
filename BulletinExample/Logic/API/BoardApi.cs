using BulletinExample.Core;
using BulletinExample.Logic.Containers.Base;
using BulletinExample.Logic.Containers.Base.Board;
using FessooFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.API
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Публичная точка доступа для работы с бордами. </summary>
    ///
    /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public static class BoardApi
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Проверяем целостность данных
        /// - Парсит категории
        /// - Парсит поля.
        /// </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void CheckIntegrity(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);
                var boardContainer = BoardContainerList.Get(board.Id);
                boardContainer.ReloadGroups();
            });

        }

        /// <summary>   Получает все буллетины с борды. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------
        public static void GetBulletins(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);

                //var boardContainer = BoardContainerList.Get(boardId);
                //var groups = boardContainer.LoadGroups();
                //var s = DataSerializer.Serialize(groups);
            });
        }
    }
}
