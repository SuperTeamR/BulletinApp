using BulletinEngine.Core;
using BulletinHub.Helpers;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class MessageHelper
    {
        public static void RunCollectingMessages(string userLogin)
        {
            BCT.Execute(d =>
            {
                //Находим пользователя
                var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.Login == userLogin);
                if (user == null)
                {
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Пользователь с логином {userLogin} не найден");
                    return;
                }
                var userId = user.Id;

                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.State != (int)DefaultState.Created).ToArray();

                foreach(var access in accesses)
                    TaskHelper.CreateCollectMessages(userId, access);
            });
        }
    }
}
