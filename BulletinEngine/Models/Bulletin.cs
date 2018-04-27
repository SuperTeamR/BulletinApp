using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Helpers;
using BulletinHub.Helpers;
using BulletinHub.Tools;
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
        public Guid? GroupId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Images { get; set; }
        public DateTime? DatePublication { get; set; }

        public DateTime? GenerationCheckLast { get; set; }
        public DateTime? GenerationCheckNext { get; set; }
#if DEBUG
        private static int GenerationCheckPeriod = 1;
#else
        private static int GenerationCheckPeriod =  60 * 60 * 24;
#endif

        #endregion
        #region Another methos
        public void SetGenerationCheck()
        {
            GenerationCheckLast = DateTime.Now;
            GenerationCheckNext = GenerationCheckLast.Value.AddSeconds(GenerationCheckPeriod);
            StateEnum = StateEnum;
            BCT.SaveChanges();
        }
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
            arg1.GenerationCheckLast = arg2.GenerationCheckLast;
            arg1.GenerationCheckNext = arg2.GenerationCheckNext;
            arg1.DatePublication = arg2.DatePublication;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Bulletin>> CreatorsService => new[]
        {
             EntityObjectALMCreator<Bulletin>.New(ToPackageCache, ToEntity, new Version(1,0,0,0)),
              EntityObjectALMCreator<Bulletin>.New<BulletinCache>(ToBulletinCache,ToEntity, new Version(1,0,0,0))
        };
        #region BulletinCache
        private Bulletin ToEntity(BulletinCache cache, Bulletin entity)
        {
            entity.Title = cache.Title;
            entity.Description = cache.Description;
            entity.Images = cache.Images;
            entity.Price = cache.Price;
            var group = BCT.Context.BulletinDb.Groups.FirstOrDefault(q => q.Hash == cache.GroupSignature);
            if (group != null)
            {
                entity.GroupId = group.Id;
            }
            return entity;
        }

        private BulletinCache ToBulletinCache(Bulletin entity)
        {
            var cache = new BulletinCache();
            cache.Title = entity.Title;
            cache.Name = entity.Title;
            cache.Description = entity.Description;
            cache.Images = entity.Images;
            cache.Price = entity.Price;
            var group = GroupHelper.GetGroupSignature2(entity.Id);
            if (group != null)
            {
                cache.CurrentGroup = group.ToString();
            }
            return cache;
        }
        #endregion
        #region BulletinPackage
        public static Bulletin ToEntity(BulletinPackage obj, Bulletin entity)
        {
            var result = default(Bulletin);
            BCT.Execute(d =>
            {
                var hash = obj.Signature.GetHash();
                var dbGroup = d.BulletinDb.Groups.FirstOrDefault(q => q.Hash == hash);
                var bulletinTitle = obj.ValueFields["Название объявления"];
                var bulletinDesc = obj.ValueFields["Описание объявления"];
                var bulletinPrice = obj.ValueFields["Цена"];
                var bulletinImages = obj.ValueFields.ContainsKey("Фотографии") ? obj.ValueFields["Фотографии"] : null;

                entity.UserId = d.UserId;
                entity.GroupId = dbGroup.Id;
                entity.Title = bulletinTitle;
                entity.Description = bulletinDesc;
                entity.Price = bulletinPrice;
                entity.Images = bulletinImages;

                result = entity;
            });
            return result;
        }
        public static BulletinPackage ToPackageCache(Bulletin obj)
        {
            var result = default(BulletinPackage);
            BCT.Execute(d =>
            {
                var package = new BulletinPackage();
                var valueFields = new Dictionary<string, string>();
                valueFields.Add("Название объявления", obj.Title);
                valueFields.Add("Описание объявления", obj.Description);
                valueFields.Add("Цена", obj.Price);
                valueFields.Add("Вид объявления ", "Продаю свое");
                if (string.IsNullOrEmpty(obj.Images))
                    valueFields.Add("Фотографии", obj.Images);
                package.ValueFields = valueFields;
                package.Title = obj.Title;

                result = package;
            });
            return result;
        }
        #endregion

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
                    result = c.BulletinDb.Bulletins
                    .Where(q => workStates.Contains(q.State)).Take(1).ToArray();
                }
                else
                {
                    result = c.BulletinDb.Bulletins.Where(q => q.UserId == c.UserId).ToArray();
                }
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

        #region Custom query
        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                //Пока не заморачивался - передаётся базовый объект и требуется привести к типу
                var entities = Enumerable.Empty<Bulletin>();
                if (obj.Any())
                    entities = obj.Select(q => (Bulletin)q).ToArray();
                switch (code)
                {
                    case "Create":
                        result = BulletinHelper.Create(entities);
                        break;
                    case "All":
                        result = BulletinHelper.All();
                        break;
                    case "AddAvito":
                        result = new[] { BulletinHelper.AddAvito(entities.FirstOrDefault()) };
                        break;
                    case "Edit":
                        result = new[] { BulletinHelper.Edit(entities.FirstOrDefault()) };
                        break;
                    case "Remove":
                        BulletinHelper.Remove(entities);
                        break;
                    default:
                        break;
                }
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