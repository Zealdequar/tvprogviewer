namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel manufacturer mapping
    /// </summary>
    public partial class TvChannelManufacturer : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the manufacturer identifier
        /// </summary>
        public int ManufacturerId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tvChannel is featured
        /// </summary>
        public bool IsFeaturedTvChannel { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
