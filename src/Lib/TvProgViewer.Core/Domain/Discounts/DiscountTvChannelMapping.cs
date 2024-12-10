namespace TvProgViewer.Core.Domain.Discounts
{
    /// <summary>
    /// Represents a discount-tvchannel mapping class
    /// </summary>
    public partial class DiscountTvChannelMapping : DiscountMapping
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public override int EntityId { get; set; }
    }
}