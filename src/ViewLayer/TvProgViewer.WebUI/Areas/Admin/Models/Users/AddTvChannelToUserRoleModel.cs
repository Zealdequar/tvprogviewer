using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a tvChannel model to add to the user role 
    /// </summary>
    public partial record AddTvChannelToUserRoleModel : BaseTvProgEntityModel
    {
        #region Properties

        public int AssociatedToTvChannelId { get; set; }

        #endregion
    }
}