using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a setting list model
    /// </summary>
    public partial record SettingListModel : BasePagedListModel<SettingModel>
    {
    }
}