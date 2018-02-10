using BulletinExample.Entity.Data.Enums;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Достуная пользователю группа </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class UserGroup : EntityObjectALM<UserGroup, UserGroupState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<UserGroup, UserGroupState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Created, UserGroupState.Active, Active),
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Active, UserGroupState.Limited, Limited),
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Limited, UserGroupState.Active, Active),
        };

        private UserGroup Limited(UserGroup arg1, UserGroup arg2)
        {
            return arg1;
        }

        private UserGroup Active(UserGroup arg1, UserGroup arg2)
        {
            return arg1;
        }

        protected override IEnumerable<UserGroupState> DefaultState => new[]
        {
            UserGroupState.Error,
        };

        protected override int GetStateValue(UserGroupState state)
        {
            return (int)state;
        }
    }
}
