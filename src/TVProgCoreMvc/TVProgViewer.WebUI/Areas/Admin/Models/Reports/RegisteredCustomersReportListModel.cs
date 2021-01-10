using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a registered users report list model
    /// </summary>
    public partial record RegisteredUsersReportListModel : BasePagedListModel<RegisteredUsersReportModel>
    {
    }
}