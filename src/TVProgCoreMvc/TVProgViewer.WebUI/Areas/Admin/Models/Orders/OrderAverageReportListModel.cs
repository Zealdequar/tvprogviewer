using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Orders
{
    /// <summary>
    /// Represents an an order average report line summary list model
    /// </summary>
    public partial record OrderAverageReportListModel : BasePagedListModel<OrderAverageReportModel>
    {
    }
}