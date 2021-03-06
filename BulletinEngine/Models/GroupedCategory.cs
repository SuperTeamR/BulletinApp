﻿using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Категория, используемая группой </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GroupedCategory : EntityObjectALM<GroupedCategory, GroupedCategoryState>
    {
        #region Entity properties
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
        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<GroupedCategory, GroupedCategoryState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<GroupedCategory, GroupedCategoryState>(GroupedCategoryState.Created, GroupedCategoryState.Handled, SetValueDefault)
        };
        protected override int GetStateValue(GroupedCategoryState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override GroupedCategory SetValueDefault(GroupedCategory arg1, GroupedCategory arg2)
        {
            arg1.CategoryId = arg2.CategoryId;
            arg1.GroupId = arg2.GroupId;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<GroupedCategory>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<GroupedCategory>>();
        #endregion
    }

    public enum GroupedCategoryState
    {
        Created = 0,
        Handled = 1,
        Error = 99
    }
}