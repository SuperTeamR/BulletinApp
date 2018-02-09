using FessooFramework.Objects.Data;
using System.Collections.Generic;

namespace BulletinEngine.Entity.Data
{
    ///-------------------------------------------------------------------------------------------------
    /// <summary>   Шаблон поля буллетина. Содержит данные для доступа к полю и его заполнению </summary>
    ///
    /// <remarks>   SV Milovanov, 01.02.2018. </remarks>
    ///-------------------------------------------------------------------------------------------------

    public class FieldTemplate : EntityObjectALM<FieldTemplate, FieldTemplateState>
    {
        #region Entity properties
		//-------------------------------------------------------------------------------------------------
        /// <summary>   Наименование поля </summary>
        ///
        /// <value> The name. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Name { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Тэг поля </summary>
        ///
        /// <value> The tag. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Tag { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Атрибут доступа </summary>
        ///
        /// <value> The attribute. </value>
        ///-------------------------------------------------------------------------------------------------

        public string Attribute { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Является ли картинкой </summary>
        ///
        /// <value> True if this object is image, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsImage { get; set; }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Является ли значение динамическим </summary>
        ///
        /// <value> True if this object is dynamic, false if not. </value>
        ///-------------------------------------------------------------------------------------------------

        public bool IsDynamic { get; set; }

        #endregion

        protected override IEnumerable<EntityObjectALMConfiguration<FieldTemplate, FieldTemplateState>> Configurations => new[]
        {
            new EntityObjectALMConfiguration<FieldTemplate, FieldTemplateState>(FieldTemplateState.Created, FieldTemplateState.Handled, Handled)
        };

        private FieldTemplate Handled(FieldTemplate arg1, FieldTemplate arg2)
        {
            return arg1;
        }

        protected override IEnumerable<FieldTemplateState> DefaultState => new[]
        {
            FieldTemplateState.Error
        };

        protected override int GetStateValue(FieldTemplateState state)
        {
            return (int)state;
        }
    }

    public enum FieldTemplateState
    {
        Created = 0,
        Handled = 1,
        Error = 99,
    }
}
