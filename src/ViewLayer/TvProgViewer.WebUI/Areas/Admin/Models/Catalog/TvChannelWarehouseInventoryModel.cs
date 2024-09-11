using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a tvChannel warehouse inventory model
    /// </summary>
    public partial record TvChannelWarehouseInventoryModel : BaseTvProgModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.Warehouse")]
        public int WarehouseId { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.Warehouse")]
        public string WarehouseName { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.WarehouseUsed")]
        public bool WarehouseUsed { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.StockQuantity")]
        public int StockQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.ReservedQuantity")]
        public int ReservedQuantity { get; set; }

        [TvProgResourceDisplayName("Admin.Catalog.TvChannels.TvChannelWarehouseInventory.Fields.PlannedQuantity")]
        public int PlannedQuantity { get; set; }

        #endregion
    }
}