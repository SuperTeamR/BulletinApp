using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinHub.Entity.Converters;
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

    public class Bulletin : EntityObjectALM<Bulletin, BulletinState>
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
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.WaitPublication, Default),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Closed, Default),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Cloning, Default),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Created, Default)
        };

        protected override IEnumerable<BulletinState> DefaultState => new[]
        {
            BulletinState.Error, BulletinState.Cloning
        };

        protected override int GetStateValue(BulletinState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private Bulletin Default(Bulletin arg1, Bulletin arg2)
        {
            arg1.UserId = arg2.UserId;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Bulletin>> CreatorsService => new[]
        {
             EntityObjectALMCreator<Bulletin>.New(BulletinConverter.Convert, BulletinConverter.Convert, new Version(1,0,0,0))
        };
        #endregion

        #region DataService -- Methods
        public override IEnumerable<EntityObject> _CollectionObjectLoad()
        {
            var workStates = new[]
            {
                (int)BulletinState.Cloning,
            };
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                var id = c._SessionInfo.HashUID;
                var id2 = c._SessionInfo.SessionUID;
                if (id == "Engine")
                {
                    result = c.Db1.Bulletins
                    .Where(q => workStates.Contains(q.State)).ToArray();
                }
                else
                    result = base._CollectionObjectLoad();
            });
            return result;
        }

        public override EntityObject _ObjectLoadById(Guid id)
        {
            return base._ObjectLoadById(id);
        }
        public override IEnumerable<TDataModel> _CacheSave<TDataModel>(IEnumerable<TDataModel> objs)
        {
            var result = Enumerable.Empty<TDataModel>();
            BCT.Execute(d =>
            {
                d.SaveChanges();
                result = objs;
            });
            return result;
        }


        #endregion
    }

    public enum BulletinState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Publication = 3,
        Edited = 4,
        Closed = 5,
        Cloning = 6,
        Error = 99,
    }
}
