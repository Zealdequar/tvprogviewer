using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents an online user list model
    /// </summary>
    public partial record OnlineUserListModel : BasePagedListModel<OnlineUserModel>
    {
    }
}