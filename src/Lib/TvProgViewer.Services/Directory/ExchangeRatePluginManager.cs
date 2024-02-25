using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Directory
{
    /// <summary>
    /// Represents an exchange rate plugin manager implementation
    /// </summary>
    public partial class ExchangeRatePluginManager : PluginManager<IExchangeRateProvider>, IExchangeRatePluginManager
    {
        #region Fields

        private readonly CurrencySettings _currencySettings;

        #endregion

        #region Ctor

        public ExchangeRatePluginManager(CurrencySettings currencySettings,
            IUserService userService,
            IPluginService pluginService) : base(userService, pluginService)
        {
            _currencySettings = currencySettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Load primary active exchange rate provider
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the exchange rate provider
        /// </returns>
        public virtual async Task<IExchangeRateProvider> LoadPrimaryPluginAsync(User user = null, int storeId = 0)
        {
            return await LoadPrimaryPluginAsync(_currencySettings.ActiveExchangeRateProviderSystemName, user, storeId);
        }

        /// <summary>
        /// Check whether the passed exchange rate provider is active
        /// </summary>
        /// <param name="exchangeRateProvider">Exchange rate provider to check</param>
        /// <returns>Result</returns>
        public virtual bool IsPluginActive(IExchangeRateProvider exchangeRateProvider)
        {
            return IsPluginActive(exchangeRateProvider, new List<string> { _currencySettings.ActiveExchangeRateProviderSystemName });
        }

        #endregion
    }
}