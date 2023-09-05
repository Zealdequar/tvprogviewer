using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents a recurring payment list model
    /// </summary>
    public partial record RecurringPaymentListModel : BasePagedListModel<RecurringPaymentModel>
    {
    }
}