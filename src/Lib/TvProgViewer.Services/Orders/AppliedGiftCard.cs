﻿using TvProgViewer.Core.Domain.Orders;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Applied gift card
    /// </summary>
    public partial class AppliedGiftCard
    {
        /// <summary>
        /// Gets or sets the used value
        /// </summary>
        public decimal AmountCanBeUsed { get; set; }

        /// <summary>
        /// Gets the gift card
        /// </summary>
        public GiftCard GiftCard { get; set; }
    }
}
