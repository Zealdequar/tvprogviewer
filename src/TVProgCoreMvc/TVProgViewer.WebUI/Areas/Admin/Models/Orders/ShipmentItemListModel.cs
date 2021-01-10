using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a shipment item list model
    /// </summary>
    public partial record ShipmentItemListModel : BasePagedListModel<ShipmentItemModel>
    {
    }
}