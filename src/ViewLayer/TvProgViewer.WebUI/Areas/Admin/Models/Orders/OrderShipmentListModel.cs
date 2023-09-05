using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order shipment list model
    /// </summary>
    public partial record OrderShipmentListModel : BasePagedListModel<ShipmentModel>
    {
    }
}