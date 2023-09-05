using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user attribute value list model
    /// </summary>
    public partial record UserAttributeValueListModel : BasePagedListModel<UserAttributeValueModel>
    {
    }
}