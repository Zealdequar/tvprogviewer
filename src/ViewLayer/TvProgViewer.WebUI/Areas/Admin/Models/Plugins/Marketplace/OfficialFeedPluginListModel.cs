using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Plugins.Marketplace
{
    /// <summary>
    /// Represents a list model of plugins of the official feed
    /// </summary>
    public partial record OfficialFeedPluginListModel : BasePagedListModel<OfficialFeedPluginModel>
    {
    }
}