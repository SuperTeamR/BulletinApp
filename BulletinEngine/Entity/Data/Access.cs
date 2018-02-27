using BulletinBridge.Data;
using BulletinEngine.Core;
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
        protected override IEnumerable<AccessState> DefaultState => new[] { AccessState.Error, AccessState.Unchecked, AccessState.Checking };

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
            EntityObjectALMCreator<Access>.New(AccessConverter.ConvertToCache, AccessConverter.ConvertToModel, new Version(1,0,0,0))
        };
        #endregion

        #region DataService -- Methods
        public override IEnumerable<EntityObject> _CollectionObjectLoad()
        {
            var workStates = new[]
            {
                (int)AccessState.Unchecked,
            };
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                var id = c._SessionInfo.HashUID;
                var id2 = c._SessionInfo.SessionUID;
                if (id == "Engine")
                    result = c.Db1.Accesses
                    .Where(q => workStates.Contains(q.State)).ToArray();
                else
                    result = base._CollectionObjectLoad();
            });
            return result;
        }
        public override IEnumerable<TDataModel> _CacheSave<TDataModel>(IEnumerable<TDataModel> objs)
        {
            var result = Enumerable.Empty<TDataModel>();
            BCT.Execute(d =>
            {
                var access = objs.FirstOrDefault() as Access;
                var dbAccess = d.Db1.Accesses.FirstOrDefault(q => q.Login == access.Login && q.Password == access.Password);
                if (dbAccess == null)
                {
                    access.BoardId = d.Db1.Boards.FirstOrDefault().Id;
                    access.UserId = d.Db1.Users.FirstOrDefault().Id;
                    access.StateEnum = AccessState.Activated;
                    d.SaveChanges();
                }
                result = new TDataModel[] { access as TDataModel };
            });
              
            return result;
        }

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
        Unchecked = 6,
        Checking = 7,
        Error = 99,
    }
}
