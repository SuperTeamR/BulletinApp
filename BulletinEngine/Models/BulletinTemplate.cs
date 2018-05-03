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
        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }
        public string Category4 { get; set; }
        public string Category5 { get; set; }

        /// <summary>
        /// Область - Калининградская область, Москва, Московская область т.д.
        /// </summary>
        public string Region1 { get; set; }
        /// <summary>
        /// Город - Москва, Санкт-петербург, Подольск
        /// </summary>
        public string Region2 { get; set; }
        /// <summary>
        /// Район или метро 
        /// </summary>
        public string Region3 { get; set; }
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
            arg1.Category1 = arg2.Category1;
            arg1.Category2 = arg2.Category2;
            arg1.Category3 = arg2.Category3;
            arg1.Category4 = arg2.Category4;
            arg1.Category5 = arg2.Category5;
            arg1.Region1 = arg2.Region1;
            arg1.Region2 = arg2.Region2;
            arg1.Region3 = arg2.Region3;
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
            entity.Category1 = cache.Category1;
            entity.Category2 = cache.Category2;
            entity.Category3 = cache.Category3;
            entity.Category4 = cache.Category4;
            entity.Category5 = cache.Category5;
            entity.Region1 = cache.Region1;
            entity.Region2 = cache.Region2;
            entity.Region3 = cache.Region3;
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
            cache.Category1 = entity.Category1;
            cache.Category2 = entity.Category2;
            cache.Category3 = entity.Category3;
            cache.Category4 = entity.Category4;
            cache.Category5 = entity.Category5;
            cache.Region1 = entity.Region1;
            cache.Region2 = entity.Region2;
            cache.Region3 = entity.Region3;
            return cache;
        }
        #endregion
    }
}
