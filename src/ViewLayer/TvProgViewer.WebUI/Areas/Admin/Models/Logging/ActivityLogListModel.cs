using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log list model
    /// </summary>
    public partial record ActivityLogListModel : BasePagedListModel<ActivityLogModel>
    {
    }
}