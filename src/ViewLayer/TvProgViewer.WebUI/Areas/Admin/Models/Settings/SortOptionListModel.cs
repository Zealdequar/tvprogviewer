using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a sort option list model
    /// </summary>
    public partial record SortOptionListModel : BasePagedListModel<SortOptionModel>
    {
    }
}