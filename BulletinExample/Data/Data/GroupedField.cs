using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinExample.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Поле, используемое группой </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GroupedField : EntityObjectALM<GroupedField, GroupedFieldState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Идентификатор шаблона поля  </summary>
        ///
        /// <value> The identifier of the guild. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid FieldId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор поля в DOM </summary>
        ///
        /// <value> The identifier of the HTML. </value>
        ///-------------------------------------------------------------------------------------------------
        public string HtmlId { get; set; }
        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<GroupedField, GroupedFieldState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<GroupedField, GroupedFieldState>(GroupedFieldState.Created, GroupedFieldState.Handled, Handled),
        };
        protected override IEnumerable<GroupedFieldState> DefaultState => new[]
        {
            GroupedFieldState.Error
        };


        protected override int GetStateValue(GroupedFieldState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private GroupedField Handled(GroupedField arg1, GroupedField arg2)
        {
            arg1.FieldId = arg2.FieldId;
            arg1.GroupId = arg2.GroupId;
            arg1.HtmlId = arg2.HtmlId;

            return arg1;
        }
        #endregion

    }

    public enum GroupedFieldState
    {
        Created = 0,
        Handled = 1,
        Error = 99

    }
}
