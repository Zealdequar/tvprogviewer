namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// TvChannel review approved event
    /// </summary>
    public partial class TvChannelReviewApprovedEvent
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        public TvChannelReviewApprovedEvent(TvChannelReview tvChannelReview)
        {
            TvChannelReview = tvChannelReview;
        }

        /// <summary>
        /// TvChannel review
        /// </summary>
        public TvChannelReview TvChannelReview { get; }
    }
}