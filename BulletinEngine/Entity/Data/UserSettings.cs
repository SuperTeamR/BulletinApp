using BulletinHub.Entity.Converters;
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
            EntityObjectALMCreator<UserSettings>.New(UserSettingsConverter.Convert, UserSettingsConverter.Convert, new Version(1, 0, 0, 0))
        };
        #endregion
    }
    public enum UserSettingsState
    {
        Created = 0,
        Error = 99,
    }
}