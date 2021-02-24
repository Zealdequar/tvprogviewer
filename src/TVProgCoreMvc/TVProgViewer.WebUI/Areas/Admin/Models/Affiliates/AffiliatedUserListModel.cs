using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Affiliates
{
    /// <summary>
    /// Represents an affiliated user list model
    /// </summary>
    public partial record AffiliatedUserListModel : BasePagedListModel<AffiliatedUserModel>
    {
    }
}