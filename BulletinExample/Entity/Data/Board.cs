using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Борда буллетинов. </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class Board : EntityObject
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Наименование борды. </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Name { get; set; }

    }
}
