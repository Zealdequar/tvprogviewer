using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated user list model
    /// </summary>
    public partial record AffiliatedUserListModel : BasePagedListModel<AffiliatedUserModel>
    {
    }
}