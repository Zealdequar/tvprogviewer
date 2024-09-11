namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel review helpfulness
    /// </summary>
    public partial class TvChannelReviewHelpfulness : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel review identifier
        /// </summary>
        public int TvChannelReviewId { get; set; }

        /// <summary>
        /// A value indicating whether a review a helpful
        /// </summary>
        public bool WasHelpful { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }
    }
}
