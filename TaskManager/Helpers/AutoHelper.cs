using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using BulletinHub.Helpers;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class AutoHelper
    {
        /// <summary>
        /// Количество инстанций на доступ
        /// </summary>
        const int defaultCapacity = 3;
        public static void NextInstances(string userLogin)
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

                var bulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).ToArray().Where(q => q.DatePublication == null || q.DatePublication.Value.Date != DateTime.Today);
                if(!bulletins.Any())
                {
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Нет буллетинов на републикацию");
                    return;
                }

                var bulletinCount = bulletins.Count();
                var boards = d.BulletinDb.Boards.ToArray();
                var boardCount = boards.Count();
                var needCapacity = bulletinCount * boardCount;
                var accountCapacity = ComputeAccessCapacity(userId);

                ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Нужно публикаций: {needCapacity}");
                ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Доступно мест: {accountCapacity}");
                if (accountCapacity < needCapacity)
                {
                    var needPlaces = needCapacity - accountCapacity;
                    var needAccounts = 1 + (needPlaces - 1) / defaultCapacity;
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Нужно создать {needAccounts} аккаунтов");
                    CreateNewAccesses(userId, needAccounts);
                }
                if(accountCapacity > 0)
                {
                    var toRepublication = bulletins.Take(accountCapacity).ToArray();
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Запуск публикации буллетинов");
                    RunTasks(userId, toRepublication);
                }
            });
        }

        static int ComputeAccessCapacity(Guid userId)
        {
            var accountCapacity = 0;
            BCT.Execute(d =>
            {
                var boards = d.BulletinDb.Boards.ToArray();
                // Сколько объявлений можно повесить на текущие аккаунты
                foreach (var board in boards)
                {
                    var needPlaces = defaultCapacity;
                    var boardAccesses = d.BulletinDb.Accesses.Where(q => !q.HasBlocked).Where(q => q.UserId == userId && q.BoardId == board.Id && (q.State == (int)FessooFramework.Objects.Data.DefaultState.Created || q.State == (int)FessooFramework.Objects.Data.DefaultState.Enable)).OrderBy(q => q.LastPublication).ToArray();

                    foreach (var access in boardAccesses)
                    {
                        var occupiedPlaces = d.BulletinDb.BulletinInstances.Count(q => q.BoardId == board.Id && q.AccessId == access.Id);
                        if (occupiedPlaces < needPlaces)
                        {
                            accountCapacity += needPlaces - occupiedPlaces;
                        }
                        else if (needPlaces >= occupiedPlaces && !access.HasBlocked)
                        {
                            access.HasBlocked = true;
                            access.StateEnum = FessooFramework.Objects.Data.DefaultState.Disable;
                        }
                    }
                    d.SaveChanges();
                }
            });
            return accountCapacity;
        }

        static void CreateNewAccesses(Guid userId, int needAccounts)
        {
            BCT.Execute(d =>
            {
                for (var i = 0; i < needAccounts; i++)
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
                    };
                    newAccess.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
                    d.SaveChanges();
                    newAccess.StateEnum = FessooFramework.Objects.Data.DefaultState.Disable;
                    d.SaveChanges();
                    TaskHelper.CreateAccessRegistration(newAccess);
                }
            });
        }

        static void RunTasks(Guid userId, IEnumerable<Bulletin> bulletins)
        {
            BCT.Execute(d =>
            {
                foreach (var bulletin in bulletins)
                {
                    BulletinHelper.AutoPublicateBulletin(bulletin.Id);
                }
            });
        }


    }
}
