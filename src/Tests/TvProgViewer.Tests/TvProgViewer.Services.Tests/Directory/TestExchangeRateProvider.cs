using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.Directory
{
    public class TestExchangeRateProvider : BasePlugin, IExchangeRateProvider
    {
        /// <summary>
        /// Gets currency live rates
        /// </summary>
        /// <param name="exchangeRateCurrencyCode">Exchange rate currency code</param>
        /// <returns>Exchange rates</returns>
        public Task<IList<ExchangeRate>> GetCurrencyLiveRatesAsync(string exchangeRateCurrencyCode)
        {
            return Task.FromResult<IList<ExchangeRate>>(new List<ExchangeRate>());
        }
    }
}
