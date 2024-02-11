using System;

namespace TvProgViewer.Core.Domain.Orders
{
    /// <summary>
    /// Represents an order item
    /// </summary>
    public partial class OrderItem : BaseEntity
    {
        /// <summary>
        /// Gets or sets the order item identifier
        /// </summary>
        public Guid OrderItemGuid { get; set; }

        /// <summary>
        /// Gets or sets the order identifier
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Gets or sets the tvchannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the quantity
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (include tax)
        /// </summary>
        public decimal UnitPriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the unit price in primary store currency (exclude tax)
        /// </summary>
        public decimal UnitPriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (include tax)
        /// </summary>
        public decimal PriceInclTax { get; set; }

        /// <summary>
        /// Gets or sets the price in primary store currency (exclude tax)
        /// </summary>
        public decimal PriceExclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (include tax)
        /// </summary>
        public decimal DiscountAmountInclTax { get; set; }

        /// <summary>
        /// Gets or sets the discount amount (exclude tax)
        /// </summary>
        public decimal DiscountAmountExclTax { get; set; }

        /// <summary>
        /// Gets or sets the original cost of this order item (when an order was placed), qty 1
        /// </summary>
        public decimal OriginalTvChannelCost { get; set; }

        /// <summary>
        /// Gets or sets the attribute description
        /// </summary>
        public string AttributeDescription { get; set; }

        /// <summary>
        /// Gets or sets the tvchannel attributes in XML format
        /// </summary>
        public string AttributesXml { get; set; }
        
        /// <summary>
        /// Gets or sets the download count
        /// </summary>
        public int DownloadCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether download is activated
        /// </summary>
        public bool IsDownloadActivated { get; set; }

        /// <summary>
        /// Gets or sets a license download identifier (in case this is a downloadable tvchannel)
        /// </summary>
        public int? LicenseDownloadId { get; set; }

        /// <summary>
        /// Gets or sets the total weight of one item
        /// It's nullable for compatibility with the previous version of tvProgViewer where was no such property
        /// </summary>
        public decimal? ItemWeight { get; set; }

        /// <summary>
        /// Gets or sets the rental tvchannel start date (null if it's not a rental tvchannel)
        /// </summary>
        public DateTime? RentalStartDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the rental tvchannel end date (null if it's not a rental tvchannel)
        /// </summary>
        public DateTime? RentalEndDateUtc { get; set; }
    }
}
