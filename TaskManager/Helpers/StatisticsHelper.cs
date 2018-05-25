using BulletinEngine.Core;
using BulletinHub.Helpers;
using BulletinHub.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class StatisticsHelper
    {
        /// <summary>
        /// Собирает статистику по переадресации
        /// </summary>
        /// <param name="userId"></param>
        public static bool GetCallsStatistics(string userLogin)
        {
            var result = false;
            BCT.Execute(d =>
            {
                var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.Login == userLogin);
                if (user == null) return;
                var userId = user.Id;
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

                result = true;
            });
            return result;
        }

        /// <summary>
        /// Запускает задачи по сбору статистики для пользователя
        /// </summary>
        /// <param name="userId"></param>
        public static bool RunUserStatisticsTask(string userLogin)
        {
            var result = false;
            BCT.Execute(d =>
            {
                var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.Login == userLogin);
                if (user == null) return;
                var userId = user.Id;
                // Создаем задачи на сбор статистики с аккаунтов
                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.State != (int)FessooFramework.Objects.Data.DefaultState.Created).ToArray();
                foreach (var access in accesses)
                    TaskHelper.CreateAccessStatistics(access);

                // Создаем задачи на сбор статистики с инстанций
                var bulletinIds = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).Select(q => q.Id).ToArray();
                var instances = d.BulletinDb.BulletinInstances.Where(q => bulletinIds.Contains(q.BulletinId) && q.Url != null).ToArray();
                foreach (var instance in instances)
                    TaskHelper.CreateInstanceStatistics(userId, instance);

                result = true;
            });
            return result;
        }

        /// <summary>
        /// Перерасчитывает статистику для пользователя
        /// </summary>
        /// <param name="userId"></param>
        public static bool ComputeUserStatistics(string userLogin)
        {
            var result = false;
            BCT.Execute(d =>
            {
                var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.Login == userLogin);
                if (user == null) return;
                var userId = user.Id;

                var bulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).ToArray();
                var bulletinIds = bulletins.Select(q => q.Id).ToArray();
                foreach (var bulletin in bulletins)
                {
                    var bulletinViews = d.BulletinDb.BulletinInstances.Where(q => q.BulletinId == bulletin.Id).ToArray().Sum(q => q.Views);
                    bulletin.Views = bulletinViews;
                    bulletin.StateEnum = bulletin.StateEnum;
                }
                d.SaveChanges();

                var accesses = d.BulletinDb.Accesses.Where(q => q.UserId == userId && q.State != (int)FessooFramework.Objects.Data.DefaultState.Created).ToArray();
                var totalViews = accesses.Sum(q => q.Views);
                var totalMessages = accesses.Sum(q => q.Messages);
                var totalCalls = accesses.Sum(q => q.Calls);
                var totalBulletins = bulletins.Count();
                var totalInstances = d.BulletinDb.BulletinInstances.Count(q => q.Url != null && q.ActivationDate != null && bulletinIds.Any(qq => qq == q.BulletinId));


                var userStat = d.BulletinDb.UserStatistics.FirstOrDefault(q => q.UserId == userId);
                if (userStat == null)
                {
                    userStat = new BulletinHub.Models.UserStatistics();
                    userStat.UserId = userId;
                }
                userStat.TotalViews = totalViews;
                userStat.TotalMessages = totalMessages;
                userStat.TotalCalls = totalCalls;
                userStat.TotalProducts = totalBulletins;
                userStat.TotalInstances = totalInstances;
                userStat.StateEnum = FessooFramework.Objects.Data.DefaultState.Enable;

                d.SaveChanges();
            });
            return result;
        }


        public struct ProductCount
        {
            public int BulletinCount { get; set; }
            public int InstanceCount { get; set; }
        }

        public static ProductCount GetProductStatisticsByPeriod(Guid userId, DateTime from, DateTime until)
        {
            var result = default(ProductCount);
            BCT.Execute(d =>
            {
                var bulletins = d.BulletinDb.Bulletins.Where(q => q.UserId == userId).ToArray();
                var bulletinIds = bulletins.Select(q => q.Id);

                var instances = d.BulletinDb.BulletinInstances.Where(q => q.Url != null
                    && q.ActivationDate != null
                    && bulletinIds.Any(qq => qq == q.BulletinId)
                    && q.ActivationDate >= from && q.ActivationDate <= until).ToArray();

                var bulletinCount = instances.GroupBy(q => q.BulletinId).Count();
                var instanceCount = instances.Count();

                result = new ProductCount
                {
                    BulletinCount = bulletinCount,
                    InstanceCount = instanceCount
                };
            });
            return result;
        }

    }
}
