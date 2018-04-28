using System;
using System.Collections.Generic;
using BulletinBridge.Data;

namespace BulletinWebWorker.Containers.Base
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет созданием и изменение буллетинов на борде </summary>
    ///
    /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    abstract class BulletinPackageContainerBase
    {
        public abstract Guid Uid { get; }


        public abstract void AddBulletins2(IEnumerable<BulletinBridge.Data.TaskCache_old> packages);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------

        [Obsolete]
        public abstract void AddBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Отредактировать буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------

        public abstract void EditBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);

        /// <summary>
        /// Перепубликация буллетинов
        /// </summary>
        /// <param name="packages"></param>
        public abstract void RepublicateBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);

        /// <summary>
        /// Клонировать буллетины
        /// </summary>
        /// <param name="packages"></param>
        public abstract void CloneBulletins(IEnumerable<AggregateBulletinPackage> packages);
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получает коллекцию буллетинов для заданного доступа </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="access">   Доступ к борде </param>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process the bulletins in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        public abstract void GetBulletinList2(IEnumerable<TaskCache_old> tasks);
        public abstract void GetBulletinDetails2(IEnumerable<TaskCache_old> tasks);
        public abstract void GetBulletinList(IEnumerable<AccessCache> accesses);
        public abstract void GetBulletinDetails(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);
        public abstract void CheckModerationState(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);
    }
}
