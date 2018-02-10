using BulletinExample.Core;
using BulletinExample.Logic.Containers.Base;
using BulletinExample.Logic.Containers.Base.Board;
using BulletinExample.Logic.Data;
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
                var boardContainer = BoardContainerList.Get(board.Id);
                boardContainer.GetBulletins();
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Генерирует xls с имеющимися буллетинами </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///
        /// <param name="boardName">    Name of the board. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void GetXlsBulletins(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);
                var boardContainer = BoardContainerList.Get(board.Id);
                boardContainer.GetXlsBulletins();
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets XLS group. </summary>
        ///
        /// <remarks>   SV Milovanov, 06.02.2018. </remarks>
        ///
        /// <param name="boardName">    Name of the board. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void GetXlsGroup(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);
                var boardContainer = BoardContainerList.Get(board.Id);
                var signature = new GroupSignature("Хобби и отдых", "Спорт и отдых", "Другое");
                boardContainer.GetXlsGroup(signature);
            });
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Редактирует буллетины из xls. </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///
        /// <param name="boardName">    Name of the board. </param>
        ///-------------------------------------------------------------------------------------------------

        public static void EditBulletinsFromXls(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);
                var boardContainer = BoardContainerList.Get(board.Id);
                boardContainer.EditBulletinsFromXls();
            });
        }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетина через xls </summary>
        ///
        /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
        ///-------------------------------------------------------------------------------------------------
        public static void AddBulletinsFromXls(string boardName)
        {
            DCT.Execute(data =>
            {
                var board = data.Db1.Boards.FirstOrDefault(q => q.Name == boardName);
                var boardContainer = BoardContainerList.Get(board.Id);
                boardContainer.AddBulletinsFromXls();
            });
        }
    }
}
