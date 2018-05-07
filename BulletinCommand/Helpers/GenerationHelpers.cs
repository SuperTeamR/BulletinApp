using BulletinEngine.Contexts;
using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Helpers;
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
                foreach (var access in c.BulletinDb.Accesses.Where(q => !q.HasBlocked).ToArray())
                {
                    access.GenerationCheckNext = null;
                    access.StateEnum = access.StateEnum;
                }
                c.SaveChanges();
                //2. Публикация инстанций объявлений
                foreach (var bulletin in c.BulletinDb.Bulletins.ToArray())
                {
                    bulletin.GenerationCheckNext = null;
                    bulletin.StateEnum = bulletin.StateEnum;
                }
                c.SaveChanges();


                //0. Сбор данных с аккаунта - сообщения и буллетины -
                //0. Редактирование существующих инстанций
                //0. Сбор аналитики по инстанциям

                //Clear tasks
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
                //GenerationAccessCheck();
                //2. Публикация инстанций объявлений
                GenerationBulletinsPublish();
                //3. Сбор данных под аккаунт
                //GenerationCollectorBulletinTemplate();

                //0. Сбор данных с аккаунта - сообщения и буллетины(единожды)
                //0. Редактирование существующих инстанций
                //0. Сбор аналитики по инстанциям
            });
            Console.WriteLine($"GenerationFull execute complete");
        }
        #region AccessCheck
        public static void GenerationAccessCheck()
        {
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                var acceses = c.BulletinDb.Accesses.Where(q => !q.HasBlocked).Where(q => q.GenerationCheckNext == null || date > q.GenerationCheckNext).ToArray();
                foreach (var access in acceses)
                    TaskAccessCheck(access);
            });
        }
        private static void TaskAccessCheck(Access access)
        {
            BCT.Execute(c =>
            {
                var tasks = c.TempDB.Tasks.Where(q => q.AccessId == access.Id && q.Command == (int)BulletinHub.Entity.Data.TaskCommand.AccessCheck);
                if (tasks != null)
                    TaskHelper.Remove(tasks);
                access.SetGenerationCheck();
                TaskHelper.CreateAccessCheck(access);
            });
        }
        #endregion
        #region BulletinsPublish
        /// <summary>
        /// Минуты
        /// </summary>
        public static int SprintPublicashionTimeout = 5;
        public static void GenerationBulletinsPublish()
        {
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                var bulletins = c.BulletinDb.Bulletins.Where(q =>
                (q.GenerationCheckNext == null || date > q.GenerationCheckNext)).ToArray();
                foreach (var bulletin in bulletins.ToArray())
                    BulletinHub.Helpers.BulletinHelper.BulletinPublicate(bulletin);
                c.SaveChanges();
            });
        }
       
      
        #endregion
        #region CollectorBulletinTemplate
        public static void GenerationCollectorBulletinTemplate()
        {
            BCT.Execute(c =>
            {
                var date = DateTime.Now;
                var bulletins = c.BulletinDb.Bulletins.Where(q => !q.HasRemoved).ToArray();
                var boards = c.BulletinDb.Boards.Where(q => !q.HasRemoved).ToArray();
                foreach (var board in boards)
                {
                    foreach (var bulletin in bulletins)
                    {
                        var tasks = c.TempDB.Tasks.Where(q => (q.State == (int)BulletinHub.Entity.Data.TaskState.Created || q.State == (int)BulletinHub.Entity.Data.TaskState.Enabled) && q.BulletinId == bulletin.Id && q.Command == (int)BulletinHub.Entity.Data.TaskCommand.BulletinTemplateCollector).ToArray();
                        if (tasks != null && tasks.Any())
                            TaskHelper.Remove(tasks);
                        TaskHelper.CreateBulletinTemplateCollector(bulletin.UserId, board.Id, bulletin, DateTime.Now);
                    }
                }
            });
        }
        #endregion
    }
}
