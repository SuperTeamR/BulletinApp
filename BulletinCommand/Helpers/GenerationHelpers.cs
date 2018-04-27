using BulletinEngine.Contexts;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using FessooFramework.Tools.DCT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinCommand.Helpers
{
    public static class GenerationHelpers
    {
        public static void GenerationClearData()
        {
            Console.WriteLine($"GenerationClearData execute start");
            BCT.Execute(c =>
            {
                //1. Проверка аккаунтов - на авторизацию и активность
                foreach (var access in c.BulletinDb.Accesses.ToArray())
                {
                    access.GenerationCheckNext = null;
                    access.StateEnum = access.StateEnum;
                }
                c.SaveChanges();
                //2. Сбор данных с аккаунта - сообщения и буллетины(единожды)
                //3. Публикация инстанций объявлений
                foreach (var bulletin in c.BulletinDb.Bulletins.ToArray())
                {
                    bulletin.GenerationCheckNext = null;
                    bulletin.StateEnum = bulletin.StateEnum;
                }
                c.SaveChanges();
                //4. Редактирование существующих инстанций
                //5. Сбор аналитики по инстанциям
                c.TempDB.Tasks.RemoveRange(c.TempDB.Tasks.ToArray());
                c.SaveChanges();
            });
            Console.WriteLine($"GenerationClearData execute complete");
        }


        public static void GenerationFull()
        {
            Console.WriteLine($"GenerationFull execute start");
            BCT.Execute(c =>
            {
                //1. Проверка аккаунтов - на авторизацию и активность
                GenerationAccessCheck();
                //2. Сбор данных с аккаунта - сообщения и буллетины(единожды)
                //3. Публикация инстанций объявлений
                GenerationBulletinsPublish();
                //4. Редактирование существующих инстанций
                //5. Сбор аналитики по инстанциям
            });
            Console.WriteLine($"GenerationFull execute complete");
        }

        public static void GenerationAccessCheck()
        {
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                var acceses = c.BulletinDb.Accesses.Where(q => q.GenerationCheckNext == null || date > q.GenerationCheckNext).ToArray();
                foreach (var access in acceses)
                {
                    var tasks = c.TempDB.Tasks.Where(q => q.AccessId == access.Id && q.Command == (int)BulletinHub.Entity.Data.TaskCommand.AccessCheck);
                    if (tasks != null)
                        CommandTaskHelper.Remove(tasks);
                    access.SetGenerationCheck();
                    CommandTaskHelper.CreateAccessCheck(access.UserId, access.Id);
                }
            });
        }

        public static void GenerationBulletinsPublish()
        {
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                var bulletins = c.BulletinDb.Bulletins.Where(q => 
                (q.GenerationCheckNext == null || date > q.GenerationCheckNext)).ToArray();
                foreach (var bulletinsByUser in bulletins.GroupBy(q => q.UserId).ToArray())
                {
                    var buls = bulletinsByUser.OrderBy(q => q.DatePublication).ToArray();
                    var instancs = new List<BulletinInstance>();
                    foreach (var bulletin in bulletinsByUser)
                    {
                        var tasks = c.TempDB.Tasks.Where(q => (q.State == (int)BulletinHub.Entity.Data.TaskState.Created || q.State == (int)BulletinHub.Entity.Data.TaskState.Enabled) && q.BulletinId == bulletin.Id).ToArray();
                        if (tasks != null && tasks.Any())
                            CommandTaskHelper.Remove(tasks);
                        var i = BulletinHelper.CreateInstance(bulletin);
                        if (i.Any())
                        {
                            instancs.AddRange(i);
                            bulletin.SetGenerationCheck();
                        }
                    }
                    if (instancs.Any())
                    {
                        var datePublish = date;
                       
                        foreach (var instByBoard in instancs.GroupBy(q=>q.BoardId).ToArray())
                        {
                            var accesses = c.BulletinDb.Accesses.Where(q => q.UserId == bulletinsByUser.Key && q.BoardId == instByBoard.Key && q.State == (int)FessooFramework.Objects.Data.DefaultState.Enable).ToArray();
                            if (accesses.Any())
                            {
                                var accessCount = accesses.Count();
                                var currentAccess = 0;
                                foreach (var instance in instByBoard)
                                {
                                    instance.AccessId = accesses[0].Id;
                                    CommandTaskHelper.CreateInstancePublication(bulletinsByUser.Key, instance, datePublish);
                                    datePublish = datePublish.AddMinutes(5);

                                    currentAccess += 1;
                                    if (currentAccess >= accessCount)
                                    {
                                        currentAccess = 0;
                                        datePublish = datePublish.AddMinutes(30);
                                    }
                                }
                            }
                        }
                    }
                }
                c.SaveChanges();
            });
        }
    }
}
