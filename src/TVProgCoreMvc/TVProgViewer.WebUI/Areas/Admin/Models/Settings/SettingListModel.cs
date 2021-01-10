using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Settings
{
    /// <summary>
    /// Represents a setting list model
    /// </summary>
    public partial record SettingListModel : BasePagedListModel<SettingModel>
    {
    }
}