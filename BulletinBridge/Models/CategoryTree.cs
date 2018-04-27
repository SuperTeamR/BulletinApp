using FessooFramework.Objects.Data;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class CategoryTree : CacheObject
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