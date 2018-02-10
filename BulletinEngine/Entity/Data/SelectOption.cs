using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

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

        protected override IEnumerable<EntityObjectALMConfiguration<SelectOption, SelectOptionState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<SelectOption, SelectOptionState>(SelectOptionState.Created, SelectOptionState.Handled, Handled)
        };

        private SelectOption Handled(SelectOption arg1, SelectOption arg2)
        {
            return arg1;
        }

        protected override IEnumerable<SelectOptionState> DefaultState => new[]
        {
            SelectOptionState.Error
        };

        protected override int GetStateValue(SelectOptionState state)
        {
            return (int)state;
        }
    }

    public enum SelectOptionState
    {
        Created = 0,
        Handled = 1,
        Error = 99,
    }
}
