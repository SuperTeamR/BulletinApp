using BusinessLogic.BoardLogic.Base;
using BusinessLogic.Data;
using BusinessLogic.Tools;
using CommonTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.API
{
    public static class BoardAPI
    {
        public static void Registry(string boardId)
        {

        }
        public static void Auth(string boardId)
        {

        }
        public static void Exit(string boardId)
        {

        }
        public static void CheckAccess(string boardId)
        {

        }
        public static void FillCaptcha(string boardId)
        {

        }
        public static void CheckIntegrity(string boardId)
        {
            var boardContainer = BoardContainerList.Get(boardId);
            var groups = boardContainer.LoadGroups();
            var s = DataSerializer.Serialize(groups);
        }
    }
}
