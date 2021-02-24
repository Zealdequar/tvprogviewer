using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user activity log list model
    /// </summary>
    public partial record UserActivityLogListModel : BasePagedListModel<UserActivityLogModel>
    {
    }
}