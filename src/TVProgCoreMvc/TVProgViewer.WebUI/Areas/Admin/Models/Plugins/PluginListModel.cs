using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Plugins
{
    /// <summary>
    /// Represents a plugin list model
    /// </summary>
    public partial record PluginListModel : BasePagedListModel<PluginModel>
    {
    }
}