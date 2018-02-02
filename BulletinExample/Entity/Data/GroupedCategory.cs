﻿using FessooFramework.Objects.Data;
using System;

namespace Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Категория, используемая группой </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GroupedCategory : EntityObject
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор шаблона категории </summary>
        ///
        /// <value> The identifier of the category. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid CategoryId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор шаблона группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }
    }
}