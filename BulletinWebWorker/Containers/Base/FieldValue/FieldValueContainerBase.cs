using BulletinBridge.Data;
using System;
using System.Collections.Generic;

namespace BulletinWebWorker.Containers.Base.FieldValue
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Управляет установкой и получением значений на борде </summary>
    ///
    /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    abstract class FieldValueContainerBase
    {
        public abstract Guid Uid { get; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Установить значение для поля </summary>
        ///
        /// <remarks>   SV Milovanov, 14.02.2018. </remarks>
        ///
        /// <param name="fields">   The fields. </param>
        /// <param name="name">     The name. </param>
        /// <param name="value">    The value. </param>
        ///-------------------------------------------------------------------------------------------------

        public abstract void SetFieldValue(Dictionary<string, FieldPackage> fields, string name, string value);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Получить значение для поля </summary>
        ///
        /// <remarks>   SV Milovanov, 19.02.2018. </remarks>
        ///
        /// <param name="fields">   The fields. </param>
        /// <param name="name">     The name. </param>
        ///-------------------------------------------------------------------------------------------------

        public abstract string GetFieldValue(Dictionary<string, FieldPackage> fields, string name);
    }
}
