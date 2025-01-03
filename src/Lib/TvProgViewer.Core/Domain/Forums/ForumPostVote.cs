﻿using System;

namespace TvProgViewer.Core.Domain.Forums
{
    /// <summary>
    /// Represents a forum post vote
    /// </summary>
    public partial class ForumPostVote : BaseEntity
    {
        /// <summary>
        /// Gets or sets the forum post identifier
        /// </summary>
        public int ForumPostId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this vote is up or is down
        /// </summary>
        public bool IsUp { get; set; }

        /// <summary>
        /// Gets or sets the date and time of instance creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
    }
}
