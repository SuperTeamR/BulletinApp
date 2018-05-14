using BulletinBridge.Models;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Models
{
    public class Call : EntityObjectALM<Call, DefaultState>
    {
        #region Properties
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Виртуальный номер, с которого установлена переадресация
        /// </summary>
        public string VirtualNumber { get; set; }
        /// <summary>
        /// Номер, на который идет переадресация
        /// </summary>
        public string ForwardNumber { get; set; }
        /// <summary>
        /// Дата звонка
        /// </summary>
        public DateTime CallDate { get; set; }
        /// <summary>
        /// Текущий статус переадресации:
        ///1 - Работает
        ///2 - Заблокирована
        ///3 - Завершена
        ///4 - Замена номера
        ///5 - Отправлена на блокировку
        ///6 - Ожидает завершения
        /// </summary>
        public string Status { get; set; }
        #endregion
        #region ALM
        protected override IEnumerable<EntityObjectALMConfiguration<Call, DefaultState>> Configurations => Enumerable.Empty<EntityObjectALMConfiguration<Call, DefaultState>>();

        protected override int GetStateValue(DefaultState state)
        {
            return (int)state;
        }

        protected override Call SetValueDefault(Call arg1, Call arg2)
        {
            arg1.Id = arg2.Id;
            arg1.UserId = arg2.UserId;
            arg1.VirtualNumber = arg2.VirtualNumber;
            arg1.ForwardNumber = arg2.ForwardNumber;
            arg1.CallDate = arg2.CallDate;
            return arg1;
        }
        #endregion

        #region Creators
        protected override IEnumerable<EntityObjectALMCreator<Call>> CreatorsService => new[]
        {
            EntityObjectALMCreator<Call>.New<CallCache>(ToCache, ToEntity, new Version(1,0,0,0)),
        };
        private Call ToEntity(CallCache cache, Call entity)
        {
            entity.UserId = cache.UserId;
            entity.VirtualNumber = cache.VirtualNumber;
            entity.ForwardNumber = cache.ForwardNumber;
            entity.CallDate = cache.CallDate;
            return entity;
        }
        internal static CallCache ToCache(Call obj)
        {
            var result = new CallCache();
            result.UserId = obj.UserId;
            result.VirtualNumber = obj.VirtualNumber;
            result.ForwardNumber = obj.ForwardNumber;
            result.CallDate = obj.CallDate;
            return result;
        }
        #endregion

        #region DataService -- Methods

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
       
        #endregion
    }
}
