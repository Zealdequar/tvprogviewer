using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a gift card usage history list model
    /// </summary>
    public partial record GiftCardUsageHistoryListModel : BasePagedListModel<GiftCardUsageHistoryModel>
    {
    }
}