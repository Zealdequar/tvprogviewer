using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents an online user list model
    /// </summary>
    public partial record OnlineUserListModel : BasePagedListModel<OnlineUserModel>
    {
    }
}