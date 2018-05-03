using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BulletinWebDriver.Helpers
{
    public static class RegexHelper
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
