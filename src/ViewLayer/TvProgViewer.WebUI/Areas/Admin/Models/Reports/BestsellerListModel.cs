using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Reports
{
    /// <summary>
    /// Represents a bestseller list model
    /// </summary>
    public partial record BestsellerListModel : BasePagedListModel<BestsellerModel>
    {
    }
}