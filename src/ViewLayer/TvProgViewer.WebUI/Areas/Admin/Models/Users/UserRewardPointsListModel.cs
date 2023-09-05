using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a reward points list model
    /// </summary>
    public partial record UserRewardPointsListModel : BasePagedListModel<UserRewardPointsModel>
    {
    }
}