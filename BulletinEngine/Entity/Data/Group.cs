using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Группа по категориям. Уникальна по содержанию в пределах одной борды </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Group : EntityObjectALM<Group, GroupState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор борды </summary>
        ///
        /// <value> The identifier of the board. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid BoardId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Хэш группы (генерируется на базе категорий) </summary>
        ///
        /// <value> The hash. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Hash { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<Group, GroupState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Group, GroupState>(GroupState.Created, GroupState.Loaded, Loaded),
            new EntityObjectALMConfiguration<Group, GroupState>(GroupState.Loaded, GroupState.Changed, Changed),
            new EntityObjectALMConfiguration<Group, GroupState>(GroupState.Changed, GroupState.Loaded, Loaded),
        };

        private Group Changed(Group arg1, Group arg2)
        {
            return arg1;
        }

        private Group Loaded(Group arg1, Group arg2)
        {
            return arg1;
        }

        protected override IEnumerable<GroupState> DefaultState => new[]
        {
            GroupState.Error
        };

        protected override int GetStateValue(GroupState state)
        {
            return (int)state;
        }
    }

    public enum GroupState
    {
        Created = 0,
        Changed = 1,
        Loaded = 2,
        Error = 99
    }
}
