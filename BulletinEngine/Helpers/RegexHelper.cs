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
    }
}