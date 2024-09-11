namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvChannel
    /// </summary>
    public partial class CrossSellTvChannel : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first tvChannel identifier
        /// </summary>
        public int TvChannelId1 { get; set; }

        /// <summary>
        /// Gets or sets the second tvChannel identifier
        /// </summary>
        public int TvChannelId2 { get; set; }
    }
}
