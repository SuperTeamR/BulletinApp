using BulletinBridge.Models;
using BulletinEngine.Core;
using BulletinHub.Helpers;
using BulletinHub.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BulletinCommand.Helpers
{
    static class StatisticsHelper
    {
        public static void GetCallsStatistics(Guid userId)
        {
            BCT.Execute(d =>
            {
                var request = WebRequest.Create("https://onlinesim.ru/api/forwardingList.php?apikey=f02eb880d7930c4a6eec0b39bb893e36");
                request.Timeout = 5000;
                request.ContentType = "application/json";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        var json = reader.ReadToEnd();
                        var apiData = JObject.Parse(json);
                        var array = apiData["forwardingList"]["data"].Children().ToList();
                        foreach (var a in array)
                        {
                            var call = new Call();
                            call.UserId = userId;
                            call.VirtualNumber = a["number"].Value<string>();
                            call.ForwardNumber = a["forward_number"].Children().FirstOrDefault().Value<string>();
                            call.CallDate = a["created_at"].Value<DateTime>();
                            call.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
                        }
                        d.SaveChanges();
                    }

                }
            });
        }

        /// <summary>
        /// Запускает задачи по сбору статистики для пользователя
        /// </summary>
        /// <param name="userId"></param>
        public static void RunUserStatisticsTask(Guid userId)
        {
            BCT.Execute(d =>
            {
                // Создаем задачи на сбор статистики с аккаунтов
                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.StateEnum != FessooFramework.Objects.Data.DefaultState.Disable).ToArray();
                foreach (var access in accesses)
                    TaskHelper.CreateAccessStatistics(access);

                // Создаем задачи на сбор статистики с инстанций
                var bulletinIds = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).Select(q => q.Id).ToArray();
                var instances = d.BulletinDb.BulletinInstances.Where(q => bulletinIds.Contains(q.BulletinId) && q.Url != null).ToArray();
                foreach (var instance in instances)
                    TaskHelper.CreateInstanceStatistics(userId, instance);
            });
        }

        /// <summary>
        /// Перерасчитывает статистику для пользователя
        /// </summary>
        /// <param name="userId"></param>
        public static void ComputeUserStatistics(Guid userId)
        {
            BCT.Execute(d =>
            {
                var bulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).ToArray();
                foreach (var bulletin in bulletins)
                {
                    var bulletinViews = d.BulletinDb.BulletinInstances.Count(q => q.BulletinId == bulletin.Id);
                    bulletin.Views = bulletinViews;
                    bulletin.StateEnum = bulletin.StateEnum;
                }
                d.SaveChanges();

                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.StateEnum != FessooFramework.Objects.Data.DefaultState.Disable).ToArray();
                var totalViews = accesses.Sum(q => q.Views);
                var totalMessages = accesses.Sum(q => q.Messages);
                var totalCalls = accesses.Sum(q => q.Calls);

                var user = d.BulletinDb.UserStatistics.FirstOrDefault(q => q.UserId == userId);
                if (user == null)
                {
                    user = new BulletinHub.Models.UserStatistics();
                    user.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
                }
                user.TotalViews = totalViews;
                user.TotalMessages = totalMessages;
                user.TotalCalls = totalCalls;

                d.SaveChanges();
            });
        }

    }
}
