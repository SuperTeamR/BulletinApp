using BulletinEngine.Core;
using BulletinHub.Models;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Helpers
{
    static class EmailHelper
    {
        const string senderHost = "Mail.1cbit.ru";
        const int senderPort = 465;
        const string senderEmail = "help@1cbit.ru";
        const string senderPassword = "NMTL6nkvp";
        const string senderName = "Первый БИТ";

        public static void SendStatistics(string userLogin)
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
                var stat = d.BulletinDb.UserStatistics.FirstOrDefault(q => q.UserId == userId);
                if (stat == null)
                {
                    StatisticsHelper.ComputeUserStatistics(userLogin);
                }
                var template = EmailTemplateHelper.GetTemplate_UserStatistics(userId);
                var email = CreateEmail(template);
                SendEmail(email);
            });
        }

        static Email CreateEmail(TemplateStruct template)
        {
            var result = default(Email);
            BCT.Execute(d =>
            {
                if (string.IsNullOrEmpty(template.Destination)) return;

                var email = new Email();
                email.Title = template.Title;
                email.Body = template.Body;
                email.Destination = template.Destination;
                email.StateEnum = FessooFramework.Objects.Data.DefaultState.Created;
                d.SaveChanges();

                result = email;
            });
            return result;
        }


        static bool SendEmail(Email email)
        {
            var result = false;
            BCT.Execute((data) =>
            {
                var mail = new MailMessage
                {
                    IsBodyHtml = true,
                    Subject = email.Title,
                    Body = email.Body,
                    From = new System.Net.Mail.MailAddress(senderEmail, senderName)
                };
                using (var client = new System.Net.Mail.SmtpClient
                {
                    Host = senderHost,
                    Port = senderPort,
                    EnableSsl = true,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                })
                {
                    mail.To.Add(email.Destination);
                    client.Send(mail);
                    ConsoleHelper.SendMessage("SendStatistics => Письмо отправлено");
                }
                result = true;
            });
            return result;
        }

    }
}
