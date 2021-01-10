using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user list model
    /// </summary>
    public partial record UserListModel : BasePagedListModel<UserModel>
    {
    }
}