using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Helpers;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    public static class AccessTaskHelper
    {
        public static void CreateAccess(string userLogin, bool hasForwarding, int n)
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

                for(var i = 0; i < n; i++)
                    CreateAccess(userId, hasForwarding);
            });
        }

        public static void CreateAccess(Guid userId, bool hasForwarding)
        {
            BCT.Execute(d =>
            {
                var newLogin = NameHelper.GetNewMail(userId);
                ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Регистрация аккаунта {newLogin}");

                var password = "OnlineHelp59";
                var board = d.BulletinDb.Boards.FirstOrDefault(q => q.Name == "Avito");
                var newAccess = new Access
                {
                    BoardId = board.Id,
                    Login = newLogin,
                    Password = password,
                    UserId = userId,
                    IsForwarding = hasForwarding
                };
                newAccess.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
                d.SaveChanges();
                TaskHelper.CreateAccessRegistration(newAccess);
            });
        }

    }
}
