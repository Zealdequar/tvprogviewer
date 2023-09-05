using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment item model
    /// </summary>
    public partial record ShipmentItemModel : BaseTvProgEntityModel
    {
        #region Ctor

        public ShipmentItemModel()
        {
            AvailableWarehouses = new List<WarehouseInfo>();
        }

        #endregion

        #region Properties

        public int OrderItemId { get; set; }

        public int ProductId { get; set; }

        [TvProgResourceDisplayName("Admin.Orders.Shipments.Products.ProductName")]
        public string ProductName { get; set; }

        public string Sku { get; set; }

        public string AttributeInfo { get; set; }

        public string RentalInfo { get; set; }

        public bool ShipSeparately { get; set; }

        //weight of one item (product)
        [TvProgResourceDisplayName("Admin.Orders.Shipments.Products.ItemWeight")]
        public string ItemWeight { get; set; }

        [TvProgResourceDisplayName("Admin.Orders.Shipments.Products.ItemDimensions")]
        public string ItemDimensions { get; set; }

        public int QuantityToAdd { get; set; }

        public int QuantityOrdered { get; set; }

        [TvProgResourceDisplayName("Admin.Orders.Shipments.Products.QtyShipped")]
        public int QuantityInThisShipment { get; set; }

        public int QuantityInAllShipments { get; set; }


        public string ShippedFromWarehouse { get; set; }

        public bool AllowToChooseWarehouse { get; set; }

        //used before a shipment is created
        public List<WarehouseInfo> AvailableWarehouses { get; set; }

        #endregion

        #region Nested Classes

        public partial record WarehouseInfo : BaseTvProgModel
        {
            public int WarehouseId { get; set; }
            public string WarehouseName { get; set; }
            public int StockQuantity { get; set; }
            public int ReservedQuantity { get; set; }
            public int PlannedQuantity { get; set; }
        }

        #endregion
    }
}