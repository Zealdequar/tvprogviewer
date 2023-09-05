using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Misc.Zettle.Models
{
    /// <summary>
    /// Represents a synchronization record list model
    /// </summary>
    public record SyncRecordListModel : BasePagedListModel<SyncRecordModel>
    {
    }
}