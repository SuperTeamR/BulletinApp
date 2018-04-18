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
            var result = Enumerable.Empty<Bulletin>().ToList(); ;
            BCT.Execute(d =>
            {
                var bulletins = objs.ToArray();
                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == d.UserId && q.State == (int)AccessState.Activated).ToArray();
                var cycles = bulletins.Length >= accesses.Length ? bulletins.Length / accesses.Length : 1;
                var board = d.BulletinDb.Boards.FirstOrDefault(q => q.Name == "Avito");

                for (int i = 0; i < objs.Count(); i++)
                {
                    var currentBulletin = bulletins[i];
                    currentBulletin.StateEnum = BulletinState.Created;

                    var access = accesses[i % cycles];
                    var instance = new BulletinInstance
                    {
                        AccessId = access.Id,
                        BoardId = board.Id,
                        BulletinId = currentBulletin.Id,
                    };
                    instance.StateEnum = BulletinInstanceState.WaitPublication;
                    result.Add(currentBulletin);
                }
                d.BulletinDb.SaveChanges();
            });
            return result;
        }
    }
}
