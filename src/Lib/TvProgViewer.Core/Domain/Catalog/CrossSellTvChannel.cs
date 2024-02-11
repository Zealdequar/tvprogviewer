namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a cross-sell tvchannel
    /// </summary>
    public partial class CrossSellTvChannel : BaseEntity
    {
        /// <summary>
        /// Gets or sets the first tvchannel identifier
        /// </summary>
        public int TvChannelId1 { get; set; }

        /// <summary>
        /// Gets or sets the second tvchannel identifier
        /// </summary>
        public int TvChannelId2 { get; set; }
    }
}
