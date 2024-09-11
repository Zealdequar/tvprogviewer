using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a predefined (default) tvChannel attribute value
    /// </summary>
    public partial class PredefinedTvChannelAttributeValue : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel attribute identifier
        /// </summary>
        public int TvChannelAttributeId { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel attribute name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the price adjustment
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether "price adjustment" is specified as percentage
        /// </summary>
        public bool PriceAdjustmentUsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the weight adjustment
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        /// Gets or sets the attribute value cost
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the value is pre-selected
        /// </summary>
        public bool IsPreSelected { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
