using BulletinBridge.Data;
using BulletinEngine.Core;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinHub.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Настройки Users </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------
    public class UserSettings : EntityObjectALM<UserSettings, UserSettingsState>
    {
        #region Entity properties
        public Guid UserId { get; set; }
        public int TaskGenerationPeriod { get; set; }
        public DateTime? LastTimeGeneration { get; set; }
        public DateTime? NextTaskGeneration { get; set; }
        #endregion

        #region ALM -- Definition
        protected override int GetStateValue(UserSettingsState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override UserSettings SetValueDefault(UserSettings oldObj, UserSettings newObj)
        {
            oldObj.TaskGenerationPeriod = newObj.TaskGenerationPeriod;
            oldObj.LastTimeGeneration = newObj.LastTimeGeneration;
            oldObj.NextTaskGeneration = newObj.NextTaskGeneration;
            oldObj.UserId = newObj.UserId;

            return oldObj;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<UserSettings>> CreatorsService => new[]
        {
            EntityObjectALMCreator<UserSettings>.New(ToCache, ToEntity, new Version(1, 0, 0, 0))
        };
        public static UserSettingsCache ToCache(UserSettings obj)
        {
            UserSettingsCache result = null;
            BCT.Execute(d =>
            {
            });
            return result;
        }
        public static UserSettings ToEntity(UserSettingsCache obj, UserSettings entity)
        {
            var result = default(Data.UserSettings);
            BCT.Execute(d =>
            {
                result.TaskGenerationPeriod = 7 * 24;
                entity = result;
            });
            return entity;
        }
        #endregion
    }
    public enum UserSettingsState
    {
        Created = 0,
        Error = 99,
    }
}