﻿using BulletinEngine.Core;
using BulletinHub.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Helpers;

namespace TaskManager
{
    class Program
    {
        const string Help = @"Доступные команды 
           RunStatisticsTasks -login StorePhone - запускает задачу по сбору статистики по логину юзера
           ComputeStatisticsTasks -login StorePhone - запускает пересчет статистики в БД по логину юзера
           GetCalls -login StorePhone - получает звонки
           AutoPublicate -id 438B3DF4-2F5E-423C-938E-03C1E11AABBE - публикует существующий буллетин по Guid с активацией ночью
           AutoPublicate -login StorePhone -title IPhone 32gb Black - публикует существующий буллетин по Guid с активацией ночью
            ";

        static void Main(string[] args)
        {
            BCT.Execute(c => { });

            Console.Write("Command> ");
            var command = "Help";
            if (args != null && args.Any())
                command = args[0];

            var parameters = string.Empty;
            if (args != null && args.Length > 1)
                parameters = string.Join(" ", args.Skip(1).ToArray());

            //-login
            var login = FindParameter("login", parameters);
            //-id
            var id = FindParameter("id", parameters);
            //-title
            var title = FindParameter("title", parameters);

            Console.Write(command + " " + parameters);
            BCT.Execute(d =>
            {
                switch(command)
                {
                    case "Help":
                        Console.WriteLine(Help);
                        Console.ReadLine();
                        break;
                    case "RunStatisticsTasks":
                        StatisticsHelper.RunUserStatisticsTask(login);
                        break;
                    case "ComputeStatistics":
                        StatisticsHelper.ComputeUserStatistics(login);
                        break;
                    case "GetCalls":
                        StatisticsHelper.GetCallsStatistics(login);
                        break;
                    case "AutoPublicate":
                        if(string.IsNullOrEmpty(id))
                            TaskManager.Helpers.BulletinHelper.AutoPublicateBulletin(login, title);
                        else
                            TaskManager.Helpers.BulletinHelper.AutoPublicateBulletin(new Guid(id));
                        break;
                }
            });
        }

        static string FindParameter(string parameter, string str)
        {
            var pattern = $".*?-{parameter} (.*?)($| -.*?)";
            return RegexHelper.GetValue(pattern, str);
        }
    }
}
