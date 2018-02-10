using BulletinBridge.Commands;
using BulletinBridge.Data;
using BulletinBridge.Data.Base;
using BulletinWebWorker.Containers.Base.BulletinPackage;
using FessooFramework.Tools.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinWebWorker.Containers
{
    public class WorkContainer
    {
        public void AssignWork(IEnumerable<DataObjectBase> collection, CommandApi api)
        {
            switch(api)
            {
                case CommandApi.Internal_GetBulletinWork:
                    AssignBulletinWork(collection);
                    break;
            }
        }

        void AssignBulletinWork(IEnumerable<DataObjectBase> collection)
        {
            //TODO - сделать группировку по бордам
            var bulletinContainer = BulletinPackageContainerList.Get(BoardIds.Avito);
            var stateCollection = collection.Cast<BulletinPackage>().GroupBy(q => q.State).Select(q => new { State = q.Key, Collection = q.ToList() }).ToList();
            foreach(var s in stateCollection)
            {
                switch(EnumHelper.GetValue<BulletinState>(s.State))
                {
                    case BulletinState.WaitPublication:
                        bulletinContainer.AddBulletins(s.Collection);
                        break;
                }
            }
        }
    }
}
