﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Data
{
    [DataContract]
    public class CategoryTree
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
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
