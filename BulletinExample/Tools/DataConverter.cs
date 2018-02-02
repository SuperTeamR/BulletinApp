using BulletinExample.Core;
using BulletinExample.Logic.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Tools
{
    internal static class DataConverter
    {
        public static IEnumerable<GroupPackage> ToGroupCollection(this List<CategoryTree> tree)
        {
            var result = new List<GroupPackage>();
            DCT.Execute(data =>
            {
                foreach (var t in tree)
                {
                    var categories = new List<string>();
                    GetChildCategory(t, categories, result);
                }
            });
            return result.ToList();
        }


        static void GetChildCategory(CategoryTree tree, List<string> categories, List<GroupPackage> groups)
        {
            DCT.Execute(data =>
            {
                categories.Add(tree.Name);

                if (tree.Children != null && tree.Children.Count == 0)
                {
                    var group = new GroupPackage(categories.ToArray());
                    groups.Add(group);
                    categories.Remove(tree.Name);
                    return;
                }
                foreach (var c in tree.Children)
                {
                    GetChildCategory(c, categories, groups);
                }
                categories.Remove(tree.Name);
            });
        }
    }
}
