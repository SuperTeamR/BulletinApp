using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BulletinHub.Helpers
{
    public class RegexHelper
    {
        public static IEnumerable<Match> Execute(string pattern, string source)
        {
            var result = new List<Match>();
            Regex r = new Regex(pattern, RegexOptions.Compiled);
            var matches = r.Matches(source);
            foreach (var match in matches)
            {
                result.Add((Match)match);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Возвращает первое вхождение и первую группу из него
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetValue(string pattern, string source)
        {
            var result = new List<Match>();
            Regex r = new Regex(pattern, RegexOptions.Compiled);
            var matches = r.Matches(source);
            if (matches.Count == 0)
                return "";
            foreach (var match in matches)
                return (match as Match).Groups[1].Value;
            return "";
        }
    }
}