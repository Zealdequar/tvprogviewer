using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Logging
{
    /// <summary>
    /// Represents an activity log list model
    /// </summary>
    public partial record ActivityLogListModel : BasePagedListModel<ActivityLogModel>
    {
    }
}