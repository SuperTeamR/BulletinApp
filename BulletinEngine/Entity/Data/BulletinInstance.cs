using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Entity.Converters;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.DCT;
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
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.WaitPublication, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.WaitPublication, BulletinInstanceState.OnModeration, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Publicated, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Rejected, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Blocked, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Publicated, BulletinInstanceState.Edited, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Rejected, BulletinInstanceState.Edited, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Edited, BulletinInstanceState.OnModeration, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Blocked, BulletinInstanceState.Removed, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Publicated, BulletinInstanceState.Created, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.OnModeration, BulletinInstanceState.Created, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Edited, BulletinInstanceState.Created, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.Created, SetValueDefault),
             new EntityObjectALMConfiguration<BulletinInstance, BulletinInstanceState>(BulletinInstanceState.Created, BulletinInstanceState.Unchecked, SetValueDefault),
        };

        protected override int GetStateValue(BulletinInstanceState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override BulletinInstance SetValueDefault(BulletinInstance arg1, BulletinInstance arg2)
        {
            arg1.BulletinId = arg2.BulletinId;
            arg1.BoardId = arg2.BoardId;
            arg1.AccessId = arg2.AccessId;
            arg1.Url = arg2.Url;
            arg1.GroupId = arg2.GroupId;

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
        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            DCT.Execute(d =>
            {
                var bulletinModels = Enumerable.Empty<BulletinInstance>();
                if (obj != null && obj.Any())
                {
                    bulletinModels = obj.Cast<BulletinInstance>().ToArray();

                    switch (code)
                    {
                        case "Save":
                            result = obj;
                            d.SaveChanges();
                            break;
                    }
                }

            });
            return result;
        }

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
                    result = c.BulletinDb.BulletinInstances
                    .Where(q => workStates.Contains(q.State)).Take(1).ToArray();
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
