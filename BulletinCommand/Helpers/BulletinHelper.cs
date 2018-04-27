using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinCommand.Helpers
{
    public static class BulletinHelper
    { 
        /// <summary>
        /// Создаёт базовую инстанцию без распределения по доступу
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IEnumerable<BulletinInstance> CreateInstance(Bulletin bulletin)
        {
            var result = new List<BulletinInstance>();
            BCT.Execute(c =>
            {
                var boards = c.BulletinDb.Boards.ToArray();
                foreach (var board in boards)
                {
                    var instance = new BulletinInstance();
                    instance.BoardId = board.Id;
                    instance.BulletinId = bulletin.Id;
                    instance.GroupId = bulletin.GroupId.Value;
                    instance.StateEnum = BulletinInstanceState.Created;
                    result.Add(instance);
                }
                c.SaveChanges();
            });
            return result;
        }
    }
}
