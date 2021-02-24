using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Discounts;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Discounts;
using System.Threading.Tasks;

namespace TVProgViewer.Services.Catalog
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
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductService _productService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public PriceCalculationService(CatalogSettings catalogSettings,
            CurrencySettings currencySettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IUserService userService,
            IDiscountService discountService,
            IManufacturerService manufacturerService,
            IProductAttributeParser productAttributeParser,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _catalogSettings = catalogSettings;
            _currencySettings = currencySettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _userService = userService;
            _discountService = discountService;
            _manufacturerService = manufacturerService;
            _productAttributeParser = productAttributeParser;
            _productService = productService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets allowed discounts applied to product
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Discounts</returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToProductAsync(Product product, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            if (!product.HasDiscountsApplied)
                return allowedDiscounts;

            //we use this property ("HasDiscountsApplied") for performance optimization to async Task unnecessary database calls
            foreach (var discount in await _discountService.GetAppliedDiscountsAsync(product))
                if (discount.DiscountType == DiscountType.AssignedToSkus &&
                    (await _discountService.ValidateDiscountAsync(discount, user)).IsValid)
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets allowed discounts applied to categories
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Discounts</returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToCategoriesAsync(Product product, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //load cached discount models (performance optimization)
            foreach (var discount in await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories))
            {
                //load identifier of categories with this discount applied to
                var discountCategoryIds = await _categoryService.GetAppliedCategoryIdsAsync(discount, user);

                //compare with categories of this product
                var productCategoryIds = new List<int>();
                if (discountCategoryIds.Any())
                {
                    productCategoryIds = (await _categoryService
                        .GetProductCategoriesByProductIdAsync(product.Id))
                        .Select(x => x.CategoryId)
                        .ToList();
                }

                foreach (var categoryId in productCategoryIds)
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
        /// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Discounts</returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAppliedToManufacturersAsync(Product product, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            foreach (var discount in await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToManufacturers))
            {
                //load identifier of manufacturers with this discount applied to
                var discountManufacturerIds = await _manufacturerService.GetAppliedManufacturerIdsAsync(discount, user);

                //compare with manufacturers of this product
                var productManufacturerIds = new List<int>();
                if (discountManufacturerIds.Any())
                {
                    productManufacturerIds =
                        (await _manufacturerService
                        .GetProductManufacturersByProductIdAsync(product.Id))
                        .Select(x => x.ManufacturerId)
                        .ToList();
                }

                foreach (var manufacturerId in productManufacturerIds)
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
        /// <param name="product">Product</param>
        /// <param name="user">User</param>
        /// <returns>Discounts</returns>
        protected virtual async Task<IList<Discount>> GetAllowedDiscountsAsync(Product product, User user)
        {
            var allowedDiscounts = new List<Discount>();
            if (_catalogSettings.IgnoreDiscounts)
                return allowedDiscounts;

            //discounts applied to products
            foreach (var discount in await GetAllowedDiscountsAppliedToProductAsync(product, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to categories
            foreach (var discount in await GetAllowedDiscountsAppliedToCategoriesAsync(product, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            //discounts applied to manufacturers
            foreach (var discount in await GetAllowedDiscountsAppliedToManufacturersAsync(product, user))
                if (!_discountService.ContainsDiscount(allowedDiscounts, discount))
                    allowedDiscounts.Add(discount);

            return allowedDiscounts;
        }

        /// <summary>
        /// Gets discount amount
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="productPriceWithoutDiscount">Already calculated product price without discount</param>
        /// <returns>Discount amount, Applied discounts</returns>
        protected virtual async Task<(decimal, List<Discount>)> GetDiscountAmountAsync(Product product,
            User user,
            decimal productPriceWithoutDiscount)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var appliedDiscounts = new List<Discount>();
            var appliedDiscountAmount = decimal.Zero;

            //we don't apply discounts to products with price entered by a user
            if (product.UserEntersPrice)
                return (appliedDiscountAmount, appliedDiscounts);

            //discounts are disabled
            if (_catalogSettings.IgnoreDiscounts)
                return (appliedDiscountAmount, appliedDiscounts);

            var allowedDiscounts = await GetAllowedDiscountsAsync(product, user);

            //no discounts
            if (!allowedDiscounts.Any())
                return (appliedDiscountAmount, appliedDiscounts);

            appliedDiscounts = _discountService.GetPreferredDiscount(allowedDiscounts, productPriceWithoutDiscount, out appliedDiscountAmount);

            return (appliedDiscountAmount, appliedDiscounts);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <returns>Final price, Applied discount amount, Applied discounts</returns>
        public virtual async Task<(decimal, decimal, List<Discount>)> GetFinalPriceAsync(Product product,
            User user,
            decimal additionalCharge = 0,
            bool includeDiscounts = true,
            int quantity = 1)
        {
            return await GetFinalPriceAsync(product, user,
                additionalCharge, includeDiscounts, quantity,
                null, null);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <returns>Final price, Applied discount amount, Applied discounts</returns>
        public virtual async Task<(decimal, decimal, List<Discount>)> GetFinalPriceAsync(Product product,
            User user,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            return await GetFinalPriceAsync(product, user, null, additionalCharge, includeDiscounts, quantity,
                rentalStartDate, rentalEndDate);
        }

        /// <summary>
        /// Gets the final price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="user">The user</param>
        /// <param name="overriddenProductPrice">Overridden product price. If specified, then it'll be used instead of a product price. For example, used with product attribute combinations</param>
        /// <param name="additionalCharge">Additional charge</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for final price computation</param>
        /// <param name="quantity">Shopping cart item quantity</param>
        /// <param name="rentalStartDate">Rental period start date (for rental products)</param>
        /// <param name="rentalEndDate">Rental period end date (for rental products)</param>
        /// <returns>Final price, Applied discount amount, Applied discounts</returns>
        public virtual async Task<(decimal, decimal, List<Discount>)> GetFinalPriceAsync(Product product,
            User user,
            decimal? overriddenProductPrice,
            decimal additionalCharge,
            bool includeDiscounts,
            int quantity,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.ProductPriceCacheKey,
                product,
                overriddenProductPrice,
                additionalCharge,
                includeDiscounts,
                quantity,
                await _userService.GetUserRoleIdsAsync(user),
                await _storeContext.GetCurrentStoreAsync());

            //we do not cache price if this not allowed by settings or if the product is rental product
            //otherwise, it can cause memory leaks (to store all possible date period combinations)
            if (!_catalogSettings.CacheProductPrices || product.IsRental)
                cacheKey.CacheTime = 0;

            decimal rezPrice;
            decimal discountAmount;
            List<Discount> appliedDiscounts;

            (rezPrice, discountAmount, appliedDiscounts) = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var discounts = new List<Discount>();
                var appliedDiscountAmount = decimal.Zero;

                //initial price
                var price = overriddenProductPrice ?? product.Price;

                //tier prices
                var tierPrice = await _productService.GetPreferredTierPriceAsync(product, user, (await _storeContext.GetCurrentStoreAsync()).Id, quantity);
                if (tierPrice != null)
                    price = tierPrice.Price;

                //additional charge
                price += additionalCharge;

                //rental products
                if (product.IsRental)
                    if (rentalStartDate.HasValue && rentalEndDate.HasValue)
                        price *= _productService.GetRentalPeriods(product, rentalStartDate.Value, rentalEndDate.Value);

                if (includeDiscounts)
                {
                    //discount
                    var (tmpDiscountAmount, tmpAppliedDiscounts) = await GetDiscountAmountAsync(product, user, price);
                    price -= tmpDiscountAmount;

                    if (tmpAppliedDiscounts?.Any() ?? false)
                    {
                        discounts.AddRange(tmpAppliedDiscounts);
                        appliedDiscountAmount = tmpDiscountAmount;
                    }
                }

                if (price < decimal.Zero)
                    price = decimal.Zero;

                return (price, appliedDiscountAmount, discounts);
            });

            return (rezPrice, discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Gets the product cost (one item)
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="attributesXml">Shopping cart item attributes in XML</param>
        /// <returns>Product cost (one item)</returns>
        public virtual async Task<decimal> GetProductCostAsync(Product product, string attributesXml)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var cost = product.ProductCost;
            var attributeValues = await _productAttributeParser.ParseProductAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                switch (attributeValue.AttributeValueType)
                {
                    case AttributeValueType.Simple:
                        //simple attribute
                        cost += attributeValue.Cost;
                        break;
                    case AttributeValueType.AssociatedToProduct:
                        //bundled product
                        var associatedProduct = await _productService.GetProductByIdAsync(attributeValue.AssociatedProductId);
                        if (associatedProduct != null)
                            cost += associatedProduct.ProductCost * attributeValue.Quantity;
                        break;
                    default:
                        break;
                }
            }

            return cost;
        }

        /// <summary>
        /// Get a price adjustment of a product attribute value
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="value">Product attribute value</param>
        /// <param name="user">User</param>
        /// <param name="productPrice">Product price (null for using the base product price)</param>
        /// <returns>Price adjustment</returns>
        public virtual async Task<decimal> GetProductAttributeValuePriceAdjustmentAsync(Product product, ProductAttributeValue value, User user, decimal? productPrice = null)
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
                        if (!productPrice.HasValue)
                            productPrice = (await GetFinalPriceAsync(product, user)).Item1;

                        adjustment = (decimal)((float)productPrice * (float)value.PriceAdjustment / 100f);
                    }
                    else
                    {
                        adjustment = value.PriceAdjustment;
                    }

                    break;
                case AttributeValueType.AssociatedToProduct:
                    //bundled product
                    var associatedProduct = await _productService.GetProductByIdAsync(value.AssociatedProductId);
                    if (associatedProduct != null)
                        adjustment = (await GetFinalPriceAsync(associatedProduct, user)).Item1 * value.Quantity;

                    break;
                default:
                    break;
            }

            return adjustment;
        }

        /// <summary>
        /// Round a product or order total for the currency
        /// </summary>
        /// <param name="value">Value to round</param>
        /// <param name="currency">Currency; pass null to use the primary store currency</param>
        /// <returns>Rounded value</returns>
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