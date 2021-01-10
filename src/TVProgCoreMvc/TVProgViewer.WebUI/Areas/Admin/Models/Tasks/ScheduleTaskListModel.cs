using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Tasks
{
    /// <summary>
    /// Represents a schedule task list model
    /// </summary>
    public partial record ScheduleTaskListModel : BasePagedListModel<ScheduleTaskModel>
    {
    }
}