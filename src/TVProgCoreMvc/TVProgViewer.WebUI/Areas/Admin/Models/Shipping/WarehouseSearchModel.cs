using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a warehouse search model
    /// </summary>
    public partial record WarehouseSearchModel : BaseSearchModel
    {
        [TvProgResourceDisplayName("Admin.Orders.Shipments.List.Warehouse.SearchName")]
        public string SearchName { get; set; }
    }
}