using BulletinBridge.Data;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Контейнер инстанций буллетинов пользователя </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    class Bulletin : EntityObjectALM<Bulletin, BulletinState>
    {
        #region Entity proeperties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }
        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<Bulletin, BulletinState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.WaitPublication, WaitPublication),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Closed, Closed)
        };

        protected override IEnumerable<BulletinState> DefaultState => new[]
        {
            BulletinState.Error
        };

        protected override int GetStateValue(BulletinState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private Bulletin WaitPublication(Bulletin arg1, Bulletin arg2)
        {
            arg1.UserId = arg2.UserId;

            return arg1;
        }
        private Bulletin Closed(Bulletin arg1, Bulletin arg2)
        {
            arg1.UserId = arg2.UserId;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Bulletin>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<Bulletin>>();
        #endregion
    }

    enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Publication = 3,
        Edited = 4,
        Closed = 5,
        Error = 99,
    }
}
