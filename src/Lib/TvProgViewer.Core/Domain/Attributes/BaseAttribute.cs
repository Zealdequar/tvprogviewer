using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Attributes
{
    /// <summary>
    /// Представляет базовый класс для атрибутов
    /// </summary>
    public abstract partial class BaseAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Получает или устанавливает Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Получает или устанавливает значение определяющее явлется ли атрибут обязательным 
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// Получает или устанавливает идентификатор контроля типа атрибута
        /// </summary>
        public int AttributeControlTypeId { get; set; }

        /// <summary>
        /// Получает или устанавливает отображаемый порядок
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Получает или устанавливает контроль типа атрибута
        /// </summary>
        public AttributeControlType AttributeControlType
        {
            get => (AttributeControlType)AttributeControlTypeId;
            set => AttributeControlTypeId = (int)value;
        }

        /// <summary>
        /// Значение определяющее должен ли иметь значения этот атрибут
        /// </summary>
        public bool ShouldHaveValues
        {
            get
            {
                if (AttributeControlType == AttributeControlType.TextBox ||
                    AttributeControlType == AttributeControlType.MultilineTextbox ||
                    AttributeControlType == AttributeControlType.Datepicker ||
                    AttributeControlType == AttributeControlType.FileUpload)
                    return false;

                // - иначе контроль типов атрибута поддерживает значения
                return true;
            }
        }

        /// <summary>
        /// Значение определяющее может ли этот атрибут использован как условие для какого-то другого атрибута
        /// </summary>
        public bool CanBeUsedAsCondition
        {
            get
            {
                if (AttributeControlType == AttributeControlType.ReadonlyCheckboxes ||
                    AttributeControlType == AttributeControlType.TextBox ||
                    AttributeControlType == AttributeControlType.MultilineTextbox ||
                    AttributeControlType == AttributeControlType.Datepicker ||
                    AttributeControlType == AttributeControlType.FileUpload)
                    return false;

                // - иначе контроль типов атрибута поддерживает это
                return true;
            }
        }
    }
}