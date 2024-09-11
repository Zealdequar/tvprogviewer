namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel video mapping
    /// </summary>
    public partial class TvChannelVideo : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the video identifier
        /// </summary>
        public int VideoId { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }
    }
}
