using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order list model
    /// </summary>
    public partial record OrderListModel : BasePagedListModel<OrderModel>
    {
    }
}