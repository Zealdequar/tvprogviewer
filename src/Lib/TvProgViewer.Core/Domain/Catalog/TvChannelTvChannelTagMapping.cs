namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel-tvChannel tag mapping class
    /// </summary>
    public partial class TvChannelTvChannelTagMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the tvChannel tag identifier
        /// </summary>
        public int TvChannelTagId { get; set; }
    }
}