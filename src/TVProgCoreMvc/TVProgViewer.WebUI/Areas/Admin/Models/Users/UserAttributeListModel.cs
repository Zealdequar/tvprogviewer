using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute list model
    /// </summary>
    public partial record UserAttributeListModel : BasePagedListModel<UserAttributeModel>
    {
    }
}