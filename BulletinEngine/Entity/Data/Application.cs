
using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Приложение, подключенное к системе </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Application : EntityObjectALM<Application, ApplicationState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Токен приложения </summary>
        ///
        /// <value> The token. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Token { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<Application, ApplicationState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<Application, ApplicationState>(ApplicationState.Created, ApplicationState.Closed, Closed)
        };

        private Application Closed(Application arg1, Application arg2)
        {
            return arg1;
        }

        protected override IEnumerable<ApplicationState> DefaultState => new[]
        {
            ApplicationState.Error
        };

        protected override int GetStateValue(ApplicationState state)
        {
            return (int)state;
        }
    }
    public enum ApplicationState
    {
        Created = 0,
        Closed = 1,
        Error = 2,
    }
}
