using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.MultiFactorAuthentication
{
    /// <summary>
    /// Represents an multi-factor authentication method model
    /// </summary>
    public partial record MultiFactorAuthenticationMethodModel : BaseTvProgModel, IPluginModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Fields.IsActive")]
        public bool IsActive { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Authentication.MultiFactorMethods.Configure")]
        public string ConfigurationUrl { get; set; }

        public string LogoUrl { get; set; }

        #endregion
    }
}
