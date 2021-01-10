using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card usage history list model
    /// </summary>
    public record GiftCardUsageHistoryListModel : BasePagedListModel<GiftCardUsageHistoryModel>
    {
    }
}