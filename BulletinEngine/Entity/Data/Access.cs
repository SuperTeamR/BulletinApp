using BulletinBridge.Data;
using BulletinEngine.Core;
using BulletinEngine.Helpers;
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
        protected override IEnumerable<EntityObjectALMConfiguration<Access, AccessState>> Configurations => Enumerable.Empty<EntityObjectALMConfiguration<Access, AccessState>>();

        protected override int GetStateValue(AccessState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override Access SetValueDefault(Access arg1, Access arg2)
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
            EntityObjectALMCreator<Access>.New<AccessPackage>(ToCache, ToEntity, new Version(1,0,0,0))
        };
        private Access ToEntity(AccessPackage cache, Access entity)
        {
            entity.BoardId = cache.BoardId;
            entity.Login = cache.Login;
            entity.Password = cache.Password;
            return entity;
        }
        internal static AccessPackage ToCache(Access obj)
        {
            var board = BCT.Context.BulletinDb.Boards.Find(obj.BoardId);
            var result = new AccessPackage();
            if (board != null)
            {
                result.BoardName = board.Name;
            }
            result.BoardId = obj.BoardId;
            result.Login = obj.Login;
            result.Password = obj.Password;
            return result;
        }
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
                    result = c.BulletinDb.Accesses
                    .Where(q => workStates.Contains(q.State)).ToArray();
                else
                {
                    result = c.BulletinDb.Accesses.Where(q => q.UserId == c.UserId).ToArray();
                }
            });
            return result;
        }
        public override IEnumerable<TDataModel> _CacheSave<TDataModel>(IEnumerable<TDataModel> objs)
        {
            var result = Enumerable.Empty<TDataModel>();
            BCT.Execute(d =>
            {
                var access = objs.FirstOrDefault() as Access;
                var dbAccess = d.BulletinDb.Accesses.FirstOrDefault(q => q.Login == access.Login && q.Password == access.Password);
                if (dbAccess == null)
                {
                    access.BoardId = d.BulletinDb.Boards.FirstOrDefault().Id;
                    access.UserId = d.UserId;
                    access.StateEnum = AccessState.Unchecked;
                    d.SaveChanges();
                }
                else
                {
                    dbAccess.Login = access.Login;
                    dbAccess.Password = access.Password;
                    dbAccess.State = (int)AccessState.Unchecked;
                    d.SaveChanges();
                }
                result = new TDataModel[] { access as TDataModel };
            });
            return result;
        }
        #endregion

        #region Custom query
        public override IEnumerable<EntityObject> CustomCollectionLoad(string code, string sessionUID = "", string hashUID = "", IEnumerable<EntityObject> obj = null, IEnumerable<Guid> id = null)
        {
            var result = Enumerable.Empty<EntityObject>();
            BCT.Execute(c =>
            {
                //Пока не заморачивался - передаётся базовый объект и требуется привести к типу
                var entities = Enumerable.Empty<Access>();
                if (obj.Any())
                    entities = obj.Select(q => (Access)q).ToArray();
                switch (code)
                {
                    case "All":
                        result = AccessHelper.All();
                        break;
                    case "AddAvito":
                        result = new[] { AccessHelper.AddAvito(entities.FirstOrDefault()) };
                        break;
                    case "Remove":
                        AccessHelper.Remove(entities);                        
                        break;
                    default:
                        break;
                }
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
        Cloning = 8,
        Error = 99,
    }
}