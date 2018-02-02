using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Поле, используемое группой </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class GroupedField : EntityObject
    {
        ///-------------------------------------------------------------------------------------------------
        /// <summary>  Идентификатор шаблона поля  </summary>
        ///
        /// <value> The identifier of the guild. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid FieldId { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор группы </summary>
        ///
        /// <value> The identifier of the group. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid GroupId { get; set; }
    }
}
