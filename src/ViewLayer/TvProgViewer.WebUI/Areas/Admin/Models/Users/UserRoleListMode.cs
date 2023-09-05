using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role list model
    /// </summary>
    public partial record UserRoleListModel : BasePagedListModel<UserRoleModel>
    {
    }
}