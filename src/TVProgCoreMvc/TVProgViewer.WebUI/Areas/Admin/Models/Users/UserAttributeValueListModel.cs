using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value list model
    /// </summary>
    public partial record UserAttributeValueListModel : BasePagedListModel<UserAttributeValueModel>
    {
    }
}