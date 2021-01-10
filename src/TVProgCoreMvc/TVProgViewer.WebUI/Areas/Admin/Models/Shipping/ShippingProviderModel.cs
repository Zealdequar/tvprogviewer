using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Shipping
{
    /// <summary>
    /// Represents a shipping provider model
    /// </summary>
    public partial record ShippingProviderModel : BaseTvProgModel, IPluginModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.IsActive")]
        public bool IsActive { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Fields.Logo")]
        public string LogoUrl { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Shipping.Providers.Configure")]
        public string ConfigurationUrl { get; set; }

        #endregion
    }
}