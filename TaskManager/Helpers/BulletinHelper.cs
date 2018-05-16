using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
using BulletinHub.Helpers;
using BulletinHub.Models;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class BulletinHelper
    {
        static string[] forbiddenWords = new[]
       {
            "запчасти",
        };

        /// <summary>
        /// Создаем буллетину по шаблону, запускаем задачи на публикацию и активацию
        /// </summary>
        /// <param name="userLogin"></param>
        /// <param name="title"></param>
        public static void AutoPublicateBulletin(string userLogin, string title)
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

                //Находим подходящий шаблон
                var template = ChooseTemplate(title);
                if(template == null)
                {
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Нет подходящего шаблона для Title:{title}");
                    return;
                }
                //Добавляем новый буллетин в БД
                string cardCategory1 = "Бытовая электроника";
                string cardCategory2 = "Телефоны";
                string cardCategory3 = "iPhone";
                var groupHash = StringToSha256String(cardCategory1, cardCategory2, cardCategory3, null, null);
                var bulletin = AddAvitoByTemplate(userId, template, groupHash);
                if(bulletin == null)
                {
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Ошибка при создании буллетина");
                    return;
                }
                //Запускаем задачи на публикацию и активацию
                CreatePublicationTasks(bulletin);
            });
        }

        /// <summary>
        /// Накидываем шаблон на существующий буллетин, запускаем задачи на публикацию и активацию
        /// </summary>
        /// <param name="bulletinId"></param>
        public static void AutoPublicateBulletin(Guid bulletinId)
        {
            BCT.Execute(d =>
            {
                var bulletin = d.BulletinDb.Bulletins.FirstOrDefault(q => q.Id == bulletinId);
                if (bulletin == null) return;

                ChooseTemplate(bulletin);
                CreatePublicationTasks(bulletin);
            });
        }


        static Bulletin AddAvitoByTemplate(Guid userId, BulletinTemplate template, string groupHash)
        {
            var result = default(Bulletin);
            BCT.Execute(d =>
            {
                var group = BCT.Context.BulletinDb.Groups.FirstOrDefault(q => q.Hash == groupHash);
                if(group == null)
                {
                    ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Группа с хэшем:{groupHash} не найдена");
                    return;
                }

                result = new Bulletin();
                result.GroupId = group.Id;
                result.Title = template.Title;
                result.Description = template.Description;
                result.Price = template.Price.ToString();
                result.Images = template.Images;
                result.UserId = userId;
                result.StateEnum = BulletinState.Created;

                d.SaveChanges();
            });
            return result;
        }

        static BulletinTemplate ChooseTemplate(string pattern)
        {
            var result = default(BulletinTemplate);
            BCT.Execute(d =>
            {
                var templates = d.TempDB.BulletinTemplate.Where(q => q.IsIndividualSeller && q.State != (int)DefaultState.Disable).ToArray();
                // Исключаем шаблоны с запрещенными словами
                var temp = templates.Where(q => forbiddenWords.All(x => !q.Description.ToLower().Contains(x.ToLower()))
                && forbiddenWords.All(x => !q.Title.Contains(x))).ToArray();

                // Фильтруем шаблоны по словам в тайтле
                var words = pattern.ToLower().Split(' ');
                result = temp.Where(q => words.All(x => q.Title.ToLower().Contains(x))).FirstOrDefault();
                if(result != null)
                {
                    result.StateEnum = DefaultState.Disable;
                    d.SaveChanges();
                }
            });
            return result;
        }

        static void ChooseTemplate(Bulletin bulletin)
        {
            BCT.Execute(d =>
            {
                var chosenTemplate = ChooseTemplate(bulletin.Title);
                if(chosenTemplate != null)
                {
                    bulletin.Title = chosenTemplate.Title;
                    bulletin.Description = chosenTemplate.Description;
                    bulletin.Images = chosenTemplate.Images;
                    bulletin.StateEnum = bulletin.StateEnum;

                    d.SaveChanges();
                }
            });
        }

        static void CreatePublicationTasks(Bulletin bulletin)
        {
            BCT.Execute(d =>
            {
                var tasks = d.TempDB.Tasks.Where(q => (q.State == (int)BulletinHub.Entity.Data.TaskState.Created || q.State == (int)BulletinHub.Entity.Data.TaskState.Enabled) && q.BulletinId == bulletin.Id && q.Command == (int)BulletinHub.Entity.Data.TaskCommand.InstancePublication).ToArray();
                if (tasks != null && tasks.Any())
                    TaskHelper.Remove(tasks);
                var instances = BulletinHub.Helpers.BulletinHelper.CreateInstance(bulletin);
                if (instances.Any())
                {
                    bulletin.SetGenerationCheck();
                    var datePublish = DateTime.Now;

                    foreach (var instance in instances)
                    {
                        var access = BulletinEngine.Helpers.AccessHelper.GetFreeAccess(bulletin.UserId, instance.BoardId, instance.BulletinId);
                        if (access == null)
                        {
                            ConsoleHelper.SendMessage($"AvitoPublicateBulletin => Не найден свободный доступ для буллетина {bulletin.Id}");
                            continue;
                        }
                            
                        instance.AccessId = access.Id;
                        instance.StateEnum = instance.StateEnum;
                        d.SaveChanges();
                        TaskHelper.CreateInstancePublication(bulletin.UserId, instance, datePublish);
                        var now = DateTime.Now;
                        var activationDate = now.Date.AddDays(1);
                        activationDate.Date.Subtract(now);
                        TaskHelper.CreateActivateInstance(bulletin.UserId, instance, activationDate);
                    }
                }
            });
        }

        /// <summary>
        /// Конвертирую строку в Hash строку шифрованную с помощью 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string StringToSha256String(params string[] str)
        {
            var temp = string.Empty;
            foreach (var s in str)
            {
                temp += s ?? string.Empty;
            }
            return CryptographyHelper.StringToSha256String(temp);
        }
    }
}
