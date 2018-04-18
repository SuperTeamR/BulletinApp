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
        static bool IsInitialized;

        public static void Initialize()
        {
            if (IsInitialized) return;
            IsInitialized = true;
            BCT.ExecuteAsync(d =>
            {
                while(true)
                {
                    GenerateTasks();
                    Thread.Sleep(30000);
                }
            });
        }

        static readonly int TimeoutMinutes = 30;
        static void GenerateTasks()
        {
            BCT.Execute(d =>
            {
                CreatePublicationTasks();
                CreateAccessUnloading();
            });
        }

        static void CreatePublicationTasks()
        {
            BCT.Execute(d =>
            {
                var targetType = typeof(BulletinInstance).ToString();
                var tasks = d.BulletinDb.Tasks.Where(q => q.TargetType == targetType).Select(q => q.BulletinId).ToArray();

                var bulletins = d.BulletinDb.BulletinInstances.Where(q => !tasks.Contains(q.Id) && q.State == (int)BulletinInstanceState.WaitPublication).ToArray();
                if (bulletins.Length == 0) return;

                var accesses = d.BulletinDb.Accesses/*.Where(q => q.State == (int)AccessState.Activated)*/.ToArray();
                var cycles = bulletins.Length >= accesses.Length ? bulletins.Length / accesses.Length : 1;
                for (int i = 0; i < bulletins.Length; i++)
                {
                    var currentBulletin = bulletins[i];
                    var access = accesses[i % cycles];

                    var task = new Entity.Data.Task
                    {
                        BulletinId = currentBulletin.Id,
                        AccessId = access.Id,
                        TargetDate = DateTime.Now.Add(TimeSpan.FromMinutes(TimeoutMinutes * (i / cycles))),
                        TargetType = targetType,
                        Command = (int)Entity.Data.TaskCommand.Creation,
                    };
                    task.StateEnum = Entity.Data.TaskState.Created;
                }

                d.BulletinDb.SaveChanges();
            });
        }

        static void CreateAccessUnloading()
        {
            BCT.Execute(d =>
            {
                var targetType = typeof(Access).ToString();
                var tasks = d.BulletinDb.Tasks.Where(q => q.TargetType == targetType).Select(q => q.AccessId).ToArray();

                var accesses = d.BulletinDb.Accesses.Where(q => !tasks.Contains(q.Id) && q.State == (int)BulletinInstanceState.Created).ToArray();
                if (accesses.Length == 0) return;

                for (int i = 0; i < accesses.Length; i++)
                {
                    var task = new Entity.Data.Task
                    {
                        AccessId = accesses[i].Id,
                        TargetType = targetType,
                        Command = (int)Entity.Data.TaskCommand.Checking,
                    };
                    task.StateEnum = Entity.Data.TaskState.Created;
                }

                d.BulletinDb.SaveChanges();
            });
        }
    }
}
