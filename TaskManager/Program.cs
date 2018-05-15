using BulletinEngine.Core;
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
           RunStatisticsTasks StorePhone - запускает задачу по сбору статистики по логину юзера
           ComputeStatisticsTasks StorePhone - запускает пересчет статистики в БД по логину юзера
           GetCalls StorePhone - получает звонки
            ";

        static void Main(string[] args)
        {
            BCT.Execute(c => { });

            Console.Write("Command> ");
            var command = "Help";
            if (args != null && args.Any())
                command = args[0];

            var parameter = string.Empty;
            if (args != null && args.Length > 1)
                parameter = args[1];

            Console.Write(command + " " + parameter);
            BCT.Execute(d =>
            {
                switch(command)
                {
                    case "Help":
                        Console.WriteLine(Help);
                        Console.ReadLine();
                        break;
                    case "RunStatisticsTasks":
                        StatisticsHelper.RunUserStatisticsTask(parameter);
                        break;
                    case "ComputeStatistics":
                        StatisticsHelper.ComputeUserStatistics(parameter);
                        break;
                    case "GetCalls":
                        StatisticsHelper.GetCallsStatistics(parameter);
                        break;
                }
            });
        }
    }
}
