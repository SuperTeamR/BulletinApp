using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Helpers;
using System;
using System.Linq;

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
                    //AddCurrentBulletinTasks(userId);
                    SetTime(userId);
                }
               
            });
        }
        static bool CheckIfTime(Guid userId)
        {
            var result = false;
            BCT.Execute(d =>
            {
                var userSettings = SettingsHelper.GetSettings(userId);
                if (!userSettings.NextTaskGeneration.HasValue
                || userSettings.NextTaskGeneration.Value.Ticks <= DateTime.Now.Ticks)
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
                d.BulletinDb.SaveChanges();
            });
        }
        static int timeoutBetweenTaskGroup = 30;
        static void AddNewBulletinTasks(Guid userId)
        {
            BCT.Execute(d =>
            {
                var board = d.BulletinDb.Boards.FirstOrDefault(q => q.Name == "Avito");

                var bulletinIds = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).Select(q => q.Id).ToArray();
                var usedBulletinIds = d.BulletinDb.BulletinInstances.Where(q => bulletinIds.Contains(q.BulletinId)).Select(q => q.BulletinId).ToArray();
                var unusedBulletinsIds = bulletinIds.Where(q => !usedBulletinIds.Contains(q)).ToArray();

                var targetType = typeof(BulletinInstance).ToString();

               var bulletins = d.BulletinDb.Bulletins.Where(q => unusedBulletinsIds.Contains(q.Id)).ToArray();
               if (bulletins.Length == 0) return;

                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.State != (int)AccessState.Blocked).ToArray();
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
                        BulletinId = instance.Id,
                        AccessId = access.Id,
                        UserId = userId,
                        TargetDate = DateTime.Now.Add(TimeSpan.FromMinutes(timeoutBetweenTaskGroup * (i / accesses.Length))),
                        TargetType = targetType,
                        Command = (int)Entity.Data.TaskCommand.Creation,
                    };
                    task.StateEnum = Entity.Data.TaskState.Created;
                }
                d.BulletinDb.SaveChanges();
            });
        }
        static void AddCurrentBulletinTasks(Guid userId)
        {
            BCT.Execute(d =>
            {
                var targetType = typeof(BulletinInstance).ToString();

                var bulletinIds = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).Select(q => q.Id).ToArray();
                var instances = d.BulletinDb.BulletinInstances.Where(q => bulletinIds.Contains(q.BulletinId)).ToArray();
                
                foreach(var instance in instances)
                {
                    if(instance.State == (int)BulletinInstanceState.OnModeration)
                    {
                        var task = new Entity.Data.Task
                        {
                            BulletinId = instance.Id,
                            AccessId = instance.AccessId,
                            UserId = userId,
                            TargetDate = DateTime.Now,
                            TargetType = targetType,
                            Command = (int)Entity.Data.TaskCommand.Checking,
                        };
                        task.StateEnum = Entity.Data.TaskState.Created;
                    }

                    if(instance.State == (int)BulletinInstanceState.Edited)
                    {
                        var task = new Entity.Data.Task
                        {
                            BulletinId = instance.Id,
                            AccessId = instance.AccessId,
                            UserId = userId,
                            TargetDate = DateTime.Now,
                            TargetType = targetType,
                            Command = (int)Entity.Data.TaskCommand.Editing,
                        };
                        task.StateEnum = Entity.Data.TaskState.Created;
                    }
                }
            });
        }
        static void SetTime(Guid userId)
        {
            BCT.Execute(d =>
            {
                var userSettings = SettingsHelper.GetSettings(userId);
                userSettings.LastTimeGeneration = DateTime.Now;
                userSettings.NextTaskGeneration = userSettings.LastTimeGeneration.Value.AddHours(userSettings.TaskGenerationPeriod);

                d.BulletinDb.SaveChanges();
            });
        }
    }
}