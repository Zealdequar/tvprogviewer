using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Discounts
{
    /// <summary>
    /// Represents a discount usage history list model
    /// </summary>
    public partial record DiscountUsageHistoryListModel : BasePagedListModel<DiscountUsageHistoryModel>
    {
    }
}