using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a never sold products report list model
    /// </summary>
    public partial record NeverSoldReportListModel : BasePagedListModel<NeverSoldReportModel>
    {
    }
}