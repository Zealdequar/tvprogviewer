using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a tvChannel list model to add for synchronization
    /// </summary>
    public record AddTvChannelToSyncListModel : BasePagedListModel<TvChannelModel>
    {
    }
}