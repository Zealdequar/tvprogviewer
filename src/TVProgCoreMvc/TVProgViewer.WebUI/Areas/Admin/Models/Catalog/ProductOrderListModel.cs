using TVProgViewer.WebUI.Areas.Admin.Models.Orders;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product order list model
    /// </summary>
    public partial record ProductOrderListModel : BasePagedListModel<OrderModel>
    {
    }
}