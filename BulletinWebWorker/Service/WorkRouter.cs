using BulletinBridge.Commands;
using BulletinBridge.Data;
using BulletinWebWorker.Containers.Base.BulletinPackage;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BulletinWebWorker.Containers
{
    static class WorkRouter
    {
        //public void AssignWork(IEnumerable<CacheObject> collection, CommandApi api)
        //{
        //    switch(api)
        //    {
        //        case CommandApi.Internal_GetBulletinWork:
        //            AssignBulletinWork(collection);
        //            break;
        //        case CommandApi.Internal_GetProfileWork:
        //            AssignProfileWork(collection);
        //            break;
        //    }
        //}

        public static void AssignBulletinWork(IEnumerable<BulletinPackage> collection)
        {
            //TODO - сделать группировку по бордам
            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);
            var stateCollection = collection.Cast<BulletinPackage>().GroupBy(q => q.State).Select(q => new { State = q.Key, Collection = q.ToList() }).ToList();
            foreach (var s in stateCollection)
            {
                switch (EnumHelper.GetValue<BulletinState>(s.State))
                {
                    case BulletinState.WaitPublication:
                        bulletinContainer.AddBulletins(s.Collection);
                        break;
                    case BulletinState.Edited:
                        bulletinContainer.EditBulletins(s.Collection);
                        break;
                }
            }
        }

        public static void AssignProfileWork(IEnumerable<CacheObject> collection)
        {
            //TODO - сделать группировку по бордам
            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);
            var access = collection.Cast<AccessPackage>().FirstOrDefault();
            bulletinContainer.GetBulletins(access);
        }

    }
}
