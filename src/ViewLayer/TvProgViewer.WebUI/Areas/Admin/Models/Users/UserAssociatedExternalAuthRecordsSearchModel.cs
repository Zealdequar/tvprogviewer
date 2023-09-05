using System.Collections.Generic;
using TvProgViewer.Web.Framework.Models;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TvProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a associated external auth records search model
    /// </summary>
    public partial record UserAssociatedExternalAuthRecordsSearchModel : BaseSearchModel
    {
        #region Properties

        public int UserId { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth")]
        public IList<UserAssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; } = new List<UserAssociatedExternalAuthModel>();
        
        #endregion
    }
}
