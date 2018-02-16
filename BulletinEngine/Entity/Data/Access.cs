using BulletinBridge.Data;
using BulletinHub.Entity.Converters;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Доступ к Board. </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Access : EntityObjectALM<Access, AccessState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор Board. </summary>
        ///
        /// <value> The identifier of the board. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid BoardId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя. </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Логин для доступа. </summary>
        ///
        /// <value> The login. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Login { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Пароль для доступа. </summary>
        ///
        /// <value> The password. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Password { get; set; }

        #endregion

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<Access, AccessState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Access, AccessState>(AccessState.Created, AccessState.Activated, Activated),
            new EntityObjectALMConfiguration<Access, AccessState>(AccessState.Activated, AccessState.Blocked, Blocked),
            new EntityObjectALMConfiguration<Access, AccessState>(AccessState.Activated, AccessState.Banned, Banned),
            new EntityObjectALMConfiguration<Access, AccessState>(AccessState.Activated, AccessState.DemandPay, DemandPay),
            new EntityObjectALMConfiguration<Access, AccessState>(AccessState.Banned, AccessState.Activated, Activated),
        };
        protected override IEnumerable<AccessState> DefaultState => new[] { AccessState.Error };

        protected override int GetStateValue(AccessState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        private Access DemandPay(Access arg1, Access arg2)
        {
            arg1.Id = arg2.Id;
            arg1.BoardId = arg2.BoardId;
            arg1.UserId = arg2.UserId;
            arg1.Login = arg2.Login;
            arg1.Password = arg2.Password;

            return arg1;
        }

        private Access Banned(Access arg1, Access arg2)
        {
            arg1.Id = arg2.Id;
            arg1.BoardId = arg2.BoardId;
            arg1.UserId = arg2.UserId;
            arg1.Login = arg2.Login;
            arg1.Password = arg2.Password;

            return arg1;
        }

        private Access Blocked(Access arg1, Access arg2)
        {
            arg1.Id = arg2.Id;
            arg1.BoardId = arg2.BoardId;
            arg1.UserId = arg2.UserId;
            arg1.Login = arg2.Login;
            arg1.Password = arg2.Password;

            return arg1;
        }

        private Access Activated(Access arg1, Access arg2)
        {
            arg1.Id = arg2.Id;
            arg1.BoardId = arg2.BoardId;
            arg1.UserId = arg2.UserId;
            arg1.Login = arg2.Login;
            arg1.Password = arg2.Password;

            return arg1;
        }

        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<Access>> CreatorsService => new[]
        {
            EntityObjectALMCreator<Access>.New(AccessConverter.Convert, new Version(1,0,0,0))
        };
        #endregion

    }

    public enum AccessState
    {
        Created = 0,
        Activated = 1,
        Blocked = 2,
        Banned = 3,
        DemandPay = 4,
        Closed = 5,
        Error = 99,
    }
}
