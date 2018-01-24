using ParserTestApp.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Containers.Category.Base
{
    internal static class CategoryContainerList
    {
        #region Property
        private static List<CategoryContainerBase> containerList = new List<CategoryContainerBase>();
        #endregion

        #region Constructor
        static CategoryContainerList()
        {
           
        }
        #endregion

        #region Methods
        public static void Add(CategoryContainerBase container)
        {
            _DCT.Execute((data) =>
            {
                if (containerList.Any(q => container.Category == q.Category))
                    throw new Exception("CategoryContainerBase уже добавлен");
                containerList.Add(container);
            }, _DCTGroup.CategoryContainerList);
        }

        public static CategoryContainerBase Get(Group category)
        {
            CategoryContainerBase result = null;
            _DCT.Execute((data) =>
            {
                result = containerList.FirstOrDefault(q => q.Category == category);
            }, _DCTGroup.BulletinContainerList);
            return null;
        }
        public static CategoryContainerBase Get(string category1, string category2, string category3 = null, string category4 = null, string category5 = null)
        {
            CategoryContainerBase result = null;
            _DCT.Execute((data) =>
            {
                var combinedCategory = new Group(category1, category2, category3, category4, category5);
                result = Get(combinedCategory);
            }, _DCTGroup.BulletinContainerList);
            return null;
        }
        #endregion
    }
}
