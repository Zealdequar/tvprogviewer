using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user order list model
    /// </summary>
    public partial record UserOrderListModel : BasePagedListModel<UserOrderModel>
    {
    }
}