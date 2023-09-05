using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user list model
    /// </summary>
    public partial record UserListModel : BasePagedListModel<UserModel>
    {
    }
}