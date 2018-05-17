using BulletinEngine.Core;
using CodenameGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class NameHelper
    {
        public static string GetNewMail(Guid userId)
        {
            var result = string.Empty;
            BCT.Execute(d =>
            {
                var accesses = d.BulletinDb.Accesses.Count(q => q.UserId == userId);
                var counter = accesses++;
                var domain = "@mail.ru";

                var generator = new Generator();
                generator.SetParts(WordBank.Adjectives, WordBank.FirstNames, WordBank.LastNames);
                generator.Separator = ".";
                generator.Casing = Casing.PascalCase;
                var rawName = generator.Generate();
                result = $"{rawName}_{counter}{domain}";
            });
            return result;
        }

    }
}
