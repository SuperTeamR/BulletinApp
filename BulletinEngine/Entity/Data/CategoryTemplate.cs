using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Шаблон категории буллетина </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class CategoryTemplate : EntityObjectALM<CategoryTemplate, CategoryTemplateState>
    {
        #region Entity properties
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор борды </summary>
        ///
        /// <value> The identifier of the board. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid BoardId { get; set; }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Идентификатор родителя категории </summary>
        ///
        /// <value> The identifier of the parent. </value>
        ///-------------------------------------------------------------------------------------------------

        public Guid ParentId { get; set; }
        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Название категории </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Name { get; set; }
        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<CategoryTemplate, CategoryTemplateState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<CategoryTemplate, CategoryTemplateState>(CategoryTemplateState.Created, CategoryTemplateState.Handled, Handled)
        };

        private CategoryTemplate Handled(CategoryTemplate arg1, CategoryTemplate arg2)
        {
            return arg1;
        }

        protected override IEnumerable<CategoryTemplateState> DefaultState => new[]
        {
            CategoryTemplateState.Error
        };

        protected override int GetStateValue(CategoryTemplateState state)
        {
            return (int)state;
        }
    }

    public enum CategoryTemplateState
    {
        Created = 0,
        Handled = 1,
        Error = 99
    }
}
