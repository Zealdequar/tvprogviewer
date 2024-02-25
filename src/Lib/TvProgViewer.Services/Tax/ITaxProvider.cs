using System.Threading.Tasks;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="taxRateRequest">Tax rate request</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax
        /// </returns>
        Task<TaxRateResult> GetTaxRateAsync(TaxRateRequest taxRateRequest);

        /// <summary>
        /// Gets tax total
        /// </summary>
        /// <param name="taxTotalRequest">Tax total request</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ax total
        /// </returns>
        Task<TaxTotalResult> GetTaxTotalAsync(TaxTotalRequest taxTotalRequest);
    }
}
