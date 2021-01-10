using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order shipment list model
    /// </summary>
    public partial record OrderShipmentListModel : BasePagedListModel<ShipmentModel>
    {
    }
}