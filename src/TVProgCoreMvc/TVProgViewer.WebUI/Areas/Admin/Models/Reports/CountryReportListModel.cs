using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a country report list model
    /// </summary>
    public partial record CountryReportListModel : BasePagedListModel<CountryReportModel>
    {
    }
}