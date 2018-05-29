using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinEngine.Helpers;
using BulletinHub.Entity.Data;
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

                //Определяем количество буллетинов, которые надо опубликовать
                var bulletins = GetBulletinsForPublication(userId);
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

        static IEnumerable<Bulletin> GetBulletinsForPublication(Guid userId)
        {
            var result = Enumerable.Empty<Bulletin>();
            BCT.Execute(d =>
            {
                var bulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).ToArray();
                //Определяем опубликованные инстанции
                var bulletinIds = bulletins.Select(q => q.Id);
                var publicatedInstances = d.BulletinDb.BulletinInstances.Where(q =>
                    q.Url != null
                    && bulletinIds.Any(qq => qq == q.BulletinId)).ToArray().Where(q => q.CreateDate.Date == DateTime.Today);

                //Определяем неопубликованные инстанции, которые в процесс публикации
                var unpublicatedInstances = d.BulletinDb.BulletinInstances.Where(q =>
                    q.Url == null
                    && bulletinIds.Any(qq => qq == q.BulletinId)).ToArray().Where(q => q.CreateDate.Date == DateTime.Today);
                var unpublicatedInstanceIds = unpublicatedInstances.Select(q => q.Id);
                var instanceToPublicationIds = d.TempDB.Tasks.Where(q => q.Command == (int)TaskCommand.InstancePublication
                    && q.State == 0
                    && unpublicatedInstanceIds.Any(qq => qq == q.InstanceId)).Select(q => q.InstanceId).ToArray();
                var instancesToPublication = unpublicatedInstances.Where(q => instanceToPublicationIds.Any(qq => qq == q.Id)).ToArray();

                // Пропускаем уже опубликованные или в стадии публикации буллетины
                var skippedBulletinIds = publicatedInstances.Concat(instancesToPublication).Select(q => q.BulletinId);
                result = bulletins.Where(q => skippedBulletinIds.All(qq => qq != q.Id)).ToArray();
            });
            return result;
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
                    var boardAccesses = d.BulletinDb.Accesses.Where(q => !q.HasBlocked).Where(q => q.UserId == userId && q.BoardId == board.Id && (q.State == (int)FessooFramework.Objects.Data.DefaultState.Enable)).OrderBy(q => q.LastPublication).ToArray();

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
                            access.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
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
                var forwardingEnabled = false;
                var userSettings = d.BulletinDb.UserSettings.FirstOrDefault(q => q.UserId == userId);
                if (userSettings != null && userSettings.EnableForwarding)
                    forwardingEnabled = true;

                var accessInTasks = d.TempDB.Tasks.Where(q => q.State == 0 && q.UserId == userId && q.Command == (int)TaskCommand.Registration).Select(q => q.AccessId).ToArray();
                var accessCountInTasks = d.BulletinDb.Accesses.Count(q => q.UserId == userId && accessInTasks.Any(qq => qq == q.Id));
                needAccounts -= accessCountInTasks;
                if (needAccounts <= 0) return;

                for (var i = 0; i < needAccounts; i++)
                    AccessTaskHelper.CreateAccess(userId, forwardingEnabled);
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
