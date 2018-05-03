using BulletinBridge.Models;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Models
{
    public class BulletinTemplate : EntityObjectALM<BulletinTemplate, DefaultState>
    {
        #region Property
        public string URL { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        /// <summary>
        /// Количество просмотров
        /// </summary>
        public string Count { get; set; }

        public string Images { get; set; }
        public string GroupSignature { get; set; }
        /// <summary>
        /// Область >> Город >> Место
        /// </summary>
        public string Region { get; set; }
        #endregion
        #region ALM
        protected override IEnumerable<EntityObjectALMConfiguration<BulletinTemplate, DefaultState>> Configurations => Enumerable.Empty<EntityObjectALMConfiguration<BulletinTemplate, DefaultState>>();

        protected override int GetStateValue(DefaultState state)
        {
            return (int)state;
        }

        protected override BulletinTemplate SetValueDefault(BulletinTemplate arg1, BulletinTemplate arg2)
        {
            arg1.URL = arg2.URL;
            arg1.Title = arg2.Title;
            arg1.Description = arg2.Description;
            arg1.Price = arg2.Price;
            arg1.Count = arg2.Count;
            arg1.Images = arg2.Images;
            arg1.GroupSignature = arg2.GroupSignature;
            arg1.Region = arg2.Region;
            return arg1;
        }
        #endregion
        #region Creators
        protected override IEnumerable<EntityObjectALMCreator<BulletinTemplate>> CreatorsService => new[]
        {
            EntityObjectALMCreator<BulletinTemplate>.New<BulletinTemplateCache>(ToCache, ToEntity, new Version(1,0,0,0))
        };
        private BulletinTemplate ToEntity(BulletinTemplateCache cache, BulletinTemplate entity)
        {
            entity.URL = cache.URL;
            entity.Title = cache.Title;
            entity.Description = cache.Description;
            entity.Price = cache.Price;
            entity.Count = cache.Count;
            entity.Images = cache.Images;
            entity.GroupSignature = cache.GroupSignature;
            entity.Region = cache.Region;
            return entity;
        }
        internal static BulletinTemplateCache ToCache(BulletinTemplate entity)
        {
            var cache = new BulletinTemplateCache();
            cache.URL = entity.URL;
            cache.Title = entity.Title;
            cache.Description = entity.Description;
            cache.Price = entity.Price;
            cache.Count = entity.Count;
            cache.Images = entity.Images;
            cache.GroupSignature = entity.GroupSignature;
            cache.Region = entity.Region;
            return cache;
        }
        #endregion
    }
}
