using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user activity log list model
    /// </summary>
    public partial record UserActivityLogListModel : BasePagedListModel<UserActivityLogModel>
    {
    }
}