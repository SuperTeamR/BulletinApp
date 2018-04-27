//using BulletinBridge.Data;
//using BulletinWebDriver.ServiceHelper;
//using BulletinWebWorker.Containers.Base.BulletinPackage;
//using FessooFramework.Tools.DCT;
//using FessooFramework.Tools.Helpers;
//using System.Collections.Generic;
//using System.Linq;

//namespace BulletinWebDriver
//{
//    ///-------------------------------------------------------------------------------------------------
//    /// <summary>   Управляет тасками WebWorker </summary>
//    ///
//    /// <remarks>   SV Milovanov, 09.02.2018. </remarks>
//    ///-------------------------------------------------------------------------------------------------
//    static class WebWorkerManager
//    {
//        internal static void Execute()
//        {
//            DCT.Execute(d =>
//            {
//                AskForWork();
//            });
//        }
//        static void AskForWork()
//        {
//            DCT.Execute(d =>
//            {
             

//                ServerHelper.GetNewTasks(AskForWorkCallback);
//            });
//        }
//        static void AskForWorkCallback(IEnumerable<TaskCache> tasks)
//        {
//            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);

//            var bulletinWork = tasks.Where(q => q.TargetType == "BulletinEngine.Entity.Data.BulletinInstance").ToArray();
//            var accessWork = tasks.Where(q => q.TargetType == "BulletinEngine.Entity.Data.Access").ToArray();

//            var commandCollection = bulletinWork.GroupBy(q => q.Command).Select(q => new { Command = q.Key, Collection = q.Take(1).ToList() }).ToList();
//            foreach (var c in commandCollection)
//            {
//                switch (EnumHelper.GetValue<TaskCacheCommand>(c.Command))
//                {
//                    case TaskCacheCommand.Creation:
//                        bulletinContainer.AddBulletins(c.Collection);
//                        break;
//                    case TaskCacheCommand.Editing:
//                        bulletinContainer.EditBulletins(c.Collection);
//                        break;
//                    case TaskCacheCommand.Checking:
//                        bulletinContainer.CheckModerationState(c.Collection);
//                        break;
//                }
//            }
//            commandCollection = accessWork.GroupBy(q => q.Command).Select(q => new { Command = q.Key, Collection = q.Take(1).ToList() }).ToList();
//            foreach(var c in commandCollection)
//            {
//                switch(EnumHelper.GetValue<TaskCacheCommand>(c.Command))
//                {
//                    case TaskCacheCommand.Checking:
//                        bulletinContainer.GetBulletinList(c.Collection);
//                        break;
//                }
//            }
//        }
//    }
//}