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

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Добавить буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------

        public abstract void AddBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Отредактировать буллетины </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="packages"> Пакет буллетинов </param>
        ///-------------------------------------------------------------------------------------------------

        public abstract void EditBulletins(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);

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

        public abstract void GetBulletinList(AccessPackage access);
        public abstract void GetBulletinDetails(IEnumerable<BulletinBridge.Data.BulletinPackage> packages);
    }
}
