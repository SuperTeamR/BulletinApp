using BusinessLogic.BoardLogic.Base;
using BusinessLogic.Data;
using BusinessLogic.Tools;
using CommonTools;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.API
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Публичная точка доступа для работы с бордами. </summary>
    ///
    /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public static class BoardAPI
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Регистрация на борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void Registry(string boardId)
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Авторизация на борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void Auth(string boardId)
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Выход из профиля на борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void Exit(string boardId)
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Проверка доступности борды. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void CheckAccess(string boardId)
        {

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Заполнение капчи на борде. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void FillCaptcha(string boardId)
        {

        }

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

        public static void CheckIntegrity(string boardId)
        {
            _DCT.Execute(data =>
            {
                var boardContainer = BoardContainerList.Get(boardId);
                var groups = boardContainer.LoadGroups();
                var s = DataSerializer.Serialize(groups);
            });
         
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Генерация excel-файла для выбранной доски и сигнатуры группы. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///
        /// <returns>   An array of byte. </returns>
        ///-------------------------------------------------------------------------------------------------

        public static byte[] GenerateXls(string boardId)
        {
            _DCT.Execute(data =>
            {
                var boardContainer = BoardContainerList.Get(boardId);
                var group = new GroupSignature("Транспорт", "Автомобили", "С пробегом");
                var s = boardContainer.GenerateXlsFromGroup(group);
            });
           
            return null;
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Массовое добавление буллетинов из excel-файла. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void AddFromXls(string boardId)
        {
            _DCT.Execute(data =>
            {
                var boardContainer = BoardContainerList.Get(boardId);
                boardContainer.AddFromXls();
            });

        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Edit from XLS. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void EditFromXls(string boardId)
        {
            _DCT.Execute(data =>
            {
                var boardContainer = BoardContainerList.Get(boardId);
                boardContainer.EditFromXls();
            });
        }

        /// <summary>   Получает все буллетины с борды. </summary>
        ///
        /// <remarks>   SV Milovanov, 30.01.2018. </remarks>
        ///
        /// <param name="boardId">  . </param>
        ///-------------------------------------------------------------------------------------------------

        public static void GetBulletins(string boardId)
        {
            _DCT.Execute(data =>
            {
                var boardContainer = BoardContainerList.Get(boardId);
                boardContainer.GetBulletins();
            });
        }
    }
}
