using BulletinBridge.Data;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Контейнер инстанций буллетинов пользователя </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    internal class Bulletin : EntityObjectALM<Bulletin, BulletinState>
    {
        #region Entity proeperties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<Bulletin, BulletinState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.WaitPublication, WaitPublication),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Closed, Closed)
        };

        private Bulletin WaitPublication(Bulletin arg1, Bulletin arg2)
        {
            BCT.Execute(d =>
            {
                d.Queue.Bulletins.Enqueue(new BulletinPackage
                {
                    Id = arg2.Id,
                    State = (int)arg2.StateEnum,
                });
            });
            return arg1;
        }

        private Bulletin Closed(Bulletin arg1, Bulletin arg2)
        {
            return arg1;
        }

        protected override IEnumerable<BulletinState> DefaultState => new[]
        {
            BulletinState.Error
        };

        protected override int GetStateValue(BulletinState state)
        {
            return (int)state;
        }
    }

    internal enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Publication = 3,
        Closed = 4,
        Error = 99,
    }
}
