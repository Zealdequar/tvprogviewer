using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel specification attribute
    /// </summary>
    public partial class TvChannelSpecificationAttribute : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the attribute type ID
        /// </summary>
        public int AttributeTypeId { get; set; }

        /// <summary>
        /// Gets or sets the specification attribute identifier
        /// </summary>
        public int SpecificationAttributeOptionId { get; set; }

        /// <summary>
        /// Gets or sets the custom value
        /// </summary>
        public string CustomValue { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute can be filtered by
        /// </summary>
        public bool AllowFiltering { get; set; }

        /// <summary>
        /// Gets or sets whether the attribute will be shown on the tvChannel page
        /// </summary>
        public bool ShowOnTvChannelPage { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets the attribute control type
        /// </summary>
        public SpecificationAttributeType AttributeType
        {
            get => (SpecificationAttributeType)AttributeTypeId;
            set => AttributeTypeId = (int)value;
        }
    }
}
