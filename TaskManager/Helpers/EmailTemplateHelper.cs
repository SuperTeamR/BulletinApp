using BulletinEngine.Core;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    struct TemplateStruct
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Destination { get; set; }
        public DateTime Create { get; set; }

        public static TemplateStruct New(string title, string body, string destination)
        {
            var result = new TemplateStruct();
            result.Title = title;
            result.Body = body;
            result.Destination = destination;
            result.Create = DateTime.Now;
            return result;
        }
    }

    static class EmailTemplateHelper
    {
        static readonly string title_UserStatistics = @"Статистика за {Timeperiod}";
        static readonly string body_UserStatistics = @"
            Просмотры: {Views}
            <br>Сообщения: {Messages}
            <br>Звонки: {Calls}
            <br>
            <br>Статистика публикаций
            <br>{Table}
            ";
        static readonly string table_UserStatistics = @"<table border=""1"">
                    <tr>
                        <th></th>
                        <th>За день</th>
                        <th>За неделю</th>
                        <th>За месяц</th>
                    </tr>
                    <tr>
                        <td>Продукты</td>
                        <td>{ProductDay}</td>
                        <td>{ProductWeek}</td>
                        <td>{ProductMonth}</td>
                    </tr>
                    <tr>
                        <td>Инстанции</td>
                        <td>{InstanceDay}</td>
                        <td>{InstanceWeek}</td>
                        <td>{InstanceMonth}</td>
                    </tr>
                  </table>
                ";


        public static TemplateStruct GetTemplate_UserStatistics(Guid userId)
        {
            var template = default(TemplateStruct);
            BCT.Execute(d =>
            {
                var user = d.MainDb.UserAccesses.FirstOrDefault(q => q.Id == userId);
                if (user == null) return;
                var profile = d.MainDb.UserProfiles.FirstOrDefault(q => q.Id == user.UserProfileId);
                if (profile == null) return;
                var stat = d.BulletinDb.UserStatistics.FirstOrDefault(q => q.UserId == userId);
                if (stat == null) return;

                var now = DateTime.Now;
                var title = title_UserStatistics
                    .Replace("{Timeperiod}", now.Date.ToShortDateString());

                var productYesterday = StatisticsHelper.GetProductStatisticsByPeriod(user.Id, now.Date.AddDays(-1), now.Date);
                var productWeek = StatisticsHelper.GetProductStatisticsByPeriod(user.Id, now.Date.AddDays(-7), now.Date);
                var productMonth = StatisticsHelper.GetProductStatisticsByPeriod(user.Id, now.Date.AddDays(-30), now.Date);

                var table = table_UserStatistics
                    .Replace("{ProductDay}", productYesterday.BulletinCount.ToString())
                    .Replace("{ProductWeek}", productWeek.BulletinCount.ToString())
                    .Replace("{ProductMonth}", productMonth.BulletinCount.ToString())
                    .Replace("{InstanceDay}", productYesterday.InstanceCount.ToString())
                    .Replace("{InstanceWeek}", productWeek.InstanceCount.ToString())
                    .Replace("{InstanceMonth}", productMonth.InstanceCount.ToString());

                var body = body_UserStatistics
                    .Replace("{Views}", stat.TotalViews.ToString())
                    .Replace("{Messages}", stat.TotalMessages.ToString())
                    .Replace("{Calls}", stat.TotalCalls.ToString())
                    .Replace("{Table}", table);

                template = TemplateStruct.New(title, body, profile.Email);
            });
            return template;
        }

    }
}
