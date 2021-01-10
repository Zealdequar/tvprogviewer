using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using TVProgViewer.Web.Framework.Models;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Tax
{
    /// <summary>
    /// Represents a tax provider model
    /// </summary>
    public partial record TaxProviderModel : BaseTvProgModel, IPluginModel
    {
        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.FriendlyName")]
        public string FriendlyName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.SystemName")]
        public string SystemName { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Tax.Providers.Fields.IsPrimaryTaxProvider")]
        public bool IsPrimaryTaxProvider { get; set; }
        
        [TvProgResourceDisplayName("Admin.Configuration.Tax.Providers.Configure")]
        public string ConfigurationUrl { get; set; }

        public string LogoUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }

        #endregion
    }
}