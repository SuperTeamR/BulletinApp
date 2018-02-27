using BulletinTools;
using FessooFramework.Objects.Data;
using System.Runtime.Serialization;

namespace BulletinBridge.Data
{
    [DataContract]
    public class GroupSignature : CacheObject
    {
        public string Category1 => categories[0];
        public string Category2 => categories[1];
        public string Category3 => categories[2];
        public string Category4 => categories[3];
        public string Category5 => categories[4];
        [DataMember]
        string[] categories = new string[5];

        public GroupSignature(params string[] categories)
        {
            var length = categories.Length < 5 ? categories.Length : 5;
            for (var i = 0; i < length; i++)
            {
                this.categories[i] = categories[i] == string.Empty ? null : categories[i];
            }
        }

        public override string ToString()
        {
            return string.Join(";", categories);
        }
        string hash;
        public string GetHash()
        {
            if (hash == null)
                hash = Сryptography.StringToSha256String(categories);
            return hash;
        }

        public string[] GetCategories()
        {
            return categories;
        }
    }
}
