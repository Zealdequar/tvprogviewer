﻿using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Services.Discounts
{
    /// <summary>
    /// Represents a request of discount requirement validation
    /// </summary>
    public partial class DiscountRequirementValidationRequest
    {
        /// <summary>
        /// Gets or sets the appropriate discount requirement ID (identifier)
        /// </summary>
        public int DiscountRequirementId { get; set; }

        /// <summary>
        /// Gets or sets the user
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Gets or sets the store
        /// </summary>
        public Store Store { get; set; }
    }
}
