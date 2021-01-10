using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Users
{
    /// <summary>
    /// Represents a user associated external authentication model
    /// </summary>
    public partial record UserAssociatedExternalAuthModel : BaseTvProgEntityModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.Email")]
        public string Email { get; set; }

        [TvProgResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.ExternalIdentifier")]
        public string ExternalIdentifier { get; set; }
        
        [TvProgResourceDisplayName("Admin.Users.Users.AssociatedExternalAuth.Fields.AuthMethodName")]
        public string AuthMethodName { get; set; }

        #endregion
    }
}