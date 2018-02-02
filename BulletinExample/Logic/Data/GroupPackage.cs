using BulletinExample.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Logic.Data
{
    [DataContract]
    internal class GroupPackage
    {
        [DataMember]
        public string[] Categories { get; set; } = new string[5];
        [DataMember]
        public Dictionary<string, FieldPackage> Fields { get; set; } = new Dictionary<string, FieldPackage>();

        public string Category1 => Categories[0];
        public string Category2 => Categories[1];
        public string Category3 => Categories[2];
        public string Category4 => Categories[3];
        public string Category5 => Categories[4];

        public GroupPackage(params string[] categories)
        {
            var length = categories.Length < 5 ? categories.Length : 5;
            for (var i = 0; i < length; i++)
            {
                this.Categories[i] = categories[i];
            }
        }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var c = (GroupPackage)obj;
            return (Category1 == c.Category1)
                && (Category2 == c.Category2)
                && (Category3 == c.Category3)
                && (Category4 == c.Category4)
                && (Category5 == c.Category5);
        }

        public override string ToString()
        {
            return string.Join(";", Categories);
        }

        string hash;
        public string GetHash()
        {
            if (hash == null)
                hash = Сryptography.StringToSha256String(Categories);
            return hash;
        }
    }
}
