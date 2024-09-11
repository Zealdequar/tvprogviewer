namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a related tvChannel
    /// </summary>
    public partial class RelatedTvChannel : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first tvChannel identifier
        /// </summary>
        public int TvChannelId1 { get; set; }

        /// <summary>
        /// Gets or sets the second tvChannel identifier
        /// </summary>
        public int TvChannelId2 { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
