namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a method of inventory management
    /// </summary>
    public enum ManageInventoryMethod
    {
        /// <summary>
        /// Don't track inventory for tvchannel
        /// </summary>
        DontManageStock = 0,

        /// <summary>
        /// Track inventory for tvchannel
        /// </summary>
        ManageStock = 1,

        /// <summary>
        /// Track inventory for tvchannel by tvchannel attributes
        /// </summary>
        ManageStockByAttributes = 2,
    }
}
