using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount usage history list model
    /// </summary>
    public partial record DiscountUsageHistoryListModel : BasePagedListModel<DiscountUsageHistoryModel>
    {
    }
}