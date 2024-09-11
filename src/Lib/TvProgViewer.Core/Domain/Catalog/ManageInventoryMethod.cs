namespace TvProgViewer.Core.Domain.Catalog
{
    /// <summary>
    /// Represents a method of inventory management
    /// </summary>
    public enum ManageInventoryMethod
    {
        /// <summary>
        /// Don't track inventory for tvChannel
        /// </summary>
        DontManageStock = 0,

        /// <summary>
        /// Track inventory for tvChannel
        /// </summary>
        ManageStock = 1,

        /// <summary>
        /// Track inventory for tvChannel by tvChannel attributes
        /// </summary>
        ManageStockByAttributes = 2,
    }
}
