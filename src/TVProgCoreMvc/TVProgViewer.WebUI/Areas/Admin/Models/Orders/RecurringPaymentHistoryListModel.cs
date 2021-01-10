using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment history list model
    /// </summary>
    public partial record RecurringPaymentHistoryListModel : BasePagedListModel<RecurringPaymentHistoryModel>
    {
    }
}