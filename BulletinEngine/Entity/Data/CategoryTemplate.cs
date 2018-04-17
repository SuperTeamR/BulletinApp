using FessooFramework.Objects.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

        #region ALM -- Definition
        protected override IEnumerable<EntityObjectALMConfiguration<CategoryTemplate, CategoryTemplateState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<CategoryTemplate, CategoryTemplateState>(CategoryTemplateState.Created, CategoryTemplateState.Handled, SetValueDefault)
        };

        protected override int GetStateValue(CategoryTemplateState state)
        {
            return (int)state;
        }
        #endregion

        #region ALM -- Methods
        protected override CategoryTemplate SetValueDefault(CategoryTemplate arg1, CategoryTemplate arg2)
        {
            arg1.BoardId = arg2.BoardId;
            arg1.ParentId = arg2.ParentId;
            arg1.Name = arg2.Name;

            return arg1;
        }
        #endregion

        #region ALM -- Creators
        protected override IEnumerable<EntityObjectALMCreator<CategoryTemplate>> CreatorsService => Enumerable.Empty<EntityObjectALMCreator<CategoryTemplate>>();
        #endregion
    }

    public enum CategoryTemplateState
    {
        Created = 0,
        Handled = 1,
        Error = 99
    }
}
