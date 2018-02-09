using BulletinEngine.Entity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinEngine.Core
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   A global objects. </summary>
    ///
    /// <remarks>   SV Milovanov, 02.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GlobalObjects
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Текущий пользователь </summary>
        ///
        /// <value> The current user. </value>
        ///-------------------------------------------------------------------------------------------------

        public User CurrentUser { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Текущий доступ к борде </summary>
        ///
        /// <value> The current access. </value>
        ///-------------------------------------------------------------------------------------------------

        public Access CurrentAccess { get; set; }
    }
}
