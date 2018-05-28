using BulletinEngine.Core;
using BulletinEngine.Entity.Data;
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
           AutoPublicate -login SuperKazah -brand IPhone -model 7+ 128gb - публикует буллетин на базе шаблона с активацией ночью
           CreateBulletin -login MegaArthur -brand IPhone -model 7+ 128gb - создает новый буллетин
           NextInstances -login MegaArthur - публикация новых инстанций для буллетинов
           CreateAccess - login MegaArthur -forward -n 4
            ";

        static void Main(string[] args)
        {
            BCT.Execute(c => { });

            Console.Write("Command> ");
#if DEBUG
            //var commands = CreateTestCommands();
            //foreach (var command in commands)
            //    HandleArgs(command);
            HandleArgs(args);

#else
         HandleArgs(args);
#endif


        }

        static void HandleArgs(string[] args)
        {
            var command = "Help";
            if (args != null && args.Any())
                command = args[0];
            var parameters = string.Empty;
            if (args != null && args.Length > 1)
                parameters = string.Join(" ", args.Skip(1).ToArray());
            HandleCommand(command, parameters);
        }

        static IEnumerable<string[]> CreateTestCommands()
        {

            #region StorePhone2
            //var list = new List<string>
            //{
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 5 16gb -modifier белый -price 7000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 5 16gb -modifier черный -price 8000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 4s 16gb -modifier белый -price 4500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 4s 16gb -modifier черный -price 4500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 5s 16gb -modifier белый -price 7500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 5s 16gb -modifier золотой -price 8500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 5s 16gb -modifier черный -price 8500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 16gb -modifier золотой -price 10500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 16gb -modifier черный -price 10500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 64gb -modifier золотой -price 12000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 64gb -modifier черный -price 12000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 64gb -modifier белый/серебристый -price 13000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 64gb -modifier золотой -price 13000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6 64gb -modifier черный/серый -price 13000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 16gb -modifier розовый -price 12500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 16gb -modifier черный/серый -price 12000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 32gb -modifier золотой -price 13000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 64gb -modifier черный/серый -price 12000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 16gb -modifier розовый -price 15500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 16gb -modifier черный/серый -price 15500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 64gb -modifier белый/серебристый -price 15000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 64gb -modifier золотой -price 15000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 64gb -modifier черный/серый -price 15000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s 128gb -modifier черный/серый -price 17000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 6s+ 16gb -modifier белый/серебристый -price 15000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 7 32gb -modifier золотой -price 23000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 7 128gb -modifier черный/серый -price 25000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 7 32gb -modifier розовый -price 23000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model 7 32gb -modifier черный/серый -price 23000",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model SE 64gb -modifier черный -price 12500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model SE 64gb -modifier золотой -price 12500",
            //    "CreateBulletin -login StorePhone2 -brand IPhone -model SE 64gb -modifier розовый -price 12500",
            //};
            #endregion
            #region MegaArthur2
            //var list = new List<string>
            //{
            //    //"CreateBulletin -login MegaArthur2 -brand IPhone -model 6 64gb -price 7000",
            //    //"CreateBulletin -login MegaArthur2 -brand IPhone -model 7 32gb -price 8000",
            //    "CreateBulletin -login MegaArthur2 -brand IPhone -model SE -price 4500",
            //};

            #endregion
            #region MegaArthur3
            var list = new List<string>
            {
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 5 16gb -modifier белый -price 7000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 5 16gb -modifier черный -price 8000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 4s 16gb -modifier белый -price 4500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 4s 16gb -modifier черный -price 4500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 5s 16gb -modifier белый -price 7500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 5s 16gb -modifier золотой -price 8500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 5s 16gb -modifier черный -price 8500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 16gb -modifier золотой -price 10500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 16gb -modifier черный -price 10500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 64gb -modifier золотой -price 12000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 64gb -modifier черный -price 12000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 64gb -modifier белый/серебристый -price 13000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 64gb -modifier золотой -price 13000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6 64gb -modifier черный/серый -price 13000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 16gb -modifier розовый -price 12500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 16gb -modifier черный/серый -price 12000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 32gb -modifier золотой -price 13000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 64gb -modifier черный/серый -price 12000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 16gb -modifier розовый -price 15500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 16gb -modifier черный/серый -price 15500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 64gb -modifier белый/серебристый -price 15000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 64gb -modifier золотой -price 15000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 64gb -modifier черный/серый -price 15000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s 128gb -modifier черный/серый -price 17000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 6s+ 16gb -modifier белый/серебристый -price 15000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 7 32gb -modifier золотой -price 23000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 7 128gb -modifier черный/серый -price 25000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 7 32gb -modifier розовый -price 23000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model 7 32gb -modifier черный/серый -price 23000",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model SE 64gb -modifier черный -price 12500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model SE 64gb -modifier золотой -price 12500",
                //"CreateBulletin -login MegaArthur3 -brand IPhone -model SE 64gb -modifier розовый -price 12500",
            };
            #endregion

            return list.Select(q => q.Split());
        }

        static void HandleCommand(string command, string parameters)
        {

            //-login
            var login = FindParameter("login", parameters);
            //-id
            var id = FindParameter("id", parameters);
            //-brand
            var brand = FindParameter("brand", parameters);
            //-model
            var model = FindParameter("model", parameters);
            //-modifier
            var modifier = FindParameter("modifier", parameters);
            //-price
            var price = FindParameter("price", parameters);
            //-isForwarding
            var isForwarding = HasParameter("forward", parameters);
            //-n
            var n = FindParameter("n", parameters);

            Console.Write(command + " " + parameters);
            BCT.Execute(d =>
            {
                switch (command)
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
                        if (string.IsNullOrEmpty(id))
                            TaskManager.Helpers.BulletinHelper.AutoPublicateBulletin(login, brand, model, modifier, price);
                        else
                            TaskManager.Helpers.BulletinHelper.AutoPublicateBulletin(new Guid(id), brand, model, modifier, price);
                        break;
                    case "NextInstances":
                        if (!string.IsNullOrEmpty(login))
                            AutoHelper.NextInstances(login);
                        break;
                    case "CreateBulletin":
                        TaskManager.Helpers.BulletinHelper.CreateBulletin(login, brand, model, modifier, price);
                        break;
                    case "CollectMessages":
                        if (!string.IsNullOrEmpty(login))
                            MessageHelper.RunCollectingMessages(login);
                        break;
                    case "SendStatistics":
                        if (!string.IsNullOrEmpty(login))
                            EmailHelper.SendStatistics(login);
                        break;
                    case "CreateAccess":
                        if (!string.IsNullOrEmpty(login))
                        {
                            var number = 1;
                            if (!string.IsNullOrEmpty(n))
                            {
                                int.TryParse(n, out number);
                            }
                            AccessTaskHelper.CreateAccess(login, isForwarding, number);
                        }
                        break;
                }
            });


        }

        static string FindParameter(string parameter, string str)
        {
            var pattern = $".*?-{parameter} (.*?)($| -.*?)";
            return RegexHelper.GetValue(pattern, str);
        }
        static bool HasParameter(string parameter, string str)
        {
            var pattern = $".*?-({parameter})($| -.*?)";
            return string.Equals(RegexHelper.GetValue(pattern, str), parameter);
        }
    }
}
