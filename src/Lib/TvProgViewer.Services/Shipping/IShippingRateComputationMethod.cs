using System.Threading.Tasks;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Shipping.Tracking;

namespace TvProgViewer.Services.Shipping
{
    /// <summary>
    /// Provides an interface of shipping rate computation method
    /// </summary>
    public partial interface IShippingRateComputationMethod : IPlugin
    {
        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the represents a response of getting shipping rate options
        /// </returns>
        Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the fixed shipping rate; or null in case there's no fixed shipping rate
        /// </returns>
        Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Get associated shipment tracker
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment tracker
        /// </returns>
        Task<IShipmentTracker> GetShipmentTrackerAsync();
    }
}