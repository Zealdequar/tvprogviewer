using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user back in stock subscriptions list model
    /// </summary>
    public partial record UserBackInStockSubscriptionListModel : BasePagedListModel<UserBackInStockSubscriptionModel>
    {
    }
}