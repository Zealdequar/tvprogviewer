using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a reward points list model
    /// </summary>
    public partial record UserRewardPointsListModel : BasePagedListModel<UserRewardPointsModel>
    {
    }
}