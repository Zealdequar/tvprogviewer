using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Data;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Shopping cart service
    /// </summary>
    public partial class ShoppingCartService : IShoppingCartService
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IAclService _aclService;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly IRepository<ShoppingCartItem> _sciRepository;
        private readonly IShippingService _shippingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly OrderSettings _orderSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public ShoppingCartService(CatalogSettings catalogSettings,
            IAclService aclService,
            IActionContextAccessor actionContextAccessor,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICurrencyService currencyService,
            IUserService userService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            IRepository<ShoppingCartItem> sciRepository,
            IShippingService shippingService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            OrderSettings orderSettings,
            ShoppingCartSettings shoppingCartSettings)
        {
            _catalogSettings = catalogSettings;
            _aclService = aclService;
            _actionContextAccessor = actionContextAccessor;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _currencyService = currencyService;
            _userService = userService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _sciRepository = sciRepository;
            _shippingService = shippingService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeService = storeService;
            _storeMappingService = storeMappingService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _orderSettings = orderSettings;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Determine if the shopping cart item is the same as the one being compared
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart item is equal
        /// </returns>
        protected virtual async Task<bool> ShoppingCartItemIsEqualAsync(ShoppingCartItem shoppingCartItem,
            TvChannel tvChannel,
            string attributesXml,
            decimal userEnteredPrice,
            DateTime? rentalStartDate,
            DateTime? rentalEndDate)
        {
            if (shoppingCartItem.TvChannelId != tvChannel.Id)
                return false;

            //attributes
            var attributesEqual = await _tvChannelAttributeParser.AreTvChannelAttributesEqualAsync(shoppingCartItem.AttributesXml, attributesXml, false, false);
            if (!attributesEqual)
                return false;

            //gift cards
            if (tvChannel.IsGiftCard)
            {
                _tvChannelAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName1, out var _, out var giftCardSenderName1, out var _, out var _);

                _tvChannelAttributeParser.GetGiftCardAttribute(shoppingCartItem.AttributesXml, out var giftCardRecipientName2, out var _, out var giftCardSenderName2, out var _, out var _);

                var giftCardsAreEqual = giftCardRecipientName1.Equals(giftCardRecipientName2, StringComparison.InvariantCultureIgnoreCase)
                    && giftCardSenderName1.Equals(giftCardSenderName2, StringComparison.InvariantCultureIgnoreCase);
                if (!giftCardsAreEqual)
                    return false;
            }

            //price is the same (for tvChannels which require users to enter a price)
            if (tvChannel.UserEntersPrice)
            {
                //we use rounding to eliminate errors associated with storing real numbers in memory when comparing
                var userEnteredPricesEqual = Math.Round(shoppingCartItem.UserEnteredPrice, 2) == Math.Round(userEnteredPrice, 2);
                if (!userEnteredPricesEqual)
                    return false;
            }

            if (!tvChannel.IsRental)
                return true;

            //rental tvChannels
            var rentalInfoEqual = shoppingCartItem.RentalStartDateUtc == rentalStartDate && shoppingCartItem.RentalEndDateUtc == rentalEndDate;

            return rentalInfoEqual;
        }

        /// <summary>
        /// Gets a value indicating whether user shopping cart is empty
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Result</returns>
        protected virtual bool IsUserShoppingCartEmpty(User user)
        {
            return !_sciRepository.Table.Any(sci => sci.UserId == user.Id);
        }

        /// <summary>
        /// Validates required tvChannels (tvChannels which require some other tvChannels to be added to the cart)
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredTvChannels">Whether to add required tvChannels</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        protected virtual async Task<IList<string>> GetRequiredTvChannelWarningsAsync(User user, ShoppingCartType shoppingCartType, TvChannel tvChannel,
            int storeId, int quantity, bool addRequiredTvChannels, int shoppingCartItemId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            //at now we ignore quantities of required tvChannels and use 1
            var requiredTvChannelQuantity = 1;

            //get user shopping cart
            var cart = await GetShoppingCartAsync(user, shoppingCartType, storeId);

            var tvChannelsRequiringTvChannel = await GetTvChannelsRequiringTvChannelAsync(cart, tvChannel);

            //whether other cart items require the passed tvChannel
            var passedTvChannelRequiredQuantity = cart.Where(ci => tvChannelsRequiringTvChannel.Any(p => p.Id == ci.TvChannelId))
                .Sum(item => item.Quantity * requiredTvChannelQuantity);

            if (passedTvChannelRequiredQuantity > quantity)
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RequiredTvChannelUpdateWarning"), passedTvChannelRequiredQuantity));

            //whether the passed tvChannel requires other tvChannels
            if (!tvChannel.RequireOtherTvChannels)
                return warnings;

            //get these required tvChannels
            var requiredTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(_tvChannelService.ParseRequiredTvChannelIds(tvChannel));
            if (!requiredTvChannels.Any())
                return warnings;

            //get warnings
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            var warningLocale = await _localizationService.GetResourceAsync("ShoppingCart.RequiredTvChannelWarning");
            foreach (var requiredTvChannel in requiredTvChannels)
            {
                var tvChannelsRequiringRequiredTvChannel = await GetTvChannelsRequiringTvChannelAsync(cart, requiredTvChannel);

                //get the required quantity of the required tvChannel
                var requiredTvChannelRequiredQuantity = quantity * requiredTvChannelQuantity +
                    cart.Where(ci => tvChannelsRequiringRequiredTvChannel.Any(p => p.Id == ci.TvChannelId))
                        .Where(item => item.Id != shoppingCartItemId)
                        .Sum(item => item.Quantity * requiredTvChannelQuantity);

                //whether required tvChannel is already in the cart in the required quantity
                var quantityToAdd = requiredTvChannelRequiredQuantity - (cart.FirstOrDefault(item => item.TvChannelId == requiredTvChannel.Id)?.Quantity ?? 0);
                if (quantityToAdd <= 0)
                    continue;

                //prepare warning message
                var url = urlHelper.RouteUrl(nameof(TvChannel), new { SeName = await _urlRecordService.GetSeNameAsync(requiredTvChannel) });
                var requiredTvChannelName = WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(requiredTvChannel, x => x.Name));
                var requiredTvChannelWarning = _catalogSettings.UseLinksInRequiredTvChannelWarnings
                    ? string.Format(warningLocale, $"<a href=\"{url}\">{requiredTvChannelName}</a>", requiredTvChannelRequiredQuantity)
                    : string.Format(warningLocale, requiredTvChannelName, requiredTvChannelRequiredQuantity);

                //add to cart (if possible)
                if (addRequiredTvChannels && tvChannel.AutomaticallyAddRequiredTvChannels)
                {
                    //do not add required tvChannels to prevent circular references
                    var addToCartWarnings = await GetShoppingCartItemWarningsAsync(
                        user: user,
                        tvChannel: requiredTvChannel,
                        attributesXml: null,
                        userEnteredPrice: decimal.Zero,
                        shoppingCartType: shoppingCartType,
                        storeId: storeId,
                        quantity: quantityToAdd,
                        addRequiredTvChannels: true);

                    //don't display all specific errors only the generic one
                    if (addToCartWarnings.Any())
                        warnings.Add(requiredTvChannelWarning);
                }
                else
                {
                    warnings.Add(requiredTvChannelWarning);
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates a tvChannel for standard properties
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <param name="storeId">Store identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        protected virtual async Task<IList<string>> GetStandardWarningsAsync(User user, ShoppingCartType shoppingCartType, TvChannel tvChannel,
            string attributesXml, decimal userEnteredPrice, int quantity, int shoppingCartItemId, int storeId)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            //deleted
            if (tvChannel.Deleted)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.TvChannelDeleted"));
                return warnings;
            }

            //published
            if (!tvChannel.Published)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.TvChannelUnpublished"));
            }

            //we can add only simple tvChannels
            if (tvChannel.TvChannelType != TvChannelType.SimpleTvChannel)
            {
                warnings.Add("This is not simple tvChannel");
            }

            //ACL
            if (!await _aclService.AuthorizeAsync(tvChannel, user))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.TvChannelUnpublished"));
            }

            //Store mapping
            if (!await _storeMappingService.AuthorizeAsync(tvChannel, storeId))
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.TvChannelUnpublished"));
            }

            //disabled "add to cart" button
            if (shoppingCartType == ShoppingCartType.ShoppingCart && tvChannel.DisableBuyButton)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.BuyingDisabled"));
            }

            //disabled "add to wishlist" button
            if (shoppingCartType == ShoppingCartType.Wishlist && tvChannel.DisableWishlistButton)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.WishlistDisabled"));
            }

            //call for price
            if (shoppingCartType == ShoppingCartType.ShoppingCart && tvChannel.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
            {
                warnings.Add(await _localizationService.GetResourceAsync("TvChannels.CallForPrice"));
            }

            //user entered price
            if (tvChannel.UserEntersPrice)
            {
                if (userEnteredPrice < tvChannel.MinimumUserEnteredPrice ||
                    userEnteredPrice > tvChannel.MaximumUserEnteredPrice)
                {
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var minimumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvChannel.MinimumUserEnteredPrice, currentCurrency);
                    var maximumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvChannel.MaximumUserEnteredPrice, currentCurrency);
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.UserEnteredPrice.RangeError"),
                        await _priceFormatter.FormatPriceAsync(minimumUserEnteredPrice, false, false),
                        await _priceFormatter.FormatPriceAsync(maximumUserEnteredPrice, false, false)));
                }
            }

            //quantity validation
            var hasQtyWarnings = false;
            if (quantity < tvChannel.OrderMinimumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MinimumQuantity"), tvChannel.OrderMinimumQuantity));
                hasQtyWarnings = true;
            }

            if (quantity > tvChannel.OrderMaximumQuantity)
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumQuantity"), tvChannel.OrderMaximumQuantity));
                hasQtyWarnings = true;
            }

            var allowedQuantities = _tvChannelService.ParseAllowedQuantities(tvChannel);
            if (allowedQuantities.Length > 0 && !allowedQuantities.Contains(quantity))
            {
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AllowedQuantities"), string.Join(", ", allowedQuantities)));
            }

            var validateOutOfStock = shoppingCartType == ShoppingCartType.ShoppingCart || !_shoppingCartSettings.AllowOutOfStockItemsToBeAddedToWishlist;
            if (validateOutOfStock && !hasQtyWarnings)
            {
                switch (tvChannel.ManageInventoryMethod)
                {
                    case ManageInventoryMethod.DontManageStock:
                        //do nothing
                        break;
                    case ManageInventoryMethod.ManageStock:
                        if (tvChannel.BackorderMode == BackorderMode.NoBackorders)
                        {
                            var maximumQuantityCanBeAdded = await _tvChannelService.GetTotalStockQuantityAsync(tvChannel);

                            warnings.AddRange(await GetQuantityTvChannelWarningsAsync(tvChannel, quantity, maximumQuantityCanBeAdded));

                            if (warnings.Any())
                                return warnings;

                            //validate tvChannel quantity with non combinable tvChannel attributes
                            var tvChannelAttributeMappings = await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
                            if (tvChannelAttributeMappings?.Any() == true)
                            {
                                var onlyCombinableAttributes = tvChannelAttributeMappings.All(mapping => !mapping.IsNonCombinable());
                                if (!onlyCombinableAttributes)
                                {
                                    var cart = await GetShoppingCartAsync(user, shoppingCartType, storeId);
                                    var totalAddedQuantity = cart
                                        .Where(item => item.TvChannelId == tvChannel.Id && item.Id != shoppingCartItemId)
                                        .Sum(tvChannel => tvChannel.Quantity);

                                    totalAddedQuantity += quantity;

                                    //counting a tvChannel into bundles
                                    foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                    {
                                        var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(bundle.AttributesXml);
                                        foreach (var attributeValue in attributeValues)
                                        {
                                            if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToTvChannel && attributeValue.AssociatedTvChannelId == tvChannel.Id)
                                                totalAddedQuantity += bundle.Quantity * attributeValue.Quantity;
                                        }
                                    }

                                    warnings.AddRange(await GetQuantityTvChannelWarningsAsync(tvChannel, totalAddedQuantity, maximumQuantityCanBeAdded));
                                }
                            }

                            if (warnings.Any())
                                return warnings;

                            //validate tvChannel quantity and tvChannel quantity into bundles
                            if (string.IsNullOrEmpty(attributesXml))
                            {
                                var cart = await GetShoppingCartAsync(user, shoppingCartType, storeId);
                                var totalQuantityInCart = cart.Where(item => item.TvChannelId == tvChannel.Id && item.Id != shoppingCartItemId && string.IsNullOrEmpty(item.AttributesXml))
                                    .Sum(tvChannel => tvChannel.Quantity);

                                totalQuantityInCart += quantity;

                                foreach (var bundle in cart.Where(x => x.Id != shoppingCartItemId && !string.IsNullOrEmpty(x.AttributesXml)))
                                {
                                    var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(bundle.AttributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        if (attributeValue.AttributeValueType == AttributeValueType.AssociatedToTvChannel && attributeValue.AssociatedTvChannelId == tvChannel.Id)
                                            totalQuantityInCart += bundle.Quantity * attributeValue.Quantity;
                                    }
                                }

                                warnings.AddRange(await GetQuantityTvChannelWarningsAsync(tvChannel, totalQuantityInCart, maximumQuantityCanBeAdded));
                            }
                        }

                        break;
                    case ManageInventoryMethod.ManageStockByAttributes:
                        var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
                        if (combination != null)
                        {
                            //combination exists
                            //let's check stock level
                            if (!combination.AllowOutOfStockOrders)
                                warnings.AddRange(await GetQuantityTvChannelWarningsAsync(tvChannel, quantity, combination.StockQuantity));
                        }
                        else
                        {
                            //combination doesn't exist
                            if (tvChannel.AllowAddingOnlyExistingAttributeCombinations)
                            {
                                //maybe, is it better  to display something like "No such tvChannel/combination" message?
                                var tvChannelAvailabilityRange = await _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvChannel.TvChannelAvailabilityRangeId);
                                var warning = tvChannelAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                                        await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
                                warnings.Add(warning);
                            }
                        }

                        break;
                    default:
                        break;
                }
            }

            //availability dates
            var availableStartDateError = false;
            if (tvChannel.AvailableStartDateTimeUtc.HasValue)
            {
                var availableStartDateTime = DateTime.SpecifyKind(tvChannel.AvailableStartDateTimeUtc.Value, DateTimeKind.Utc);
                if (availableStartDateTime.CompareTo(DateTime.UtcNow) > 0)
                {
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
                    availableStartDateError = true;
                }
            }

            if (!tvChannel.AvailableEndDateTimeUtc.HasValue || availableStartDateError)
                return warnings;

            var availableEndDateTime = DateTime.SpecifyKind(tvChannel.AvailableEndDateTimeUtc.Value, DateTimeKind.Utc);
            if (availableEndDateTime.CompareTo(DateTime.UtcNow) < 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.NotAvailable"));
            }

            return warnings;
        }

        /// <summary>
        /// Validates the maximum quantity a tvChannel can be added 
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="maximumQuantityCanBeAdded">The maximum quantity a tvChannel can be added</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings 
        /// </returns>
        protected virtual async Task<IList<string>> GetQuantityTvChannelWarningsAsync(TvChannel tvChannel, int quantity, int maximumQuantityCanBeAdded)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            if (maximumQuantityCanBeAdded < quantity)
            {
                if (maximumQuantityCanBeAdded <= 0)
                {
                    var tvChannelAvailabilityRange = await _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvChannel.TvChannelAvailabilityRangeId);
                    var warning = tvChannelAvailabilityRange == null ? await _localizationService.GetResourceAsync("ShoppingCart.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
                    warnings.Add(warning);
                }
                else
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.QuantityExceedsStock"), maximumQuantityCanBeAdded));
            }

            return warnings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteShoppingCartItemAsync(ShoppingCartItem shoppingCartItem, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            var user = await _userService.GetUserByIdAsync(shoppingCartItem.UserId);
            var storeId = shoppingCartItem.StoreId;

            //reset checkout data
            if (resetCheckoutData)
                await _userService.ResetCheckoutDataAsync(user, shoppingCartItem.StoreId);

            //delete item
            await _sciRepository.DeleteAsync(shoppingCartItem);

            //reset "HasShoppingCartItems" property used for performance optimization
            user.HasShoppingCartItems = !IsUserShoppingCartEmpty(user);
            await _userService.UpdateUserAsync(user);

            //validate checkout attributes
            if (ensureOnlyActiveCheckoutAttributes &&
                //only for shopping cart items (ignore wishlist)
                shoppingCartItem.ShoppingCartType == ShoppingCartType.ShoppingCart)
            {
                var cart = await GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, storeId);

                var checkoutAttributesXml =
                    await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.CheckoutAttributes,
                        storeId);
                checkoutAttributesXml =
                    await _checkoutAttributeParser.EnsureOnlyActiveAttributesAsync(checkoutAttributesXml, cart);
                await _genericAttributeService.SaveAttributeAsync(user, TvProgUserDefaults.CheckoutAttributes,
                    checkoutAttributesXml, storeId);
            }

            if (!_catalogSettings.RemoveRequiredTvChannels)
                return;

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(shoppingCartItem.TvChannelId);
            if (!tvChannel?.RequireOtherTvChannels ?? true)
                return;

            var requiredTvChannelIds = _tvChannelService.ParseRequiredTvChannelIds(tvChannel);
            var requiredShoppingCartItems =
                (await GetShoppingCartAsync(user, shoppingCartType: shoppingCartItem.ShoppingCartType))
                    .Where(item => requiredTvChannelIds.Any(id => id == item.TvChannelId))
                    .ToList();

            //update quantity of required tvChannels in the cart if the main one is removed
            foreach (var cartItem in requiredShoppingCartItems)
            {
                //at now we ignore quantities of required tvChannels and use 1
                var requiredTvChannelQuantity = 1;

                await UpdateShoppingCartItemAsync(user, cartItem.Id, cartItem.AttributesXml, cartItem.UserEnteredPrice,
                    quantity: cartItem.Quantity - shoppingCartItem.Quantity * requiredTvChannelQuantity,
                    resetCheckoutData: false);
            }
        }

        /// <summary>
        /// Delete shopping cart item
        /// </summary>
        /// <param name="shoppingCartItemId">Shopping cart item ID</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <param name="ensureOnlyActiveCheckoutAttributes">A value indicating whether to ensure that only active checkout attributes are attached to the current user</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteShoppingCartItemAsync(int shoppingCartItemId, bool resetCheckoutData = true,
            bool ensureOnlyActiveCheckoutAttributes = false)
        {
            var shoppingCartItem = await _sciRepository.Table.FirstOrDefaultAsync(sci => sci.Id == shoppingCartItemId);
            if (shoppingCartItem != null)
                await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, ensureOnlyActiveCheckoutAttributes);
        }

        /// <summary>
        /// Deletes expired shopping cart items
        /// </summary>
        /// <param name="olderThanUtc">Older than date and time</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of deleted items
        /// </returns>
        public virtual async Task<int> DeleteExpiredShoppingCartItemsAsync(DateTime olderThanUtc)
        {
            var query = from sci in _sciRepository.Table
                        where sci.UpdatedOnUtc < olderThanUtc
                        select sci;

            var cartItems = await query.ToListAsync();

            foreach (var cartItem in cartItems)
                await DeleteShoppingCartItemAsync(cartItem);

            return cartItems.Count;
        }

        /// <summary>
        /// Get tvChannels from shopping cart whether requiring specific tvChannel
        /// </summary>
        /// <param name="cart">Shopping cart </param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetTvChannelsRequiringTvChannelAsync(IList<ShoppingCartItem> cart, TvChannel tvChannel)
        {
            if (cart is null)
                throw new ArgumentNullException(nameof(cart));

            if (tvChannel is null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (cart.Count == 0)
                return new List<TvChannel>();

            var tvChannelIds = cart.Select(ci => ci.TvChannelId).ToArray();

            var cartTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelIds);

            return cartTvChannels.Where(cartTvChannel =>
                cartTvChannel.RequireOtherTvChannels &&
                _tvChannelService.ParseRequiredTvChannelIds(cartTvChannel).Contains(tvChannel.Id)).ToList();
        }

        /// <summary>
        /// Gets shopping cart
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type; pass null to load all records</param>
        /// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <param name="tvChannelId">TvChannel identifier; pass null to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); pass null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); pass null to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping Cart
        /// </returns>
        public virtual async Task<IList<ShoppingCartItem>> GetShoppingCartAsync(User user, ShoppingCartType? shoppingCartType = null,
            int storeId = 0, int? tvChannelId = null, DateTime? createdFromUtc = null, DateTime? createdToUtc = null)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var items = _sciRepository.Table.Where(sci => sci.UserId == user.Id);

            //filter by type
            if (shoppingCartType.HasValue)
                items = items.Where(item => item.ShoppingCartTypeId == (int)shoppingCartType.Value);

            //filter shopping cart items by store
            if (storeId > 0 && !_shoppingCartSettings.CartsSharedBetweenStores)
                items = items.Where(item => item.StoreId == storeId);

            //filter shopping cart items by tvChannel
            if (tvChannelId > 0)
                items = items.Where(item => item.TvChannelId == tvChannelId);

            //filter shopping cart items by date
            if (createdFromUtc.HasValue)
                items = items.Where(item => createdFromUtc.Value <= item.CreatedOnUtc);
            if (createdToUtc.HasValue)
                items = items.Where(item => createdToUtc.Value >= item.CreatedOnUtc);

            var key = _staticCacheManager.PrepareKeyForShortTermCache(TvProgOrderDefaults.ShoppingCartItemsAllCacheKey, user, shoppingCartType, storeId, tvChannelId, createdFromUtc, createdToUtc);

            return await _staticCacheManager.GetAsync(key, async () => await items.ToListAsync());
        }

        /// <summary>
        /// Validates shopping cart item attributes
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="ignoreNonCombinableAttributes">A value indicating whether we should ignore non-combinable attributes</param>
        /// <param name="ignoreConditionMet">A value indicating whether we should ignore filtering by "is condition met" property</param>
        /// <param name="ignoreBundledTvChannels">A value indicating whether we should ignore bundled (associated) tvChannels</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetShoppingCartItemAttributeWarningsAsync(User user,
            ShoppingCartType shoppingCartType,
            TvChannel tvChannel,
            int quantity = 1,
            string attributesXml = "",
            bool ignoreNonCombinableAttributes = false,
            bool ignoreConditionMet = false,
            bool ignoreBundledTvChannels = false,
            int shoppingCartItemId = 0)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            //ensure it's our attributes
            var attributes1 = await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(attributesXml);
            if (ignoreNonCombinableAttributes)
            {
                attributes1 = attributes1.Where(x => !x.IsNonCombinable()).ToList();
            }

            foreach (var attribute in attributes1)
            {
                if (attribute.TvChannelId == 0)
                {
                    warnings.Add("Attribute error");
                    return warnings;
                }

                if (attribute.TvChannelId != tvChannel.Id)
                {
                    warnings.Add("Attribute error");
                }
            }

            //validate required tvChannel attributes (whether they're chosen/selected/entered)
            var attributes2 = await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
            if (ignoreNonCombinableAttributes)
            {
                attributes2 = attributes2.Where(x => !x.IsNonCombinable()).ToList();
            }

            //validate conditional attributes only (if specified)
            if (!ignoreConditionMet)
            {
                attributes2 = await attributes2.WhereAwait(async x =>
                {
                    var conditionMet = await _tvChannelAttributeParser.IsConditionMetAsync(x, attributesXml);
                    return !conditionMet.HasValue || conditionMet.Value;
                }).ToListAsync();
            }

            foreach (var a2 in attributes2)
            {
                if (a2.IsRequired)
                {
                    var found = false;
                    //selected tvChannel attributes
                    foreach (var a1 in attributes1)
                    {
                        if (a1.Id != a2.Id)
                            continue;

                        var attributeValuesStr = _tvChannelAttributeParser.ParseValues(attributesXml, a1.Id);

                        foreach (var str1 in attributeValuesStr)
                        {
                            if (string.IsNullOrEmpty(str1.Trim()))
                                continue;

                            found = true;
                            break;
                        }
                    }

                    //if not found
                    if (!found)
                    {
                        var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(a2.TvChannelAttributeId);

                        var textPrompt = await _localizationService.GetLocalizedAsync(a2, x => x.TextPrompt);
                        var notFoundWarning = !string.IsNullOrEmpty(textPrompt) ?
                            textPrompt :
                            string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"), await _localizationService.GetLocalizedAsync(tvChannelAttribute, a => a.Name));

                        warnings.Add(notFoundWarning);
                    }
                }

                if (a2.AttributeControlType != AttributeControlType.ReadonlyCheckboxes)
                    continue;

                //users cannot edit read-only attributes
                var allowedReadOnlyValueIds = (await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(a2.Id))
                    .Where(x => x.IsPreSelected)
                    .Select(x => x.Id)
                    .ToArray();

                var selectedReadOnlyValueIds = (await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml))
                    .Where(x => x.TvChannelAttributeMappingId == a2.Id)
                    .Select(x => x.Id)
                    .ToArray();

                if (!CommonHelper.ArraysEqual(allowedReadOnlyValueIds, selectedReadOnlyValueIds))
                {
                    warnings.Add("You cannot change read-only values");
                }
            }

            //validation rules
            foreach (var pam in attributes2)
            {
                if (!pam.ValidationRulesAllowed())
                    continue;

                string enteredText;
                int enteredTextLength;

                var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(pam.TvChannelAttributeId);

                //minimum length
                if (pam.ValidationMinLength.HasValue)
                {
                    if (pam.AttributeControlType == AttributeControlType.TextBox ||
                        pam.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        enteredText = _tvChannelAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
                        enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (pam.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(tvChannelAttribute, a => a.Name), pam.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (!pam.ValidationMaxLength.HasValue)
                    continue;

                if (pam.AttributeControlType != AttributeControlType.TextBox && pam.AttributeControlType != AttributeControlType.MultilineTextbox)
                    continue;

                enteredText = _tvChannelAttributeParser.ParseValues(attributesXml, pam.Id).FirstOrDefault();
                enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                if (pam.ValidationMaxLength.Value < enteredTextLength)
                {
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(tvChannelAttribute, a => a.Name), pam.ValidationMaxLength.Value));
                }
            }

            if (warnings.Any() || ignoreBundledTvChannels)
                return warnings;

            //validate bundled tvChannels
            var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToTvChannel)
                    continue;

                var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingByIdAsync(attributeValue.TvChannelAttributeMappingId);

                if (ignoreNonCombinableAttributes && tvChannelAttributeMapping != null && tvChannelAttributeMapping.IsNonCombinable())
                    continue;

                //associated tvChannel (bundle)
                var associatedTvChannel = await _tvChannelService.GetTvChannelByIdAsync(attributeValue.AssociatedTvChannelId);
                if (associatedTvChannel != null)
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var totalQty = quantity * attributeValue.Quantity;
                    var associatedTvChannelWarnings = await GetShoppingCartItemWarningsAsync(user,
                        shoppingCartType, associatedTvChannel, store.Id,
                        string.Empty, decimal.Zero, null, null, totalQty, false, shoppingCartItemId);

                    var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeMapping.TvChannelAttributeId);

                    foreach (var associatedTvChannelWarning in associatedTvChannelWarnings)
                    {
                        var attributeName = await _localizationService.GetLocalizedAsync(tvChannelAttribute, a => a.Name);
                        var attributeValueName = await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name);
                        warnings.Add(string.Format(
                            await _localizationService.GetResourceAsync("ShoppingCart.AssociatedAttributeWarning"),
                            attributeName, attributeValueName, associatedTvChannelWarning));
                    }
                }
                else
                {
                    warnings.Add($"Associated tvChannel cannot be loaded - {attributeValue.AssociatedTvChannelId}");
                }
            }

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item (gift card)
        /// </summary>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetShoppingCartItemGiftCardWarningsAsync(ShoppingCartType shoppingCartType,
            TvChannel tvChannel, string attributesXml)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            //gift cards
            if (!tvChannel.IsGiftCard)
                return warnings;

            _tvChannelAttributeParser.GetGiftCardAttribute(attributesXml, out var giftCardRecipientName, out var giftCardRecipientEmail, out var giftCardSenderName, out var giftCardSenderEmail, out var _);

            if (string.IsNullOrEmpty(giftCardRecipientName))
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientNameError"));

            if (tvChannel.GiftCardType == GiftCardType.Virtual)
            {
                //validate for virtual gift cards only
                if (string.IsNullOrEmpty(giftCardRecipientEmail) || !CommonHelper.IsValidEmail(giftCardRecipientEmail))
                    warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.RecipientEmailError"));
            }

            if (string.IsNullOrEmpty(giftCardSenderName))
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderNameError"));

            if (tvChannel.GiftCardType != GiftCardType.Virtual)
                return warnings;

            //validate for virtual gift cards only
            if (string.IsNullOrEmpty(giftCardSenderEmail) || !CommonHelper.IsValidEmail(giftCardSenderEmail))
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.SenderEmailError"));

            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item for rental tvChannels
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetRentalTvChannelWarningsAsync(TvChannel tvChannel,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            if (!tvChannel.IsRental)
                return warnings;

            if (!rentalStartDate.HasValue)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterStartDate"));
                return warnings;
            }

            if (!rentalEndDate.HasValue)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.EnterEndDate"));
                return warnings;
            }

            if (rentalStartDate.Value.CompareTo(rentalEndDate.Value) > 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateLessEndDate"));
                return warnings;
            }

            //allowed start date should be the future date
            //we should compare rental start date with a store local time
            //but we what if a store works in distinct timezones? how we should handle it? skip it for now
            //we also ignore hours (anyway not supported yet)
            //today
            var nowDtInStoreTimeZone = _dateTimeHelper.ConvertToUserTime(DateTime.Now, TimeZoneInfo.Local, _dateTimeHelper.DefaultStoreTimeZone);
            var todayDt = new DateTime(nowDtInStoreTimeZone.Year, nowDtInStoreTimeZone.Month, nowDtInStoreTimeZone.Day);
            var todayDtUtc = _dateTimeHelper.ConvertToUtcTime(todayDt, _dateTimeHelper.DefaultStoreTimeZone);
            //dates are entered in store timezone (e.g. like in hotels)
            var startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.DefaultStoreTimeZone);
            //but we what if dates should be entered in a user timezone?
            //DateTime startDateUtc = _dateTimeHelper.ConvertToUtcTime(rentalStartDate.Value, _dateTimeHelper.CurrentTimeZone);
            if (todayDtUtc.CompareTo(startDateUtc) <= 0)
                return warnings;

            warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.Rental.StartDateShouldBeFuture"));
            return warnings;
        }

        /// <summary>
        /// Validates shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">User entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredTvChannels">Whether to add required tvChannels</param>
        /// <param name="shoppingCartItemId">Shopping cart identifier; pass 0 if it's a new item</param>
        /// <param name="getStandardWarnings">A value indicating whether we should validate a tvChannel for standard properties</param>
        /// <param name="getAttributesWarnings">A value indicating whether we should validate tvChannel attributes</param>
        /// <param name="getGiftCardWarnings">A value indicating whether we should validate gift card properties</param>
        /// <param name="getRequiredTvChannelWarnings">A value indicating whether we should validate required tvChannels (tvChannels which require other tvChannels to be added to the cart)</param>
        /// <param name="getRentalWarnings">A value indicating whether we should validate rental properties</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetShoppingCartItemWarningsAsync(User user, ShoppingCartType shoppingCartType,
            TvChannel tvChannel, int storeId,
            string attributesXml, decimal userEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredTvChannels = true, int shoppingCartItemId = 0,
            bool getStandardWarnings = true, bool getAttributesWarnings = true,
            bool getGiftCardWarnings = true, bool getRequiredTvChannelWarnings = true,
            bool getRentalWarnings = true)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();

            //standard properties
            if (getStandardWarnings)
                warnings.AddRange(await GetStandardWarningsAsync(user, shoppingCartType, tvChannel, attributesXml, userEnteredPrice, quantity, shoppingCartItemId, storeId));

            //selected attributes
            if (getAttributesWarnings)
                warnings.AddRange(await GetShoppingCartItemAttributeWarningsAsync(user, shoppingCartType, tvChannel, quantity, attributesXml, false, false, false, shoppingCartItemId));

            //gift cards
            if (getGiftCardWarnings)
                warnings.AddRange(await GetShoppingCartItemGiftCardWarningsAsync(shoppingCartType, tvChannel, attributesXml));

            //required tvChannels
            if (getRequiredTvChannelWarnings)
                warnings.AddRange(await GetRequiredTvChannelWarningsAsync(user, shoppingCartType, tvChannel, storeId, quantity, addRequiredTvChannels, shoppingCartItemId));

            //rental tvChannels
            if (getRentalWarnings)
                warnings.AddRange(await GetRentalTvChannelWarningsAsync(tvChannel, rentalStartDate, rentalEndDate));

            return warnings;
        }

        /// <summary>
        /// Validates whether this shopping cart is valid
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="checkoutAttributesXml">Checkout attributes in XML format</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether to validate checkout attributes</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> GetShoppingCartWarningsAsync(IList<ShoppingCartItem> shoppingCart,
            string checkoutAttributesXml, bool validateCheckoutAttributes)
        {
            var warnings = new List<string>();

            if (shoppingCart.Count > _shoppingCartSettings.MaximumShoppingCartItems)
                warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));

            var hasStandartTvChannels = false;
            var hasRecurringTvChannels = false;

            foreach (var sci in shoppingCart)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);
                if (tvChannel == null)
                {
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.CannotLoadTvChannel"), sci.TvChannelId));
                    return warnings;
                }

                if (tvChannel.IsRecurring)
                    hasRecurringTvChannels = true;
                else
                    hasStandartTvChannels = true;
            }

            //don't mix standard and recurring tvChannels
            if (hasStandartTvChannels && hasRecurringTvChannels)
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.CannotMixStandardAndAutoshipTvChannels"));

            //recurring cart validation
            if (hasRecurringTvChannels)
            {
                var cyclesError = (await GetRecurringCycleInfoAsync(shoppingCart)).error;
                if (!string.IsNullOrEmpty(cyclesError))
                {
                    warnings.Add(cyclesError);
                    return warnings;
                }
            }

            //validate checkout attributes
            if (!validateCheckoutAttributes)
                return warnings;

            //selected attributes
            var attributes1 = await _checkoutAttributeParser.ParseCheckoutAttributesAsync(checkoutAttributesXml);

            //existing checkout attributes
            var excludeShippableAttributes = !await ShoppingCartRequiresShippingAsync(shoppingCart);
            var store = await _storeContext.GetCurrentStoreAsync();
            var attributes2 = await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id, excludeShippableAttributes);

            //validate conditional attributes only (if specified)
            attributes2 = await attributes2.WhereAwait(async x =>
            {
                var conditionMet = await _checkoutAttributeParser.IsConditionMetAsync(x, checkoutAttributesXml);
                return !conditionMet.HasValue || conditionMet.Value;
            }).ToListAsync();

            foreach (var a2 in attributes2)
            {
                if (!a2.IsRequired)
                    continue;

                var found = false;
                //selected checkout attributes
                foreach (var a1 in attributes1)
                {
                    if (a1.Id != a2.Id)
                        continue;

                    var attributeValuesStr = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, a1.Id);
                    foreach (var str1 in attributeValuesStr)
                        if (!string.IsNullOrEmpty(str1.Trim()))
                        {
                            found = true;
                            break;
                        }
                }

                if (found)
                    continue;

                //if not found
                warnings.Add(!string.IsNullOrEmpty(await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt))
                    ? await _localizationService.GetLocalizedAsync(a2, a => a.TextPrompt)
                    : string.Format(await _localizationService.GetResourceAsync("ShoppingCart.SelectAttribute"),
                        await _localizationService.GetLocalizedAsync(a2, a => a.Name)));
            }

            //now validation rules

            //minimum length
            foreach (var ca in attributes2)
            {
                string enteredText;
                int enteredTextLength;

                if (ca.ValidationMinLength.HasValue)
                {
                    if (ca.AttributeControlType == AttributeControlType.TextBox ||
                        ca.AttributeControlType == AttributeControlType.MultilineTextbox)
                    {
                        enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                        enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                        if (ca.ValidationMinLength.Value > enteredTextLength)
                        {
                            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMinimumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMinLength.Value));
                        }
                    }
                }

                //maximum length
                if (!ca.ValidationMaxLength.HasValue)
                    continue;

                if (ca.AttributeControlType != AttributeControlType.TextBox && ca.AttributeControlType != AttributeControlType.MultilineTextbox)
                    continue;

                enteredText = _checkoutAttributeParser.ParseValues(checkoutAttributesXml, ca.Id).FirstOrDefault();
                enteredTextLength = string.IsNullOrEmpty(enteredText) ? 0 : enteredText.Length;

                if (ca.ValidationMaxLength.Value < enteredTextLength)
                {
                    warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.TextboxMaximumLength"), await _localizationService.GetLocalizedAsync(ca, a => a.Name), ca.ValidationMaxLength.Value));
                }
            }

            return warnings;
        }

        /// <summary>
        /// Gets the shopping cart item sub total
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>Shopping cart item sub total. Applied discount amount. Applied discounts. Maximum discounted qty. Return not nullable value if discount cannot be applied to ALL items</returns>
        public virtual async Task<(decimal subTotal, decimal discountAmount, List<Discount> appliedDiscounts, int? maximumDiscountQty)> GetSubTotalAsync(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            decimal subTotal;
            int? maximumDiscountQty = null;

            //unit price
            var (unitPrice, discountAmount, appliedDiscounts) = await GetUnitPriceAsync(shoppingCartItem, includeDiscounts);

            //discount
            if (appliedDiscounts.Any())
            {
                //we can properly use "MaximumDiscountedQuantity" property only for one discount (not cumulative ones)
                Discount oneAndOnlyDiscount = null;
                if (appliedDiscounts.Count == 1)
                    oneAndOnlyDiscount = appliedDiscounts.First();

                if ((oneAndOnlyDiscount?.MaximumDiscountedQuantity.HasValue ?? false) &&
                    shoppingCartItem.Quantity > oneAndOnlyDiscount.MaximumDiscountedQuantity.Value)
                {
                    maximumDiscountQty = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                    //we cannot apply discount for all shopping cart items
                    var discountedQuantity = oneAndOnlyDiscount.MaximumDiscountedQuantity.Value;
                    var discountedSubTotal = unitPrice * discountedQuantity;
                    discountAmount *= discountedQuantity;

                    var notDiscountedQuantity = shoppingCartItem.Quantity - discountedQuantity;
                    var notDiscountedUnitPrice = (await GetUnitPriceAsync(shoppingCartItem, false)).unitPrice;
                    var notDiscountedSubTotal = notDiscountedUnitPrice * notDiscountedQuantity;

                    subTotal = discountedSubTotal + notDiscountedSubTotal;
                }
                else
                {
                    //discount is applied to all items (quantity)
                    //calculate discount amount for all items
                    discountAmount *= shoppingCartItem.Quantity;

                    subTotal = unitPrice * shoppingCartItem.Quantity;
                }
            }
            else
            {
                subTotal = unitPrice * shoppingCartItem.Quantity;
            }

            return (subTotal, discountAmount, appliedDiscounts, maximumDiscountQty);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="shoppingCartItem">The shopping cart item</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
        /// </returns>
        public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(ShoppingCartItem shoppingCartItem,
            bool includeDiscounts)
        {
            if (shoppingCartItem == null)
                throw new ArgumentNullException(nameof(shoppingCartItem));

            var user = await _userService.GetUserByIdAsync(shoppingCartItem.UserId);
            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(shoppingCartItem.TvChannelId);
            var store = await _storeService.GetStoreByIdAsync(shoppingCartItem.StoreId);

            return await GetUnitPriceAsync(tvChannel,
                user,
                store,
                shoppingCartItem.ShoppingCartType,
                shoppingCartItem.Quantity,
                shoppingCartItem.AttributesXml,
                shoppingCartItem.UserEnteredPrice,
                shoppingCartItem.RentalStartDateUtc,
                shoppingCartItem.RentalEndDateUtc,
                includeDiscounts);
        }

        /// <summary>
        /// Gets the shopping cart unit price (one item)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="attributesXml">TvChannel attributes (XML format)</param>
        /// <param name="userEnteredPrice">User entered price (if specified)</param>
        /// <param name="rentalStartDate">Rental start date (null for not rental tvChannels)</param>
        /// <param name="rentalEndDate">Rental end date (null for not rental tvChannels)</param>
        /// <param name="includeDiscounts">A value indicating whether include discounts or not for price computation</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart unit price (one item). Applied discount amount. Applied discounts
        /// </returns>
        public virtual async Task<(decimal unitPrice, decimal discountAmount, List<Discount> appliedDiscounts)> GetUnitPriceAsync(TvChannel tvChannel,
            User user,
            Store store,
            ShoppingCartType shoppingCartType,
            int quantity,
            string attributesXml,
            decimal userEnteredPrice,
            DateTime? rentalStartDate, DateTime? rentalEndDate,
            bool includeDiscounts)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var discountAmount = decimal.Zero;
            var appliedDiscounts = new List<Discount>();

            decimal finalPrice;

            var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
            if (combination?.OverriddenPrice.HasValue ?? false)
            {
                (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(tvChannel,
                        user,
                        store,
                        combination.OverriddenPrice.Value,
                        decimal.Zero,
                        includeDiscounts,
                        quantity,
                        tvChannel.IsRental ? rentalStartDate : null,
                        tvChannel.IsRental ? rentalEndDate : null);
            }
            else
            {
                //summarize price of all attributes
                var attributesTotalPrice = decimal.Zero;
                var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
                if (attributeValues != null)
                {
                    foreach (var attributeValue in attributeValues)
                    {
                        attributesTotalPrice += await _priceCalculationService.GetTvChannelAttributeValuePriceAdjustmentAsync(tvChannel,
                            attributeValue,
                            user,
                            store,
                            tvChannel.UserEntersPrice ? (decimal?)userEnteredPrice : null,
                            quantity);
                    }
                }

                //get price of a tvChannel (with previously calculated price of all attributes)
                if (tvChannel.UserEntersPrice)
                {
                    finalPrice = userEnteredPrice;
                }
                else
                {
                    int qty;
                    if (_shoppingCartSettings.GroupTierPricesForDistinctShoppingCartItems)
                    {
                        //the same tvChannels with distinct tvChannel attributes could be stored as distinct "ShoppingCartItem" records
                        //so let's find how many of the current tvChannels are in the cart                        
                        qty = (await GetShoppingCartAsync(user, shoppingCartType: shoppingCartType, tvChannelId: tvChannel.Id))
                            .Sum(x => x.Quantity);

                        if (qty == 0)
                        {
                            qty = quantity;
                        }
                    }
                    else
                    {
                        qty = quantity;
                    }

                    (_, finalPrice, discountAmount, appliedDiscounts) = await _priceCalculationService.GetFinalPriceAsync(tvChannel,
                        user,
                        store,
                        attributesTotalPrice,
                        includeDiscounts,
                        qty,
                        tvChannel.IsRental ? rentalStartDate : null,
                        tvChannel.IsRental ? rentalEndDate : null);
                }
            }

            //rounding
            if (_shoppingCartSettings.RoundPricesDuringCalculation)
                finalPrice = await _priceCalculationService.RoundPriceAsync(finalPrice);

            return (finalPrice, discountAmount, appliedDiscounts);
        }

        /// <summary>
        /// Finds a shopping cart item in the cart
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">Price entered by a user</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the found shopping cart item
        /// </returns>
        public virtual async Task<ShoppingCartItem> FindShoppingCartItemInTheCartAsync(IList<ShoppingCartItem> shoppingCart,
            ShoppingCartType shoppingCartType,
            TvChannel tvChannel,
            string attributesXml = "",
            decimal userEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null,
            DateTime? rentalEndDate = null)
        {
            if (shoppingCart == null)
                throw new ArgumentNullException(nameof(shoppingCart));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            return await shoppingCart.Where(sci => sci.ShoppingCartType == shoppingCartType)
                .FirstOrDefaultAwaitAsync(async sci => await ShoppingCartItemIsEqualAsync(sci, tvChannel, attributesXml, userEnteredPrice, rentalStartDate, rentalEndDate));
        }

        /// <summary>
        /// Add a tvChannel to shopping cart
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="shoppingCartType">Shopping cart type</param>
        /// <param name="storeId">Store identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">The price enter by a user</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">Quantity</param>
        /// <param name="addRequiredTvChannels">Whether to add required tvChannels</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> AddToCartAsync(User user, TvChannel tvChannel,
            ShoppingCartType shoppingCartType, int storeId, string attributesXml = null,
            decimal userEnteredPrice = decimal.Zero,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool addRequiredTvChannels = true)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var warnings = new List<string>();
            if (shoppingCartType == ShoppingCartType.ShoppingCart && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart, user))
            {
                warnings.Add("Shopping cart is disabled");
                return warnings;
            }

            if (shoppingCartType == ShoppingCartType.Wishlist && !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist, user))
            {
                warnings.Add("Wishlist is disabled");
                return warnings;
            }

            if (user.IsSearchEngineAccount())
            {
                warnings.Add("Search engine can't add to cart");
                return warnings;
            }

            if (quantity <= 0)
            {
                warnings.Add(await _localizationService.GetResourceAsync("ShoppingCart.QuantityShouldPositive"));
                return warnings;
            }

            //reset checkout info
            await _userService.ResetCheckoutDataAsync(user, storeId);

            var cart = await GetShoppingCartAsync(user, shoppingCartType, storeId);

            var shoppingCartItem = await FindShoppingCartItemInTheCartAsync(cart,
                shoppingCartType, tvChannel, attributesXml, userEnteredPrice,
                rentalStartDate, rentalEndDate);

            if (shoppingCartItem != null)
            {
                //update existing shopping cart item
                var newQuantity = shoppingCartItem.Quantity + quantity;
                warnings.AddRange(await GetShoppingCartItemWarningsAsync(user, shoppingCartType, tvChannel,
                    storeId, attributesXml,
                    userEnteredPrice, rentalStartDate, rentalEndDate,
                    newQuantity, addRequiredTvChannels, shoppingCartItem.Id));

                if (warnings.Any())
                    return warnings;

                await addRequiredTvChannelsToCartAsync();

                if (warnings.Any())
                    return warnings;

                shoppingCartItem.AttributesXml = attributesXml;
                shoppingCartItem.Quantity = newQuantity;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

                await _sciRepository.UpdateAsync(shoppingCartItem);
            }
            else
            {
                //new shopping cart item
                warnings.AddRange(await GetShoppingCartItemWarningsAsync(user, shoppingCartType, tvChannel,
                    storeId, attributesXml, userEnteredPrice,
                    rentalStartDate, rentalEndDate,
                    quantity, addRequiredTvChannels));

                if (warnings.Any())
                    return warnings;

                await addRequiredTvChannelsToCartAsync();

                if (warnings.Any())
                    return warnings;

                //maximum items validation
                switch (shoppingCartType)
                {
                    case ShoppingCartType.ShoppingCart:
                        if (cart.Count >= _shoppingCartSettings.MaximumShoppingCartItems)
                        {
                            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumShoppingCartItems"), _shoppingCartSettings.MaximumShoppingCartItems));
                            return warnings;
                        }

                        break;
                    case ShoppingCartType.Wishlist:
                        if (cart.Count >= _shoppingCartSettings.MaximumWishlistItems)
                        {
                            warnings.Add(string.Format(await _localizationService.GetResourceAsync("ShoppingCart.MaximumWishlistItems"), _shoppingCartSettings.MaximumWishlistItems));
                            return warnings;
                        }

                        break;
                    default:
                        break;
                }

                var now = DateTime.UtcNow;
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartType = shoppingCartType,
                    StoreId = storeId,
                    TvChannelId = tvChannel.Id,
                    AttributesXml = attributesXml,
                    UserEnteredPrice = userEnteredPrice,
                    Quantity = quantity,
                    RentalStartDateUtc = rentalStartDate,
                    RentalEndDateUtc = rentalEndDate,
                    CreatedOnUtc = now,
                    UpdatedOnUtc = now,
                    UserId = user.Id
                };

                await _sciRepository.InsertAsync(shoppingCartItem);

                //updated "HasShoppingCartItems" property used for performance optimization
                user.HasShoppingCartItems = !IsUserShoppingCartEmpty(user);

                await _userService.UpdateUserAsync(user);
            }

            return warnings;

            async Task addRequiredTvChannelsToCartAsync()
            {
                //get these required tvChannels
                var requiredTvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(_tvChannelService.ParseRequiredTvChannelIds(tvChannel));
                if (!requiredTvChannels.Any())
                    return;

                foreach (var requiredTvChannel in requiredTvChannels)
                {
                    var tvChannelsRequiringRequiredTvChannel = await GetTvChannelsRequiringTvChannelAsync(cart, requiredTvChannel);

                    //get the required quantity of the required tvChannel
                    var requiredTvChannelRequiredQuantity = quantity +
                        cart.Where(ci => tvChannelsRequiringRequiredTvChannel.Any(p => p.Id == ci.TvChannelId))
                            .Where(item => item.Id != (shoppingCartItem?.Id ?? 0))
                            .Sum(item => item.Quantity);

                    //whether required tvChannel is already in the cart in the required quantity
                    var quantityToAdd = requiredTvChannelRequiredQuantity - (cart.FirstOrDefault(item => item.TvChannelId == requiredTvChannel.Id)?.Quantity ?? 0);
                    if (quantityToAdd <= 0)
                        continue;

                    if (addRequiredTvChannels && tvChannel.AutomaticallyAddRequiredTvChannels)
                    {
                        //do not add required tvChannels to prevent circular references
                        var addToCartWarnings = await AddToCartAsync(user, requiredTvChannel, shoppingCartType, storeId,
                            quantity: quantityToAdd, addRequiredTvChannels: requiredTvChannel.AutomaticallyAddRequiredTvChannels);

                        if (addToCartWarnings.Any())
                        {
                            warnings.AddRange(addToCartWarnings);
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Updates the shopping cart item
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="shoppingCartItemId">Shopping cart item identifier</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="userEnteredPrice">New user entered price</param>
        /// <param name="rentalStartDate">Rental start date</param>
        /// <param name="rentalEndDate">Rental end date</param>
        /// <param name="quantity">New shopping cart item quantity</param>
        /// <param name="resetCheckoutData">A value indicating whether to reset checkout data</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warnings
        /// </returns>
        public virtual async Task<IList<string>> UpdateShoppingCartItemAsync(User user,
            int shoppingCartItemId, string attributesXml,
            decimal userEnteredPrice,
            DateTime? rentalStartDate = null, DateTime? rentalEndDate = null,
            int quantity = 1, bool resetCheckoutData = true)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var warnings = new List<string>();

            var shoppingCartItem = await _sciRepository.GetByIdAsync(shoppingCartItemId, cache => default);

            if (shoppingCartItem == null || shoppingCartItem.UserId != user.Id)
                return warnings;

            if (resetCheckoutData)
            {
                //reset checkout data
                await _userService.ResetCheckoutDataAsync(user, shoppingCartItem.StoreId);
            }

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(shoppingCartItem.TvChannelId);

            if (quantity > 0)
            {
                //check warnings
                warnings.AddRange(await GetShoppingCartItemWarningsAsync(user, shoppingCartItem.ShoppingCartType,
                    tvChannel, shoppingCartItem.StoreId,
                    attributesXml, userEnteredPrice,
                    rentalStartDate, rentalEndDate, quantity, false, shoppingCartItemId));
                if (warnings.Any())
                    return warnings;

                //if everything is OK, then update a shopping cart item
                shoppingCartItem.Quantity = quantity;
                shoppingCartItem.AttributesXml = attributesXml;
                shoppingCartItem.UserEnteredPrice = userEnteredPrice;
                shoppingCartItem.RentalStartDateUtc = rentalStartDate;
                shoppingCartItem.RentalEndDateUtc = rentalEndDate;
                shoppingCartItem.UpdatedOnUtc = DateTime.UtcNow;

                await _sciRepository.UpdateAsync(shoppingCartItem);
                await _userService.UpdateUserAsync(user);
            }
            else
            {
                //check warnings for required tvChannels
                warnings.AddRange(await GetRequiredTvChannelWarningsAsync(user, shoppingCartItem.ShoppingCartType,
                    tvChannel, shoppingCartItem.StoreId, quantity, false, shoppingCartItemId));
                if (warnings.Any())
                    return warnings;

                //delete a shopping cart item
                await DeleteShoppingCartItemAsync(shoppingCartItem, resetCheckoutData, true);
            }

            return warnings;
        }

        /// <summary>
        /// Migrate shopping cart
        /// </summary>
        /// <param name="fromUser">From user</param>
        /// <param name="toUser">To user</param>
        /// <param name="includeCouponCodes">A value indicating whether to coupon codes (discount and gift card) should be also re-applied</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task MigrateShoppingCartAsync(User fromUser, User toUser, bool includeCouponCodes)
        {
            if (fromUser == null)
                throw new ArgumentNullException(nameof(fromUser));
            if (toUser == null)
                throw new ArgumentNullException(nameof(toUser));

            if (fromUser.Id == toUser.Id)
                return; //the same user

            //shopping cart items
            var fromCart = await GetShoppingCartAsync(fromUser);

            for (var i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);

                await AddToCartAsync(toUser, tvChannel, sci.ShoppingCartType, sci.StoreId,
                    sci.AttributesXml, sci.UserEnteredPrice,
                    sci.RentalStartDateUtc, sci.RentalEndDateUtc, sci.Quantity, false);
            }

            for (var i = 0; i < fromCart.Count; i++)
            {
                var sci = fromCart[i];
                await DeleteShoppingCartItemAsync(sci);
            }

            //copy discount and gift card coupon codes
            if (includeCouponCodes)
            {
                //discount
                foreach (var code in await _userService.ParseAppliedDiscountCouponCodesAsync(fromUser))
                    await _userService.ApplyDiscountCouponCodeAsync(toUser, code);

                //gift card
                foreach (var code in await _userService.ParseAppliedGiftCardCouponCodesAsync(fromUser))
                    await _userService.ApplyGiftCardCouponCodeAsync(toUser, code);

                //save user
                await _userService.UpdateUserAsync(toUser);
            }

            //move selected checkout attributes
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(fromUser, TvProgUserDefaults.CheckoutAttributes, store.Id);
            await _genericAttributeService.SaveAttributeAsync(toUser, TvProgUserDefaults.CheckoutAttributes, checkoutAttributesXml, store.Id);
        }

        /// <summary>
        /// Indicates whether the shopping cart requires shipping
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if the shopping cart requires shipping; otherwise, false.
        /// </returns>
        public virtual async Task<bool> ShoppingCartRequiresShippingAsync(IList<ShoppingCartItem> shoppingCart)
        {
            return await shoppingCart.AnyAwaitAsync(async shoppingCartItem => await _shippingService.IsShipEnabledAsync(shoppingCartItem));
        }

        /// <summary>
        /// Gets a value indicating whether shopping cart is recurring
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> ShoppingCartIsRecurringAsync(IList<ShoppingCartItem> shoppingCart)
        {
            if (shoppingCart is null)
                throw new ArgumentNullException(nameof(shoppingCart));

            if (!shoppingCart.Any())
                return false;

            return await _tvChannelService.HasAnyRecurringTvChannelAsync(shoppingCart.Select(sci => sci.TvChannelId).ToArray());
        }

        /// <summary>
        /// Get a recurring cycle information
        /// </summary>
        /// <param name="shoppingCart">Shopping cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the error (if exists); otherwise, empty string. Cycle length. Cycle period. Total cycles
        /// </returns>
        public virtual async Task<(string error, int cycleLength, RecurringTvChannelCyclePeriod cyclePeriod, int totalCycles)> GetRecurringCycleInfoAsync(IList<ShoppingCartItem> shoppingCart)
        {
            var rezCycleLength = 0;
            RecurringTvChannelCyclePeriod rezCyclePeriod = 0;
            var rezTotalCycles = 0;

            int? cycleLength = null;
            RecurringTvChannelCyclePeriod? cyclePeriod = null;
            int? totalCycles = null;

            foreach (var sci in shoppingCart)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);
                if (tvChannel == null)
                {
                    throw new TvProgException($"TvChannel (Id={sci.TvChannelId}) cannot be loaded");
                }

                if (!tvChannel.IsRecurring)
                    continue;

                var conflictError = await _localizationService.GetResourceAsync("ShoppingCart.ConflictingShipmentSchedules");

                //cycle length
                if (cycleLength.HasValue && cycleLength.Value != tvChannel.RecurringCycleLength)
                    return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
                cycleLength = tvChannel.RecurringCycleLength;

                //cycle period
                if (cyclePeriod.HasValue && cyclePeriod.Value != tvChannel.RecurringCyclePeriod)
                    return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
                cyclePeriod = tvChannel.RecurringCyclePeriod;

                //total cycles
                if (totalCycles.HasValue && totalCycles.Value != tvChannel.RecurringTotalCycles)
                    return (conflictError, rezCycleLength, rezCyclePeriod, rezTotalCycles);
                totalCycles = tvChannel.RecurringTotalCycles;
            }

            if (!cycleLength.HasValue)
                return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);

            rezCycleLength = cycleLength.Value;
            rezCyclePeriod = cyclePeriod.Value;
            rezTotalCycles = totalCycles.Value;

            return (string.Empty, rezCycleLength, rezCyclePeriod, rezTotalCycles);
        }

        #endregion
    }
}
