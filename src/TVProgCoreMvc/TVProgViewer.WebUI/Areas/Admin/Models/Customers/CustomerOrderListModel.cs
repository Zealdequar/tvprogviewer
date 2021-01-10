using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user order list model
    /// </summary>
    public partial record UserOrderListModel : BasePagedListModel<UserOrderModel>
    {
    }
}