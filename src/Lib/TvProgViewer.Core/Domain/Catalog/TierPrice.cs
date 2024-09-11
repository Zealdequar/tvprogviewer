using System;

namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a tier price
    /// </summary>
    public partial class TierPrice : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvChannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the store identifier (0 - all stores)
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// Gets or sets the user role identifier
        /// </summary>
        public int? UserRoleId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the start date and time in UTC
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the end date and time in UTC
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }
    }
}
