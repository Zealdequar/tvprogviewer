using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a sales summary list model
    /// </summary>
    public partial record SalesSummaryListModel : BasePagedListModel<SalesSummaryModel>
    {
    }
}
