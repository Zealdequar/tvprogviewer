using System.Collections.Generic;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a associated external auth records search model
    /// </summary>
    public record UserAssociatedExternalAuthRecordsSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
        public IList<UserAssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; } = new List<UserAssociatedExternalAuthModel>();
        
        #endregion
    }
}
