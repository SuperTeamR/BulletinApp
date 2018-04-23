using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
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

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<UserGroup, UserGroupState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Created, UserGroupState.Active, SetValueDefault),
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Active, UserGroupState.Limited, SetValueDefault),
            new EntityObjectALMConfiguration<UserGroup, UserGroupState>(UserGroupState.Limited, UserGroupState.Active, SetValueDefault),
        };
        protected override int GetStateValue(UserGroupState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override UserGroup SetValueDefault(UserGroup arg1, UserGroup arg2)
        {
            arg1.GroupId = arg2.GroupId;
            arg1.UserId = arg2.UserId;

            return arg1;
        }
        #endregion
        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<UserGroup>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<UserGroup>>();
        #endregion
    }
    public enum UserGroupState
    {
        Created = 0,
        Active = 1,
        Limited = 2,
        Error = 3,
    }
}