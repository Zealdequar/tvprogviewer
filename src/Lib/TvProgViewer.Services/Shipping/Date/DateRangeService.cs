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
        private readonly IRepository<TvChannelAvailabilityRange> _tvChannelAvailabilityRangeRepository;

        #endregion

        #region Ctor

        public DateRangeService(IRepository<DeliveryDate> deliveryDateRepository,
            IRepository<TvChannelAvailabilityRange> tvChannelAvailabilityRangeRepository)
        {
            _deliveryDateRepository = deliveryDateRepository;
            _tvChannelAvailabilityRangeRepository = tvChannelAvailabilityRangeRepository;
        }

        #endregion

        #region Methods

        #region Delivery dates

        /// <summary>
        /// Get a delivery date
        /// </summary>
        /// <param name="deliveryDateId">The delivery date identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
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
        /// Задача представляет асинхронную операцию
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
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.InsertAsync(deliveryDate);
        }

        /// <summary>
        /// Update the delivery date
        /// </summary>
        /// <param name="deliveryDate">Delivery date</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.UpdateAsync(deliveryDate);
        }

        /// <summary>
        /// Delete a delivery date
        /// </summary>
        /// <param name="deliveryDate">The delivery date</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteDeliveryDateAsync(DeliveryDate deliveryDate)
        {
            await _deliveryDateRepository.DeleteAsync(deliveryDate);
        }

        #endregion

        #region TvChannel availability ranges

        /// <summary>
        /// Get a tvChannel availability range
        /// </summary>
        /// <param name="tvChannelAvailabilityRangeId">The tvChannel availability range identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel availability range
        /// </returns>
        public virtual async Task<TvChannelAvailabilityRange> GetTvChannelAvailabilityRangeByIdAsync(int tvChannelAvailabilityRangeId)
        {
            return tvChannelAvailabilityRangeId != 0 ? await _tvChannelAvailabilityRangeRepository.GetByIdAsync(tvChannelAvailabilityRangeId, cache => default) : null;
        }

        /// <summary>
        /// Get all tvChannel availability ranges
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel availability ranges
        /// </returns>
        public virtual async Task<IList<TvChannelAvailabilityRange>> GetAllTvChannelAvailabilityRangesAsync()
        {
            return await _tvChannelAvailabilityRangeRepository.GetAllAsync(query =>
            {
                return from par in query
                    orderby par.DisplayOrder, par.Id
                    select par;
            }, cache => default);
        }

        /// <summary>
        /// Insert the tvChannel availability range
        /// </summary>
        /// <param name="tvChannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvChannelAvailabilityRange)
        {
            await _tvChannelAvailabilityRangeRepository.InsertAsync(tvChannelAvailabilityRange);
        }

        /// <summary>
        /// Update the tvChannel availability range
        /// </summary>
        /// <param name="tvChannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvChannelAvailabilityRange)
        {
            await _tvChannelAvailabilityRangeRepository.UpdateAsync(tvChannelAvailabilityRange);
        }

        /// <summary>
        /// Delete the tvChannel availability range
        /// </summary>
        /// <param name="tvChannelAvailabilityRange">TvChannel availability range</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAvailabilityRangeAsync(TvChannelAvailabilityRange tvChannelAvailabilityRange)
        {
            await _tvChannelAvailabilityRangeRepository.DeleteAsync(tvChannelAvailabilityRange);
        }

        #endregion

        #endregion
    }
}