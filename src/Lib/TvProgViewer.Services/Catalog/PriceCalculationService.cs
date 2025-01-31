﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// Price calculation service
    /// </summary>
    public partial class PriceCalculationService : IPriceCalculationService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly CurrencySettings _currencySettings;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IDiscountService _discountService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelService _tvChannelService;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public PriceCalculationService(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IUserService userService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelService tvChannelService,
            IStaticCacheManager staticCacheManager)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _userService = userService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelService = tvChannelService;
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets allowed discounts applied to tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discounts
        /// </returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToTvChannelAsync(TvChannel tvChannel, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            if (!tvChannel.HasDiscountsApplied)
                return allowedDiscounts;

            //we use this property ("HasDiscountsApplied") for performance optimization to avoid unnecessary database calls
            foreach (var discount in await _discountService.GetAppliedDiscountsAsync(tvChannel))
                if (discount.DiscountType == DiscountType.AssignedToSkus &&
                    (await _discountService.ValidateDiscountAsync(discount, user)).IsValid)
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to categories
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discounts
        /// </returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToCategoriesAsync(TvChannel tvChannel, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //load cached discount models (performance optimization)
            foreach (var discount in await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories))
            {
                //load identifier of categories with this discount applied to
                var discountCategoryIds = await _categoryService.GetAppliedCategoryIdsAsync(discount, user);

                //compare with categories of this tvChannel
                var tvChannelCategoryIds = new List<int>();
                if (discountCategoryIds.Any())
                {
                    tvChannelCategoryIds = (await _categoryService
                        .GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id))
                        .Select(x => x.CategoryId)
                        .ToList();
                }

                foreach (var categoryId in tvChannelCategoryIds)
                {
                    if (!discountCategoryIds.Contains(categoryId))
                        continue;

                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, user)).IsValid)
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to manufacturers
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discounts
        /// </returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToManufacturersAsync(TvChannel tvChannel, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            foreach (var discount in await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers))
            {
                //load identifier of manufacturers with this discount applied to
                var discountManufacturerIds = await _manufacturerService.GetAppliedManufacturerIdsAsync(discount, user);

                //compare with manufacturers of this tvChannel
                var tvChannelManufacturerIds = new List<int>();
                if (discountManufacturerIds.Any())
                {
                    tvChannelManufacturerIds =
                        (await _manufacturerService
                        .GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id))
                        .Select(x => x.ManufacturerId)
                        .ToList();
                }

                foreach (var manufacturerId in tvChannelManufacturerIds)
                {
                    if (!discountManufacturerIds.Contains(manufacturerId))
                        continue;

                    if (!_discountService.ContainsDiscount(allowedDiscounts, discount) &&
                        (await _discountService.ValidateDiscountAsync(discount, user)).IsValid)
                        allowedDiscounts.Add(discount);
                }
            }

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discounts
        /// </returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAsync(TvChannel tvChannel, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //discounts applied to tvChannels
            foreach (var discount in await GetAllowedDiscountsAppliedToTvChannelAsync(tvChannel, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to categories
            foreach (var discount in await GetAllowedDiscountsAppliedToCategoriesAsync(tvChannel, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to manufacturers
            foreach (var discount in await GetAllowedDiscountsAppliedToManufacturersAsync(tvChannel, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="tvChannelPriceWithoutDiscount">Already calculated tvChannel price without discount</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the discount amount, Applied discounts
        /// </returns>
        protected virtual async Task<(decimal, List<Discount>)> GetDiscountAmountAsync(TvChannel tvChannel,
            User user,
            decimal tvChannelPriceWithoutDiscount)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var appliedDiscounts = new List<Discount>();
            var appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to tvChannels with price entered by a user
            if (tvChannel.UserEntersPrice)
                return (appliedDiscountAmount, appliedDiscounts);

            //discounts are disabled
            if (_catalogSettings.IgnoreDiscounts)
                return (appliedDiscountAmount, appliedDiscounts);

            var allowedDiscounts = await GetAllowedDiscountsAsync(tvChannel, user);

            //no discounts
            if (!allowedDiscounts.Any())
                return (appliedDiscountAmount, appliedDiscounts);

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, tvChannelPriceWithoutDiscount, out appliedDiscountAmount);

            return (appliedDiscountAmount, appliedDiscounts);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        public virtual async Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvChannel,
            User user,
            Store store,
            decimal additionalCharge = 0,
            bool includeDiscounts = true,
            int quantity = 1)
        {
            return await GetFinalPriceAsync(tvChannel, user, store,
                additionalCharge, includeDiscounts, quantity,
                null, null);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental tvChannels)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental tvChannels)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        public virtual async Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvChannel,
            User user,
            Store store,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            return await GetFinalPriceAsync(tvChannel, user, store, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">The user</param>
        /// <param name="store">Store</param>
        /// <param name="overriddenTvChannelPrice">Overridden tvChannel price. If specified, then it'll be used instead of a tvChannel price. For example, used with tvChannel attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental tvChannels)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental tvChannels)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the final price without discounts, Final price, Applied discount amount, Applied discounts
        /// </returns>
        public virtual async Task<(decimal priceWithoutDiscounts, decimal finalPrice, decimal appliedDiscountAmount, List<Discount> appliedDiscounts)> GetFinalPriceAsync(TvChannel tvChannel,
            User user,
            Store store,
            decimal? overriddenTvChannelPrice,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelPriceCacheKey,
                tvChannel,
                overriddenTvChannelPrice,
                additionalCharge,
                includeDiscounts,
                quantity,
                await _userService.GetUserRoleIdsAsync(user),
                store);

            //we do not cache price if this not allowed by settings or if the tvChannel is rental tvChannel
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (!_catalogSettings.CacheTvChannelPrices || tvChannel.IsRental)
                cacheKey.CacheTime = 0;

            decimal rezPrice;
            decimal rezPriceWithoutDiscount;
            decimal discountAmount;
            List<Discount> appliedDiscounts;

            (rezPriceWithoutDiscount, rezPrice, discountAmount, appliedDiscounts) = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var discounts = new List<Discount>();
                var appliedDiscountAmount = decimal.Zero;

                //initial price
                var price = overriddenTvChannelPrice ?? tvChannel.Price;

                //tier prices
                var tierPrice = await _tvChannelService.GetPreferredTierPriceAsync(tvChannel, user, store, quantity);

                if (tierPrice != null)
                    price = tierPrice.Price;

                //additional charge
                price += additionalCharge;

                //rental tvChannels
                if (tvChannel.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price *= _tvChannelService.GetRentalPeriods(tvChannel, rentalStartDate.Value, rentalEndDate.Value);

                var priceWithoutDiscount = price;

                if (includeDiscounts)
                {
                    //discount
                    var (tmpDiscountAmount, tmpAppliedDiscounts) = await GetDiscountAmountAsync(tvChannel, user, price);
                    price -= tmpDiscountAmount;

                    if (tmpAppliedDiscounts?.Any() ?? false)
                    {
                        discounts.AddRange(tmpAppliedDiscounts);
                        appliedDiscountAmount = tmpDiscountAmount;
                    }
                }

                if (price < decimal.Zero)
                    price = decimal.Zero;

                if (priceWithoutDiscount < decimal.Zero)
                    priceWithoutDiscount = decimal.Zero;

                return (priceWithoutDiscount, price, appliedDiscountAmount, discounts);
            });

            return (rezPriceWithoutDiscount, rezPrice, discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets the tvChannel cost (one item)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel cost (one item)
        /// </returns>
        public virtual async Task<decimal> GetTvChannelCostAsync(TvChannel tvChannel, string attributesXml)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var cost = tvChannel.TvChannelCost;
            var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        cost += attributeValue.Cost;
                        break;
                    case AttributeValueType.AssociatedToTvChannel:
                        //bundled tvChannel
                        var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(attributeValue.AssociatedTvChannelId);
                        if (associatedTvChannel != null)
                            cost += associatedTvChannel.TvChannelCost * attributeValue.Quantity;
                        break;
                    default:
                        break;
                }
            }

            return cost;
        }

        /// <summary>
        /// Get a price adjustment of a tvChannel attribute value
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="value">TvChannel attribute value</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="tvChannelPrice">TvChannel price (null for using the base tvChannel price)</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price adjustment
        /// </returns>
        public virtual async Task<decimal> GetTvChannelAttributeValuePriceAdjustmentAsync(TvChannel tvChannel,
            TvChannelAttributeValue value,
            User user,
            Store store,
            decimal? tvChannelPrice = null,
            int quantity = 1)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var adjustment = decimal.Zero;
            switch (value.AttributeValueType)
            {
                case AttributeValueType.Simple:
                    //simple attribute
                    if (value.PriceAdjustmentUsePercentage)
                    {
                        if (!tvChannelPrice.HasValue)
                            tvChannelPrice = (await GetFinalPriceAsync(tvChannel, user, store, quantity: quantity)).finalPrice;

                        adjustment = (decimal)((float)tvChannelPrice * (float)value.PriceAdjustment / 100f);
                    }
                    else
                    {
                        adjustment = value.PriceAdjustment;
                    }

                    break;
                case AttributeValueType.AssociatedToTvChannel:
                    //bundled tvChannel
                    var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(value.AssociatedTvChannelId);
                    if (associatedTvChannel != null)
                        adjustment = (await GetFinalPriceAsync(associatedTvChannel, user, store)).finalPrice * value.Quantity;

                    break;
                default:
                    break;
            }

            return adjustment;
        }

        /// <summary>
        /// Round a tvChannel or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rounded value
        /// </returns>
        public virtual async Task<decimal> RoundPriceAsync(decimal value, Currency currency = null)
        {
            //we use this method because some currencies (e.g. Gungarian Forint or Swiss Franc) use non-standard rules for rounding
            //you can implement any rounding logic here

            currency ??= await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId);

            return Round(value, currency.RoundingType);
        }

        /// <summary>
        /// Round
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="roundingType">The rounding type</param>
        /// <returns>Rounded value</returns>
        public virtual decimal Round(decimal value, RoundingType roundingType)
        {
            //default round (Rounding001)
            var rez = Math.Round(value, 2);
            var fractionPart = (rez - Math.Truncate(rez)) * 10;

            //cash rounding not needed
            if (fractionPart == 0)
                return rez;

            //Cash rounding (details: https://en.wikipedia.org/wiki/Cash_rounding)
            switch (roundingType)
            {
                //rounding with 0.05 or 5 intervals
                case RoundingType.Rounding005Up:
                case RoundingType.Rounding005Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    fractionPart %= 5;
                    if (fractionPart == 0)
                        break;

                    if (roundingType == RoundingType.Rounding005Up)
                        fractionPart = 5 - fractionPart;
                    else
                        fractionPart *= -1;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.10 intervals
                case RoundingType.Rounding01Up:
                case RoundingType.Rounding01Down:
                    fractionPart = (fractionPart - Math.Truncate(fractionPart)) * 10;

                    if (roundingType == RoundingType.Rounding01Down && fractionPart == 5)
                        fractionPart = -5;
                    else
                        fractionPart = fractionPart < 5 ? fractionPart * -1 : 10 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 0.50 intervals
                case RoundingType.Rounding05:
                    fractionPart *= 10;
                    fractionPart = fractionPart < 25 ? fractionPart * -1 : fractionPart < 50 || fractionPart < 75 ? 50 - fractionPart : 100 - fractionPart;

                    rez += fractionPart / 100;
                    break;
                //rounding with 1.00 intervals
                case RoundingType.Rounding1:
                case RoundingType.Rounding1Up:
                    fractionPart *= 10;

                    if (roundingType == RoundingType.Rounding1Up && fractionPart > 0)
                        rez = Math.Truncate(rez) + 1;
                    else
                        rez = fractionPart < 50 ? Math.Truncate(rez) : Math.Truncate(rez) + 1;

                    break;
                case RoundingType.Rounding001:
                default:
                    break;
            }

            return rez;
        }

        #endregion
    }
}