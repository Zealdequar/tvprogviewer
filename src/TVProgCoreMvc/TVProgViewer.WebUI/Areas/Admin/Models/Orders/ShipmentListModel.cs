using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment list model
    /// </summary>
    public partial record ShipmentListModel : BasePagedListModel<ShipmentModel>
    {
    }
}