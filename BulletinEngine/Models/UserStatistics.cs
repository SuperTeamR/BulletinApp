using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinHub.Models
{
    public class UserStatistics : EntityObjectALM<UserStatistics, DefaultState>
    {
        #region Entity properties
        public Guid UserId { get; set; }
        /// <summary>
        /// Общее количество просмотров
        /// </summary>
        public int TotalViews { get; set; }
        /// <summary>
        /// Общее количество звонков
        /// </summary>
        public int TotalCalls { get; set; }
        /// <summary>
        /// Общее количество сообщений
        /// </summary>
        public int TotalMessages { get; set; }

        #endregion

        #region ALM -- Definition
        protected override int GetStateValue(DefaultState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override UserStatistics SetValueDefault(UserStatistics arg1, UserStatistics arg2)
        {
            arg1.Id = arg2.Id;
            arg1.UserId = arg2.UserId;
            arg1.TotalViews = arg2.TotalViews;
            arg1.TotalCalls = arg2.TotalCalls;
            arg1.TotalMessages = arg2.TotalMessages;
            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<UserStatistics>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<UserStatistics>>();


        #endregion
    }
}
