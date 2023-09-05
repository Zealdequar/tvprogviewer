using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a reward points search model
    /// </summary>
    public partial record UserRewardPointsSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserId { get; set; }
        
        #endregion
    }
}