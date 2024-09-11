using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel attribute value
    /// </summary>
    public partial class TvChannelAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel attribute mapping identifier
        /// </summary>
        public int TvChannelAttributeMappingId { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type identifier
        /// </summary>
        public int AttributeValueTypeId { get; set; }

        /// <summary>
        /// Gets or sets the associated tvChannel identifier (used only with AttributeValueType.AssociatedToTvChannel)
        /// </summary>
        public int AssociatedTvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the color RGB value (used with "Color squares" attribute type)
        /// </summary>
        public string ColorSquaresRgb { get; set; }

        /// <summary>
        /// Gets or sets the picture ID for image square (used with "Image squares" attribute type)
        /// </summary>
        public int ImageSquaresPictureId { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment (used only with AttributeValueType.Simple)
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "price adjustment" is specified as percentage (used only with AttributeValueType.Simple)
        /// </summary>
        public bool PriceAdjustmentUsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment (used only with AttributeValueType.Simple)
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the attribute value cost (used only with AttributeValueType.Simple)
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user can enter the quantity of associated tvChannel (used only with AttributeValueType.AssociatedToTvChannel)
        /// </summary>
        public bool UserEntersQty { get; set; }

        /// <summary>
        /// Gets or sets the quantity of associated tvChannel (used only with AttributeValueType.AssociatedToTvChannel)
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the picture (identifier) associated with this value. This picture should replace a tvChannel main picture once clicked (selected).
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the attribute value type
        /// </summary>
        public AttributeValueType AttributeValueType
        {
            get => (AttributeValueType)AttributeValueTypeId;
            set => AttributeValueTypeId = (int)value;
        }
    }
}
