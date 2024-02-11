namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a record to manage tvchannel inventory per warehouse
    /// </summary>
    public partial class TvChannelWarehouseInventory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the tvchannel identifier
        /// </summary>
        public int TvChannelId { get; set; }

        /// <summary>
        /// Gets or sets the warehouse identifier
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// Gets or sets the stock quantity
        /// </summary>
        public int StockQuantity { get; set; }

        /// <summary>
        /// Gets or sets the reserved quantity (ordered but not shipped yet)
        /// </summary>
        public int ReservedQuantity { get; set; }
    }
}
