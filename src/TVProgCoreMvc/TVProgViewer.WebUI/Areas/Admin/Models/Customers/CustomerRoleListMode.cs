using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user role list model
    /// </summary>
    public partial record UserRoleListModel : BasePagedListModel<UserRoleModel>
    {
    }
}