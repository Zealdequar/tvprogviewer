using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a registered users report list model
    /// </summary>
    public partial record RegisteredUsersReportListModel : BasePagedListModel<RegisteredUsersReportModel>
    {
    }
}