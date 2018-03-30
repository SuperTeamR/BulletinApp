using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Converters;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Конкретная инстанция буллетина на борде </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class BulletinInstance : EntityObjectALM<BulletinInstance, BulletinInstanceState>
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

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.WaitPublication, PreparePublicated),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.WaitPublication, BulletinInstanceState.OnModeration, OnModeration),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Publicated, Publicated),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Rejected, Rejected),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Blocked, Blocked),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Publicated, BulletinInstanceState.Edited, Edited),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Rejected, BulletinInstanceState.Edited, Edited),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Edited, BulletinInstanceState.OnModeration, OnModeration),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Blocked, BulletinInstanceState.Removed, Removed),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Publicated, BulletinInstanceState.Created, Closed),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Created, Closed),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Edited, BulletinInstanceState.Created, Closed),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.Created, Closed),
             new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.Unchecked, Closed),
        };
        protected override IEnumerable<BulletinInstanceState> DefaultState => new[] { BulletinInstanceState.Error, BulletinInstanceState.Unchecked, BulletinInstanceState.Checking };

        protected override int GetStateValue(BulletinInstanceState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private BulletinInstance Removed(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;

            return arg1;
        }
        private BulletinInstance Blocked(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;

            return arg1;
        }

        private BulletinInstance OnModeration(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;

            return arg1;
        }
        private BulletinInstance Publicated(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;

            return arg1;
        }

        private BulletinInstance PreparePublicated(BulletinInstance arg1, BulletinInstance arg2)
        {
            BCT.Execute(d =>
            {
                arg1.BulletinId = arg2.BulletinId;
                arg1.BoardId = arg2.BoardId;
                arg1.AccessId = arg2.AccessId;
                arg1.Url = arg2.Url;
                arg1.GroupId = arg2.GroupId;

                //d.Queue.Bulletins.Enqueue(arg2.Id);
            });
            return arg1;
        }

        private BulletinInstance Rejected(BulletinInstance arg1, BulletinInstance arg2)
        {
            BCT.Execute(d =>
            {
                arg1.BulletinId = arg2.BulletinId;
                arg1.BoardId = arg2.BoardId;
                arg1.AccessId = arg2.AccessId;
                arg1.Url = arg2.Url;
                arg1.GroupId = arg2.GroupId;

                //d.Queue.Bulletins.Enqueue(arg2.Id);
            });

            return arg1;
        }

        private BulletinInstance Edited(BulletinInstance arg1, BulletinInstance arg2)
        {
            BCT.Execute(d =>
            {
                arg1.BulletinId = arg2.BulletinId;
                arg1.BoardId = arg2.BoardId;
                arg1.AccessId = arg2.AccessId;
                arg1.Url = arg2.Url;
                arg1.GroupId = arg2.GroupId;

                //d.Queue.Bulletins.Enqueue(arg2.Id);
            });

            return arg1;
        }

        private BulletinInstance Closed(BulletinInstance arg1, BulletinInstance arg2)
        {
            BCT.Execute(d =>
            {
                arg1.BulletinId = arg2.BulletinId;
                arg1.BoardId = arg2.BoardId;
                arg1.AccessId = arg2.AccessId;
                arg1.Url = arg2.Url;
                arg1.GroupId = arg2.GroupId;

                //d.Queue.Bulletins.Enqueue(arg2.Id);
            });

            return arg1;
        }



        #endregion
        
        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<BulletinInstance>> CreatorsService => new[]
        {
             EntityObjectALMCreator<BulletinInstance>.New(BulletinInstanceConverter.Convert, BulletinInstanceConverter.Convert, new Version(1,0,0,0))
        };
        #endregion

        #region DataService -- Methods
        public override IEnumerable<EntityObject> _CollectionObjectLoad()
        {
            var workStates = new[]
            {
                (int)BulletinInstanceState.WaitPublication,
                (int)BulletinInstanceState.Unchecked,
                (int)BulletinInstanceState.OnModeration,
                (int)BulletinInstanceState.WaitRepublication,
            };
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                var id = c._SessionInfo.HashUID;
                var id2 = c._SessionInfo.SessionUID;
                if (id == "Engine")
                    result = c.Db1.BulletinInstances
                    .Where(q => workStates.Contains(q.State)).ToArray();
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

    public enum BulletinInstanceState
    {
        Created = 0,
        WaitPublication = 1,
        OnModeration = 2,
        Rejected = 3,
        Blocked = 4,
        Publicated = 5,
        Edited = 6,
        Removed = 7,
        Closed = 8,
        WaitRepublication = 9,
        Error = 99,
        Unchecked = 100,
        Checking = 101,
    }

}
