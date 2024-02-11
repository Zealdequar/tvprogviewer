using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Data;

namespace TvProgViewer.Services.Shipping.Date
{
    /// <summary>
    /// Represents the date range service
    /// </summary>
    public partial class DateRangeService : IDateRangeService
    {
        #region Fields

        private readonly IRepository<DeliveryDate> _deliveryDateRepository;
        private readonly IRepository<TvChannelAvailabilityRange> _tvchannelAvailabilityRangeRepository;

        #endregion

        #region Ctor

        public DateRangeService(IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<TvChannelAvailabilityRange> tvchannelAvailabilityRangeRepository)
        {
            _deliveryDateRepository = deliveryDateRepository;
            _tvchannelAvailabilityRangeRepository = tvchannelAvailabilityRangeRepository;
        }

        #endregion

        #region Methods

        #region Delivery dates

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery date
        /// </returns>
        public virtual async Task<DeliveryDate> GetDeliveryDateByIdAsync(int deliveryDateId)
        {
            return await _deliveryDateRepository.GetByIdAsync(deliveryDateId, cache => default);
        }

        /// <summary>
        /// Get all delivery dates
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the delivery dates
        /// </returns>
        public virtual async Task<IList<DeliveryDate>> GetAllDeliveryDatesAsync()
        {
            var deliveryDates = await _deliveryDateRepository.GetAllAsync(query =>
            {
                return from dd in query
                    orderby dd.DisplayOrder, dd.Id
                    select dd;
            }, cache => default);

            return deliveryDates;
        }

        /// <summary>
        /// Insert a delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.InsertAsync(deliveryDate);
        }

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.UpdateAsync(deliveryDate);
        }

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.DeleteAsync(deliveryDate);
        }

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
        public virtual async Task<TvChannelAvailabilityRange> GetTvChannelAvailabilityRangeByIdAsync(int tvchannelAvailabilityRangeId)
        {
            return tvchannelAvailabilityRangeId != 0 ? await _tvchannelAvailabilityRangeRepository.GetByIdAsync(tvchannelAvailabilityRangeId, cache => default) : null;
        }

        /// <summary>
        /// Get all tvchannel availability ranges
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel availability ranges
        /// </returns>
        public virtual async Task<IList<TvChannelAvailabilityRange>> GetAllTvChannelAvailabilityRangesAsync()
        {
            return await _tvchannelAvailabilityRangeRepository.GetAllAsync(query =>
            {
                return from par in query
                    orderby par.DisplayOrder, par.Id
                    select par;
            }, cache => default);
        }

        /// <summary>
        /// Insert the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task InsertTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange)
        {
            await _tvchannelAvailabilityRangeRepository.InsertAsync(tvchannelAvailabilityRange);
        }

        /// <summary>
        /// Update the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task UpdateTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange)
        {
            await _tvchannelAvailabilityRangeRepository.UpdateAsync(tvchannelAvailabilityRange);
        }

        /// <summary>
        /// Delete the tvchannel availability range
        /// </summary>
        /// <param name="tvchannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task DeleteTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvchannelAvailabilityRange)
        {
            await _tvchannelAvailabilityRangeRepository.DeleteAsync(tvchannelAvailabilityRange);
        }

        #endregion

        #endregion
    }
}