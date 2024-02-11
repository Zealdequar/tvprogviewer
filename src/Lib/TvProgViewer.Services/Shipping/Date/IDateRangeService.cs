using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Services.Shipping.Date
{
    /// <summary>
    /// Date range service interface
    /// </summary>
    public partial interface IDateRangeService
    {
        #region Delivery dates

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate);

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery date
        /// </returns>
        Task<DeliveryDate> GetDeliveryDateByIdAsync(int deliveryDateId);

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery dates
        /// </returns>
        Task<IList<DeliveryDate>> GetAllDeliveryDatesAsync();

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertDeliveryDateAsync(DeliveryDate deliveryDate);

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate);

        #endregion

        #region TvChannel availability ranges

        /// <summary>
        /// Get a tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRangeId">The tvchannel availability range identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel availability range
        /// </returns>
        Task<TvChannelAvailabilityRange> GetTvChannelAvailabilityRangeByIdAsync(int tvchannelAvailabilityRangeId);

        /// <summary>
        /// Get all tvchannel availability ranges
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel availability ranges
        /// </returns>
        Task<IList<TvChannelAvailabilityRange>> GetAllTvChannelAvailabilityRangesAsync();

        /// <summary>
        /// Insert the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task InsertTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange);

        /// <summary>
        /// Update the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task UpdateTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange);

        /// <summary>
        /// Delete the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        Task DeleteTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange);

        #endregion
    }
}
