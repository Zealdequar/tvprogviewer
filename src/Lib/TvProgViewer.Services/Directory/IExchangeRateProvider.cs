using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Directory
{
    /// <summary>
    /// Exchange rate provider interface
    /// </summary>
    public partial interface IExchangeRateProvider : IPlugin
    {
        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the exchange rates
        /// </returns>
        Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode);
    }
}