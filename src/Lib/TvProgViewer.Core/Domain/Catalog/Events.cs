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
        /// <param name="tvchannelReview">TvChannel review</param>
        public TvChannelReviewApprovedEvent(TvChannelReview tvchannelReview)
        {
            TvChannelReview = tvchannelReview;
        }

        /// <summary>
        /// TvChannel review
        /// </summary>
        public TvChannelReview TvChannelReview { get; }
    }
}