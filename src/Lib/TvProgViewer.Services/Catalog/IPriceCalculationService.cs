using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial interface IPriceCalculationService
    {
        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvchannel,
            User user,
            Store store,
            decimal additionalCharge = 0,
            bool includeDiscounts = true,
            int quantity = 1);

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental tvchannels)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental tvchannels)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvchannel,
            User user,
            Store store,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate);

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="overriddenTvChannelPrice">Overridden tvchannel price. If specified, then it'll be used instead of a tvchannel price. For example, used with tvchannel attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental tvchannels)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental tvchannels)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvchannel,
            User user,
            Store store,
            decimal? overriddenTvChannelPrice,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate);

        /// <summary>
        /// Gets the tvchannel cost (one item)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the tvchannel cost (one item)
        /// </returns>
        Task<decimal> GetTvChannelCostAsync(TvChannel tvchannel, string attributesXml);

        /// <summary>
        /// Get a price adjustment of a tvchannel attribute value
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="value">TvChannel attribute value</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="tvchannelPrice">TvChannel price (null for using the base tvchannel price)</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the price adjustment
        /// </returns>
        Task<decimal> GetTvChannelAttributeValuePriceAdjustmentAsync(TvChannel tvchannel,
            TvChannelAttributeValue value,
            User user,
            Store store,
            decimal? tvchannelPrice = null,
            int quantity = 1);

        /// <summary>
        /// Round a tvchannel or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the rounded value
        /// </returns>
        Task<decimal> RoundPriceAsync(decimal value, Currency currency = null);

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        decimal Round(decimal value, RoundingType roundingType);
    }
}