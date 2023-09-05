using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment item list model
    /// </summary>
    public partial record ShipmentItemListModel : BasePagedListModel<ShipmentItemModel>
    {
    }
}