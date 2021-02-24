using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user back in stock subscriptions list model
    /// </summary>
    public partial record UserBackInStockSubscriptionListModel : BasePagedListModel<UserBackInStockSubscriptionModel>
    {
    }
}