using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Plugins
{
    /// <summary>
    /// Represents a plugin list model
    /// </summary>
    public partial record PluginListModel : BasePagedListModel<PluginModel>
    {
    }
}