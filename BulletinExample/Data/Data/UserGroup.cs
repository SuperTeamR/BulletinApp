using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Достуная пользователю группа </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class UserGroup : EntityObject
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор пользователя </summary>
        ///
        /// <value> The identifier of the user. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid UserId { get; set; }
    }
}
