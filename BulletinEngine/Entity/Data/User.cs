using FessooFramework.Objects.Data;
using System.Collections.Generic;
using System.Linq;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Пользователь системы </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    //public class User : EntityObjectALM<User, UserState>
    //{
    //    #region Entity properties
    //    ///-------------------------------------------------------------------------------------------------
    //    /// <summary>   Логин для авторизации </summary>
    //    ///
    //    /// <value> The login. </value>
    //    ///-------------------------------------------------------------------------------------------------

    //    public string Login { get; set; }

    //    ///-------------------------------------------------------------------------------------------------
    //    /// <summary>   Хэш для авторизации </summary>
    //    ///
    //    /// <value> The hash. </value>
    //    ///-------------------------------------------------------------------------------------------------

    //    public string Hash { get; set; }
    //    #endregion

    //    #region ALM -- Definition
    //    protected override IEnumerable<EntityObjectALMConfiguration<User, UserState>> Configurations => new[]
    //    {
    //        new EntityObjectALMConfiguration<User, UserState>(UserState.Created, UserState.Activated, SetValueDefault),
    //        new EntityObjectALMConfiguration<User, UserState>(UserState.Activated, UserState.Payed, SetValueDefault),
    //        new EntityObjectALMConfiguration<User, UserState>(UserState.Payed, UserState.Activated, SetValueDefault),
    //        new EntityObjectALMConfiguration<User, UserState>(UserState.Payed, UserState.Closed, SetValueDefault),
    //        new EntityObjectALMConfiguration<User, UserState>(UserState.Activated, UserState.Closed, SetValueDefault),
    //    };

    //    protected override int GetStateValue(UserState state)
    //    {
    //        return (int)state;
    //    }
    //    #endregion

    //    #region ALM -- Methods
    //    protected override User SetValueDefault(User arg1, User arg2)
    //    {
    //        arg1.Login = arg2.Login;
    //        arg1.Hash = arg2.Hash;

    //        return arg1;
    //    }
    //    #endregion

    //    #region ALM -- Creators
    //    protected override IEnumerable<EntityObjectALMCreator<User>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<User>>();
    //    #endregion
    //}

    //public enum UserState
    //{
    //    Created = 0,
    //    Activated = 1,
    //    Payed = 2,
    //    Closed = 3,
    //    Error = 99,
    //}
}
