using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
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