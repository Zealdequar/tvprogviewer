using TvProgViewer.WebUI.Areas.Admin.Models.Orders;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Catalog
{
    /// <summary>
    /// Represents a product order list model
    /// </summary>
    public partial record ProductOrderListModel : BasePagedListModel<OrderModel>
    {
    }
}