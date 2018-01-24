using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTestApp.Data
{
    public class CategoryTree
    {
        public string Name { get; set; }
        public List<CategoryTree> Children { get; set; }

        public CategoryTree(string name)
        {
            Name = name;
            Children = new List<CategoryTree>();
        }
        public CategoryTree AddChild(CategoryTree category)
        {
            Children.Add(category);
            return this;
        }
    }
}
