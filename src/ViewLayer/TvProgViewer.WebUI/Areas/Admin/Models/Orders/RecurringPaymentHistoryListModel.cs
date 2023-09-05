using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment history list model
    /// </summary>
    public partial record RecurringPaymentHistoryListModel : BasePagedListModel<RecurringPaymentHistoryModel>
    {
    }
}