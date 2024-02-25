using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.WebUI.Areas.Admin.Models.Directory;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the currency model factory
    /// </summary>
    public partial interface ICurrencyModelFactory
    {
        /// <summary>
        /// Prepare currency search model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <param name="prepareExchangeRates">Whether to prepare exchange rate models</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the currency search model
        /// </returns>
        Task<CurrencySearchModel> PrepareCurrencySearchModelAsync(CurrencySearchModel searchModel, bool prepareExchangeRates = false);

        /// <summary>
        /// Prepare paged currency list model
        /// </summary>
        /// <param name="searchModel">Currency search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the currency list model
        /// </returns>
        Task<CurrencyListModel> PrepareCurrencyListModelAsync(CurrencySearchModel searchModel);

        /// <summary>
        /// Prepare currency model
        /// </summary>
        /// <param name="model">Currency model</param>
        /// <param name="currency">Currency</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the currency model
        /// </returns>
        Task<CurrencyModel> PrepareCurrencyModelAsync(CurrencyModel model, Currency currency, bool excludeProperties = false);
    }
}