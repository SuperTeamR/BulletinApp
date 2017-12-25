using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Base
{
    public static class ParseContainerList
    {
        #region Property
        private static List<IParseContainer> containerList = new List<IParseContainer>();
        #endregion
        #region Constructor
        static ParseContainerList()
        {
            Add(new ContainerAvito());
        }
        #endregion
        #region Methods
        /// <summary>
        /// Добавляем новый контейнер
        /// </summary>
        /// <param name="container"></param>
        public static void Add(IParseContainer container)
        {
            _DCT.Execute((data) =>
            {
                containerList.Add(container);
            }, _DCTGroup.ParseContainerList);
        }
        /// <summary>
        /// Запускает парсинг релизов во всех доступных контейнерах
        /// </summary>
        internal static void Execute()
        {
            _DCT.Execute((data) =>
            {
                foreach (var container in containerList)
                    container.Execute();
            }, _DCTGroup.ParseContainerList);
        }
        #endregion


    }
}
