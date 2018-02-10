using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Конкретная инстанция буллетина на борде </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal class BulletinInstance : EntityObjectALM<BulletinInstance, BulletinInstanceState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор контейнера буллетинов</summary>
        ///
        /// <value> The identifier of the bulletin. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid BulletinId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор борды </summary>
        ///
        /// <value> The identifier of the board. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid BoardId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор доступа </summary>
        ///
        /// <value> The identifier of the access. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid AccessId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Ссылка на буллетин </summary>
        ///
        /// <value> The URL. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Url { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.PreparedPublicated, PreparePublicated),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.PreparedPublicated, BulletinInstanceState.OnModeration, OnModeration),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Publicated, Publicated),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Rejected, Rejected),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Blocked, Blocked),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Publicated, BulletinInstanceState.Edited, Edited),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Rejected, BulletinInstanceState.Edited, Edited),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Edited, BulletinInstanceState.OnModeration, OnModeration),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Blocked, BulletinInstanceState.Removed, Removed),
        };

        private BulletinInstance Removed(BulletinInstance arg1, BulletinInstance arg2)
        {
            return arg1;
        }

        private BulletinInstance Edited(BulletinInstance arg1, BulletinInstance arg2)
        {
            return arg1;
        }

        private BulletinInstance Blocked(BulletinInstance arg1, BulletinInstance arg2)
        {
            return arg1;
        }

        private BulletinInstance Rejected(BulletinInstance arg1, BulletinInstance arg2)
        {
            return arg1;
        }

        private BulletinInstance Publicated(BulletinInstance arg1, BulletinInstance arg2)
        {
            return arg1;
        }

        private BulletinInstance OnModeration(BulletinInstance arg1, BulletinInstance arg2)
        {
          
            return arg1;
        }

        private BulletinInstance PreparePublicated(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;
            return arg1;
        }

        protected override IEnumerable<BulletinInstanceState> DefaultState => new[] { BulletinInstanceState.Error };

        protected override int GetStateValue(BulletinInstanceState state)
        {
            return (int)state;
        }
    }

    internal enum BulletinInstanceState
    {
        Created = 0,
        PreparedPublicated = 1,
        OnModeration = 2,
        Rejected = 3,
        Blocked = 4,
        Publicated = 5,
        Edited = 6,
        Removed = 7,
        Error = 99,
    }

}
