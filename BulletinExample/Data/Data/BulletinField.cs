using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Entity.Data
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
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Created, BulletinFieldState.Filled, Filled),
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Filled, BulletinFieldState.Error, Error),
            new EntityObjectALMConfiguration<BulletinField, BulletinFieldState>(BulletinFieldState.Filled, BulletinFieldState.Edited, Edited),
        };
        protected override IEnumerable<BulletinFieldState> DefaultState => new[]
        {
            BulletinFieldState.Error
        };

        protected override IEnumerable<EntityObjectALMCreator<BulletinField>> CreatorsService => throw new NotImplementedException();

        protected override int GetStateValue(BulletinFieldState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private BulletinField Edited(BulletinField arg1, BulletinField arg2)
        {
            arg1.BulletinInstanceId = arg2.BulletinInstanceId;
            arg1.FieldId = arg2.FieldId;
            arg1.Value = arg2.Value;

            return arg1;
        }

        private BulletinField Error(BulletinField arg1, BulletinField arg2)
        {
            arg1.BulletinInstanceId = arg2.BulletinInstanceId;
            arg1.FieldId = arg2.FieldId;
            arg1.Value = arg2.Value;

            return arg1;
        }

        private BulletinField Filled(BulletinField arg1, BulletinField arg2)
        {
            arg1.BulletinInstanceId = arg2.BulletinInstanceId;
            arg1.FieldId = arg2.FieldId;
            arg1.Value = arg2.Value;

            return arg1;
        }
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
