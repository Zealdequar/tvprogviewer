using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Tasks
{
    /// <summary>
    /// Represents a schedule task list model
    /// </summary>
    public partial record ScheduleTaskListModel : BasePagedListModel<ScheduleTaskModel>
    {
    }
}