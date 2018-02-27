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
        public static void AssignBulletinWork(IEnumerable<BulletinPackage> collection)
        {
            //TODO - сделать группировку по бордам
            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);

            var withoutValues = collection.Cast<BulletinPackage>().Where(q => (q.ValueFields == null || q.ValueFields.Count == 0)).ToArray();
            if(withoutValues.Length > 0)
                bulletinContainer.GetBulletinDetails(withoutValues);

            var stateCollection = collection.Cast<BulletinPackage>().Where(q => (q.ValueFields != null && q.ValueFields.Count != 0)).GroupBy(q => q.State).Select(q => new { State = q.Key, Collection = q.ToList() }).ToList();
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
                    case BulletinState.OnModeration:
                        bulletinContainer.CheckModerationState(s.Collection);
                        break;
                }
            }
        }

        public static void AssignProfileWork(IEnumerable<AccessPackage> collection)
        {
            //TODO - сделать группировку по бордам
            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);
            var accesses = collection.Cast<AccessPackage>().ToArray();
            bulletinContainer.GetBulletinList(accesses);
        }
    }
}
