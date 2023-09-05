using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a country report list model
    /// </summary>
    public partial record CountryReportListModel : BasePagedListModel<CountryReportModel>
    {
    }
}