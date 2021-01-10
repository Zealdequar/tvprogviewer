using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a sort option list model
    /// </summary>
    public partial record SortOptionListModel : BasePagedListModel<SortOptionModel>
    {
    }
}