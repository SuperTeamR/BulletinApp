using BulletinBridge.Models;
using BulletinClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinClient.Helpers
{
    static class TemplateFilterHelper
    {
        static string[] forbiddenWords = new[]
        {
            "запчасти",
        };
        public static IEnumerable<BulletinTemplateCache> FilterByPattern(this IEnumerable<BulletinTemplateCache> collection, string pattern)
        {
            var result = collection;
            DCT.Execute(d =>
            {
                // Исключаем шаблоны с запрещенными словами
                var temp = collection.Where(q => forbiddenWords.All(x => !q.Description.Contains(x))
                && forbiddenWords.All(x => !q.Title.Contains(x))).ToArray();

                // Фильтруем шаблоны по словам в тайтле
                var words = pattern.Split(' ');
                result = temp.Where(q => words.All(x => q.Title.Contains(x))).ToArray();
            });
            return result;
        }
    }
}
