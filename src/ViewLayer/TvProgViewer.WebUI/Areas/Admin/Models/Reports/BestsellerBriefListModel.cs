using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller brief list model
    /// </summary>
    public partial record BestsellerBriefListModel : BasePagedListModel<BestsellerModel>
    {
    }
}