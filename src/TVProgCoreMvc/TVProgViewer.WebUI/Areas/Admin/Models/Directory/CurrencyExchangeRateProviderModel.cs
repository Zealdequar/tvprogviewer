using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using TVProgViewer.Web.Framework.Models;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;

namespace TVProgViewer.WebUI.Areas.Admin.Models.Directory
{
    /// <summary>
    /// Represents a currency exchange rate provider model
    /// </summary>
    public partial record CurrencyExchangeRateProviderModel : BaseTvProgModel
    {
        #region Ctor

        public CurrencyExchangeRateProviderModel()
        {
            ExchangeRates = new List<CurrencyExchangeRateModel>();
            ExchangeRateProviders = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [TvProgResourceDisplayName("Admin.Configuration.Currencies.Fields.CurrencyRateAutoUpdateEnabled")]
        public bool AutoUpdateEnabled { get; set; }

        public IList<CurrencyExchangeRateModel> ExchangeRates { get; set; }

        [TvProgResourceDisplayName("Admin.Configuration.Currencies.Fields.ExchangeRateProvider")]
        public string ExchangeRateProvider { get; set; }
        public IList<SelectListItem> ExchangeRateProviders { get; set; }

        #endregion
    }
}