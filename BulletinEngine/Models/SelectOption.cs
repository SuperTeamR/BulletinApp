﻿using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{ 
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Значение поля-селекта </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class SelectOption : EntityObjectALM<SelectOption, SelectOptionState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Наименование значения </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------
        public string Name { get; set; }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Код значения </summary>
        ///
        /// <value> The code. </value>
        ///-------------------------------------------------------------------------------------------------
        public string Code { get; set; }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор поля </summary>
        ///
        /// <value> The identifier of the field. </value>
        ///-------------------------------------------------------------------------------------------------
        public Guid GroupedFieldId { get; set; }
        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<SelectOption, SelectOptionState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<SelectOption, SelectOptionState>(SelectOptionState.Created, SelectOptionState.Handled, SetValueDefault)
        };
        protected override int GetStateValue(SelectOptionState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override SelectOption SetValueDefault(SelectOption arg1, SelectOption arg2)
        {
            arg1.Name = arg2.Name;
            arg1.Code = arg2.Code;
            arg1.GroupedFieldId = arg2.GroupedFieldId;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<SelectOption>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<SelectOption>>();
        #endregion
    }
    public enum SelectOptionState
    {
        Created = 0,
        Handled = 1,
        Error = 99,
    }
}