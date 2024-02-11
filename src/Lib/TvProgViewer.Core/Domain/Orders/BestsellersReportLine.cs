using System;

namespace TvProgViewer.Core.Domain.Orders
{
    /// <summary>
    /// Represents a best sellers report line
    /// </summary>
    [Serializable]
    public partial class BestsellersReportLine
    {
        /// <summary>
        /// Gets or sets the tvchannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the tvchannel name
        /// </summary>
        public string TvChannelName { get; set; }

        /// <summary>
        /// Gets or sets the total amount
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Gets or sets the total quantity
        /// </summary>
        public int TotalQuantity { get; set; }
    }
}
