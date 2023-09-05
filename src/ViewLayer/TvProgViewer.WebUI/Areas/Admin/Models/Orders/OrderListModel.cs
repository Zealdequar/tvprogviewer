using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an order list model
    /// </summary>
    public partial record OrderListModel : BasePagedListModel<OrderModel>
    {
    }
}