﻿using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulletinExample.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Приложение, подключенное к системе </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Application : EntityObject
    {
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
    }
}