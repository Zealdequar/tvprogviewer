using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute list model
    /// </summary>
    public partial record UserAttributeListModel : BasePagedListModel<UserAttributeModel>
    {
    }
}