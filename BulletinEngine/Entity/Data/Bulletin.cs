using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinHub.Entity.Converters;
using BulletinHub.Tools;
using FessooFramework.Objects.Data;
using FessooFramework.Tools.DCT;
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
        public Guid? GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Images { get; set; }

        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<Bulletin, BulletinState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.WaitPublication, SetValueDefault),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Closed, SetValueDefault),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Cloning, SetValueDefault),
            new EntityObjectALMConfiguration<Bulletin, BulletinState>(BulletinState.Created, BulletinState.Created, SetValueDefault)
        };

        protected override int GetStateValue(BulletinState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override Bulletin SetValueDefault(Bulletin arg1, Bulletin arg2)
        {
            arg1.UserId = arg2.UserId;
            arg1.Title = arg2.Title;
            arg1.Description = arg2.Description;
            arg1.Price = arg2.Price;
            arg1.Images = arg2.Images;
            arg1.GroupId = arg2.GroupId;
     

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Bulletin>> CreatorsService => new[]
        {
             EntityObjectALMCreator<Bulletin>.New(BulletinContainerConverter.Convert, BulletinContainerConverter.Convert, new Version(1,0,0,0))
        };
        #endregion

        #region DataService -- Methods

        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            TaskGenerator.Initialize();


            var result = Enumerable.Empty<EntityObject>();
            DCT.Execute(d =>
            {
                var bulletinModels = Enumerable.Empty<Bulletin>();
                if(obj != null && obj.Any())
                {
                    bulletinModels = obj.Cast<Bulletin>().ToArray();

                    switch(code)
                    {
                        case "Create":
                            result = BulletinGenerator.Create(bulletinModels);
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
                    .Where(q => workStates.Contains(q.State)).Take(1).ToArray();
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
