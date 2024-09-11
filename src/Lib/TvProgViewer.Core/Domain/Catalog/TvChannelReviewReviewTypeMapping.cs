using TvProgViewer.Core.Domain.Localization;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvChannel review and review type mapping
    /// </summary>
    public partial class TvChannelReviewReviewTypeMapping : BaseEntity, ILocalizedEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel review identifier
        /// </summary>
        public int TvChannelReviewId { get; set; }

        /// <summary>
        /// Gets or sets the review type identifier
        /// </summary>
        public int ReviewTypeId { get; set; }

        /// <summary>
        /// Gets or sets the rating
        /// </summary>
        public int Rating { get; set; }
    }
}
