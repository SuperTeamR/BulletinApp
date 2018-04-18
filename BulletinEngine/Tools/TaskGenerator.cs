using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BulletinHub.Tools
{
    public static class TaskGenerator
    {
        public static void GenerateTasks(Guid userId)
        {
            BCT.Execute(d =>
            {
                if(CheckIfTime(userId))
                {
                    ClearOldTasks(userId);
                    AddNewBulletinTasks(userId);
                    SetTime(userId);
                }
               
            });
        }
        static bool CheckIfTime(Guid userId)
        {
            var result = false;
            BCT.Execute(d =>
            {
                var userSettings = d.BulletinDb.UserSettings.FirstOrDefault(q => q.UserId == userId);
                if (!userSettings.NextTaskGeneration.HasValue
                || userSettings.NextTaskGeneration.Value.Ticks >= DateTime.Now.Ticks)
                    result = true;

            });
            return result;
        }
        static void ClearOldTasks(Guid userId)
        {
            BCT.Execute(d =>
            {
                var tasks = d.BulletinDb.Tasks.Where(q => q.UserId == userId).ToArray();
                d.BulletinDb.Tasks.RemoveRange(tasks);

            });
        }

        static int timeoutBetweenTaskGroup = 30;
        static void AddNewBulletinTasks(Guid userId)
        {
            BCT.Execute(d =>
            {
                var board = d.BulletinDb.Boards.FirstOrDefault(q => q.Name == "Avito");

                var bulletinIds = d.BulletinDb.Bulletins.Select(q => q.Id).ToArray();
                var usedBulletinIds = d.BulletinDb.BulletinInstances.Where(q => bulletinIds.Contains(q.BulletinId)).Select(q => q.BulletinId).ToArray();
                var unusedBulletinsIds = bulletinIds.Where(q => usedBulletinIds.Contains(q)).ToArray();

                var targetType = typeof(BulletinInstance).ToString();

                var bulletins = d.BulletinDb.BulletinInstances.Where(q => unusedBulletinsIds.Contains(q.Id)).ToArray();
                if (bulletins.Length == 0) return;

                var accesses = d.BulletinDb.Accesses.Where(q => q.State == (int)AccessState.Activated).ToArray();
                var cycles = bulletins.Length >= accesses.Length ? bulletins.Length / accesses.Length : 1;
                for (int i = 0; i < bulletins.Length; i++)
                {
                    var currentBulletin = bulletins[i];
                    var access = accesses[i % accesses.Length];

                    var instance = new BulletinInstance
                    {
                        AccessId = access.Id,
                        BoardId = board.Id,
                        BulletinId = currentBulletin.Id,
                    };
                    instance.StateEnum = BulletinInstanceState.WaitPublication;

                    var task = new Entity.Data.Task
                    {
                        BulletinId = currentBulletin.Id,
                        AccessId = access.Id,
                        TargetDate = DateTime.Now.Add(TimeSpan.FromMinutes(timeoutBetweenTaskGroup * (i / cycles))),
                        TargetType = targetType,
                        Command = (int)Entity.Data.TaskCommand.Creation,
                    };
                    task.StateEnum = Entity.Data.TaskState.Created;
                }
                d.BulletinDb.SaveChanges();
            });
        }

        static void SetTime(Guid userId)
        {
            BCT.Execute(d =>
            {
                var userSettings = d.BulletinDb.UserSettings.FirstOrDefault(q => q.UserId == userId);
                userSettings.LastTimeGeneration = DateTime.Now;
                userSettings.NextTaskGeneration = userSettings.LastTimeGeneration.Value.AddHours(userSettings.TaskGenerationPeriod);

            });
        }
    }
}
