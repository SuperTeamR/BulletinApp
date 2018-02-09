using Data.Enums;
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Пользователь системы </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class User : EntityObjectALM<User, UserState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Логин для авторизации </summary>
        ///
        /// <value> The login. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Login { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Хэш для авторизации </summary>
        ///
        /// <value> The hash. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Hash { get; set; }
        #endregion
        protected override IEnumerable<EntityObjectALMConfiguration<User, UserState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<User, UserState>(UserState.Created, UserState.Activated, Activated),
            new EntityObjectALMConfiguration<User, UserState>(UserState.Activated, UserState.Payed, Payed),
            new EntityObjectALMConfiguration<User, UserState>(UserState.Payed, UserState.Activated, Activated),
            new EntityObjectALMConfiguration<User, UserState>(UserState.Payed, UserState.Closed, Closed),
            new EntityObjectALMConfiguration<User, UserState>(UserState.Activated, UserState.Closed, Closed),
        };

        private User Closed(User arg1, User arg2)
        {
            return arg1;
        }

        private User Payed(User arg1, User arg2)
        {
            return arg1;
        }

        private User Activated(User arg1, User arg2)
        {
            return arg1;
        }

        protected override IEnumerable<UserState> DefaultState => new[] { UserState.Error };

        protected override int GetStateValue(UserState state)
        {
            return (int)state;
        }
    }
}
