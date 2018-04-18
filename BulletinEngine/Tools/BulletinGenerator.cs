using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Tools
{
    static class BulletinGenerator
    {
        public static IEnumerable<Bulletin> Create(IEnumerable<Bulletin> objs)
        {
            var result = Enumerable.Empty<Bulletin>().ToList();
            BCT.Execute(d =>
            {
                var bulletins = objs.ToArray();
                for (int i = 0; i < objs.Count(); i++)
                {
                    var currentBulletin = bulletins[i];
                    currentBulletin.StateEnum = BulletinState.Created;
                    result.Add(currentBulletin);
                }
                d.BulletinDb.SaveChanges();
            });
            return result;
        }
    }
}
