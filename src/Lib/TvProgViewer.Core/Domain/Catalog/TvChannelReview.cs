using System;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tvchannel review
    /// </summary>
    public partial class TvChannelReview : BaseEntity
    {
        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the tvchannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the content is approved
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the review text
        /// </summary>
        public string ReviewText { get; set; }

        /// <summary>
        /// Gets or sets the reply text
        /// </summary>
        public string ReplyText { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the user is already notified of the reply to review
        /// </summary>
        public bool UserNotifiedOfReply { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Review helpful votes total
        /// </summary>
        public int HelpfulYesTotal { get; set; }

        /// <summary>
        /// Review not helpful votes total
        /// </summary>
        public int HelpfulNoTotal { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
