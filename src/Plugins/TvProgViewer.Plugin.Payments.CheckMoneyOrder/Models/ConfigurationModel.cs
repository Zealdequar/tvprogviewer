using System.Collections.Generic;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Plugin.Payments.CheckMoneyOrder.Models
{
    public record ConfigurationModel : BaseTvProgModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        public ConfigurationModel()
        {
            Locales = new List<ConfigurationLocalizedModel>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }
        
        [TvProgResourceDisplayName("Plugins.Payment.CheckMoneyOrder.DescriptionText")]
        public string DescriptionText { get; set; }
        public bool DescriptionText_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payment.CheckMoneyOrder.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFee_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payment.CheckMoneyOrder.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentage_OverrideForStore { get; set; }

        [TvProgResourceDisplayName("Plugins.Payment.CheckMoneyOrder.ShippableTvChannelRequired")]
        public bool ShippableTvChannelRequired { get; set; }
        public bool ShippableTvChannelRequired_OverrideForStore { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }

        #region Nested class

        public class ConfigurationLocalizedModel : ILocalizedLocaleModel
        {
            public int LanguageId { get; set; }
            
            [TvProgResourceDisplayName("Plugins.Payment.CheckMoneyOrder.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion

    }
}