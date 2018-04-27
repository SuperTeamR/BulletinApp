using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Поле буллетина </summary>
    ///
    /// <remarks>   SV Milovanov, 05.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class BulletinField : EntityObjectALM<BulletinField, BulletinFieldState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор инстанции буллетина </summary>
        ///
        /// <value> The identifier of the bulletin instance. </value>
        ///-------------------------------------------------------------------------------------------------
        public Guid BulletinInstanceId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор поля </summary>
        ///
        /// <value> The identifier of the field. </value>
        ///-------------------------------------------------------------------------------------------------
        public Guid FieldId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Значение поля </summary>
        ///
        /// <value> The value. </value>
        ///-------------------------------------------------------------------------------------------------
        public string Value { get; set; }
        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<BulletinField, BulletinFieldState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Created, BulletinFieldState.Filled, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Filled, BulletinFieldState.Error, SetValueDefault),
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Filled, BulletinFieldState.Edited, SetValueDefault),
        };
        protected override int GetStateValue(BulletinFieldState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override BulletinField SetValueDefault(BulletinField arg1, BulletinField arg2)
        {
            arg1.BulletinInstanceId = arg2.BulletinInstanceId;
            arg1.FieldId = arg2.FieldId;
            arg1.Value = arg2.Value;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<BulletinField>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<BulletinField>>();
        #endregion
    }
    public enum BulletinFieldState
    {
        Created = 0,
        Filled = 1,
        Edited = 2,
        Error = 99
    }
}