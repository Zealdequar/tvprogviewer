using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Directory;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Http.Extensions;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Discounts;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Media;
using TvProgViewer.WebUI.Models.ShoppingCart;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the shopping cart model factory
    /// </summary>
    public partial class ShoppingCartModelFactory : IShoppingCartModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly UserSettings _userSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDiscountService _discountService;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPaymentService _paymentService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeFormatter _tvChannelAttributeFormatter;
        private readonly ITvChannelService _tvChannelService;
        private readonly IShippingService _shippingService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        #endregion

        #region Ctor

        public ShoppingCartModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            UserSettings userSettings,
            IAddressModelFactory addressModelFactory,
            ICheckoutAttributeFormatter checkoutAttributeFormatter,
            ICheckoutAttributeParser checkoutAttributeParser,
            ICheckoutAttributeService checkoutAttributeService,
            ICountryService countryService,
            ICurrencyService currencyService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            IDiscountService discountService,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IOrderProcessingService orderProcessingService,
            IOrderTotalCalculationService orderTotalCalculationService,
            IPaymentPluginManager paymentPluginManager,
            IPaymentService paymentService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeFormatter tvChannelAttributeFormatter,
            ITvChannelService tvChannelService,
            IShippingService shippingService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            ShippingSettings shippingSettings,
            ShoppingCartSettings shoppingCartSettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _userSettings = userSettings;
            _addressModelFactory = addressModelFactory;
            _checkoutAttributeFormatter = checkoutAttributeFormatter;
            _checkoutAttributeParser = checkoutAttributeParser;
            _checkoutAttributeService = checkoutAttributeService;
            _countryService = countryService;
            _currencyService = currencyService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
            _discountService = discountService;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _orderProcessingService = orderProcessingService;
            _orderTotalCalculationService = orderTotalCalculationService;
            _paymentPluginManager = paymentPluginManager;
            _paymentService = paymentService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _tvChannelAttributeFormatter = tvChannelAttributeFormatter;
            _tvChannelService = tvChannelService;
            _shippingService = shippingService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _shippingSettings = shippingSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the checkout attribute models
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of the checkout attribute model
        /// </returns>
        protected virtual async Task<IList<ShoppingCartModel.CheckoutAttributeModel>> PrepareCheckoutAttributeModelsAsync(
            IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new List<ShoppingCartModel.CheckoutAttributeModel>();
            var store = await _storeContext.GetCurrentStoreAsync();
            var excludeShippableAttributes = !await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
            var checkoutAttributes =
                await _checkoutAttributeService.GetAllCheckoutAttributesAsync(store.Id,
                    excludeShippableAttributes);
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue)
                };
                if (!string.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _checkoutAttributeService.GetCheckoutAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var (priceAdjustmentBase, _) = await _taxService.GetCheckoutAttributePriceAsync(attribute, attributeValue);
                            var priceAdjustment =
                                await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase,
                                    await _workContext.GetWorkingCurrencyAsync());
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment =
                                    "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment);
                        }
                    }
                }

                //set already selected attributes
                var selectedCheckoutAttributes = await _genericAttributeService.GetAttributeAsync<string>(
                    await _workContext.GetCurrentUserAsync(),
                    TvProgUserDefaults.CheckoutAttributes, store.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            //clear default selection
                            foreach (var item in attributeModel.Values)
                                item.IsPreSelected = false;

                            //select new values
                            var selectedValues =
                                _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                            foreach (var attributeValue in await selectedValues.SelectMany(x => x.values).ToListAsync())
                                foreach (var item in attributeModel.Values)
                                    if (attributeValue.Id == item.Id)
                                        item.IsPreSelected = true;
                        }
                    }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                    {
                        //do nothing
                        //values are already pre-set
                    }

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var enteredText =
                                _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (enteredText.Any())
                                attributeModel.DefaultValue = enteredText[0];
                        }
                    }

                        break;
                    case AttributeControlType.Datepicker:
                    {
                        //keep in mind my that the code below works only in the current culture
                        var selectedDateStr =
                            _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                        if (selectedDateStr.Any())
                        {
                            if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                DateTimeStyles.None, out var selectedDate))
                            {
                                //successfully parsed
                                attributeModel.SelectedDay = selectedDate.Day;
                                attributeModel.SelectedMonth = selectedDate.Month;
                                attributeModel.SelectedYear = selectedDate.Year;
                            }
                        }
                    }

                        break;
                    case AttributeControlType.FileUpload:
                    {
                        if (!string.IsNullOrEmpty(selectedCheckoutAttributes))
                        {
                            var downloadGuidStr = _checkoutAttributeParser
                                .ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                            _ = Guid.TryParse(downloadGuidStr, out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                                attributeModel.DefaultValue = download.DownloadGuid.ToString();
                        }
                    }

                        break;
                    default:
                        break;
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart item model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart item model
        /// </returns>
        protected virtual async Task<ShoppingCartModel.ShoppingCartItemModel> PrepareShoppingCartItemModelAsync(IList<ShoppingCartItem> cart, ShoppingCartItem sci)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);

            var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await _tvChannelService.FormatSkuAsync(tvChannel, sci.AttributesXml),
                VendorName = _vendorSettings.ShowVendorOnOrderDetailsPage ? (await _vendorService.GetVendorByTvChannelIdAsync(tvChannel.Id))?.Name : string.Empty,
                TvChannelId = sci.TvChannelId,
                TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                Quantity = sci.Quantity,
                AttributeInfo = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, sci.AttributesXml),
            };

            //allow editing?
            //1. setting enabled?
            //2. simple tvChannel?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             tvChannel.TvChannelType == TvChannelType.SimpleTvChannel &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              tvChannel.IsGiftCard) &&
                                             tvChannel.VisibleIndividually;

            //disable removal?
            //1. do other items require this one?
            cartItemModel.DisableRemoval = (await _shoppingCartService.GetTvChannelsRequiringTvChannelAsync(cart, tvChannel)).Any();

            //allowed quantities
            var allowedQuantities = _tvChannelService.ParseAllowedQuantities(tvChannel);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (tvChannel.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                        tvChannel.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(tvChannel.RecurringCyclePeriod));

            //rental info
            if (tvChannel.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _tvChannelService.FormatRentalDate(tvChannel, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _tvChannelService.FormatRentalDate(tvChannel, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(await _localizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            if (tvChannel.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
            {
                cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                cartItemModel.UnitPriceValue = 0;
            }
            else
            {
                var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
            }
            //subtotal, discount
            if (tvChannel.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
            {
                cartItemModel.SubTotal = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                cartItemModel.SubTotalValue = 0;
            }
            else
            {
                //sub total
                var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await _shoppingCartService.GetSubTotalAsync(sci, true);
                var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, subTotal);
                var shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
                cartItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, shoppingCartItemDiscountBase);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
                        cartItemModel.Discount = await _priceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;
                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowTvChannelImagesOnShoppingCart)
            {
                cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.TvChannelName);
            }

            //item warnings
            var itemWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(
                await _workContext.GetCurrentUserAsync(),
                sci.ShoppingCartType,
                tvChannel,
                sci.StoreId,
                sci.AttributesXml,
                sci.UserEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the wishlist item model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart item model
        /// </returns>
        protected virtual async Task<WishlistModel.ShoppingCartItemModel> PrepareWishlistItemModelAsync(ShoppingCartItem sci)
        {
            if (sci == null)
                throw new ArgumentNullException(nameof(sci));

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);

            var cartItemModel = new WishlistModel.ShoppingCartItemModel
            {
                Id = sci.Id,
                Sku = await _tvChannelService.FormatSkuAsync(tvChannel, sci.AttributesXml),
                TvChannelId = tvChannel.Id,
                TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                Quantity = sci.Quantity,
                AttributeInfo = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, sci.AttributesXml),
            };

            //allow editing?
            //1. setting enabled?
            //2. simple tvChannel?
            //3. has attribute or gift card?
            //4. visible individually?
            cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                                             tvChannel.TvChannelType == TvChannelType.SimpleTvChannel &&
                                             (!string.IsNullOrEmpty(cartItemModel.AttributeInfo) ||
                                              tvChannel.IsGiftCard) &&
                                             tvChannel.VisibleIndividually;

            //allowed quantities
            var allowedQuantities = _tvChannelService.ParseAllowedQuantities(tvChannel);
            foreach (var qty in allowedQuantities)
            {
                cartItemModel.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = sci.Quantity == qty
                });
            }

            //recurring info
            if (tvChannel.IsRecurring)
                cartItemModel.RecurringInfo = string.Format(await _localizationService.GetResourceAsync("ShoppingCart.RecurringPeriod"),
                        tvChannel.RecurringCycleLength, await _localizationService.GetLocalizedEnumAsync(tvChannel.RecurringCyclePeriod));

            //rental info
            if (tvChannel.IsRental)
            {
                var rentalStartDate = sci.RentalStartDateUtc.HasValue
                    ? _tvChannelService.FormatRentalDate(tvChannel, sci.RentalStartDateUtc.Value)
                    : string.Empty;
                var rentalEndDate = sci.RentalEndDateUtc.HasValue
                    ? _tvChannelService.FormatRentalDate(tvChannel, sci.RentalEndDateUtc.Value)
                    : string.Empty;
                cartItemModel.RentalInfo =
                    string.Format(await _localizationService.GetResourceAsync("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
            }

            //unit prices
            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            if (tvChannel.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
            {
                cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                cartItemModel.UnitPriceValue = 0;
            }
            else
            {
                var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
            }
            //subtotal, discount
            if (tvChannel.CallForPrice &&
                //also check whether the current user is impersonated
                (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
            {
                cartItemModel.SubTotal = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                cartItemModel.SubTotalValue = 0;
            }
            else
            {
                //sub total
                var (subTotal, shoppingCartItemDiscountBase, _, maximumDiscountQty) = await _shoppingCartService.GetSubTotalAsync(sci, true);
                var (shoppingCartItemSubTotalWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, subTotal);
                var shoppingCartItemSubTotalWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemSubTotalWithDiscountBase, currentCurrency);
                cartItemModel.SubTotal = await _priceFormatter.FormatPriceAsync(shoppingCartItemSubTotalWithDiscount);
                cartItemModel.SubTotalValue = shoppingCartItemSubTotalWithDiscount;
                cartItemModel.MaximumDiscountedQty = maximumDiscountQty;

                //display an applied discount amount
                if (shoppingCartItemDiscountBase > decimal.Zero)
                {
                    (shoppingCartItemDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, shoppingCartItemDiscountBase);
                    if (shoppingCartItemDiscountBase > decimal.Zero)
                    {
                        var shoppingCartItemDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartItemDiscountBase, currentCurrency);
                        cartItemModel.Discount = await _priceFormatter.FormatPriceAsync(shoppingCartItemDiscount);
                        cartItemModel.DiscountValue = shoppingCartItemDiscount;
                    }
                }
            }

            //picture
            if (_shoppingCartSettings.ShowTvChannelImagesOnWishList)
            {
                cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                    _mediaSettings.CartThumbPictureSize, true, cartItemModel.TvChannelName);
            }

            //item warnings
            var itemWarnings = await _shoppingCartService.GetShoppingCartItemWarningsAsync(
                await _workContext.GetCurrentUserAsync(),
                sci.ShoppingCartType,
                tvChannel,
                sci.StoreId,
                sci.AttributesXml,
                sci.UserEnteredPrice,
                sci.RentalStartDateUtc,
                sci.RentalEndDateUtc,
                sci.Quantity,
                false,
                sci.Id);
            foreach (var warning in itemWarnings)
                cartItemModel.Warnings.Add(warning);

            return cartItemModel;
        }

        /// <summary>
        /// Prepare the order review data model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order review data model
        /// </returns>
        protected virtual async Task<ShoppingCartModel.OrderReviewDataModel> PrepareOrderReviewDataModelAsync(IList<ShoppingCartItem> cart)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new ShoppingCartModel.OrderReviewDataModel
            {
                Display = true
            };

            //billing info
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var billingAddress = await _userService.GetUserBillingAddressAsync(user);
            if (billingAddress != null)
            {
                await _addressModelFactory.PrepareAddressModelAsync(model.BillingAddress,
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings);
            }

            //shipping info
            if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                model.IsShippable = true;

                var pickupPoint = await _genericAttributeService.GetAttributeAsync<PickupPoint>(user,
                    TvProgUserDefaults.SelectedPickupPointAttribute, store.Id);
                model.SelectedPickupInStore = _shippingSettings.AllowPickupInStore && pickupPoint != null;
                if (!model.SelectedPickupInStore)
                {
                    if (await _userService.GetUserShippingAddressAsync(user) is Address address)
                    {
                        await _addressModelFactory.PrepareAddressModelAsync(model.ShippingAddress,
                            address: address,
                            excludeProperties: false,
                            addressSettings: _addressSettings);
                    }
                }
                else
                {
                    var country = await _countryService.GetCountryByTwoLetterIsoCodeAsync(pickupPoint.CountryCode);
                    var state = await _stateProvinceService.GetStateProvinceByAbbreviationAsync(pickupPoint.StateAbbreviation, country?.Id);

                    model.PickupAddress = new AddressModel
                    {
                        Address1 = pickupPoint.Address,
                        City = pickupPoint.City,
                        County = pickupPoint.County,
                        CountryName = country?.Name ?? string.Empty,
                        StateProvinceName = state?.Name ?? string.Empty,
                        ZipPostalCode = pickupPoint.ZipPostalCode
                    };
                }

                //selected shipping method
                var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(user,
                    TvProgUserDefaults.SelectedShippingOptionAttribute, store.Id);
                if (shippingOption != null)
                    model.ShippingMethod = shippingOption.Name;
            }

            //payment info
            var selectedPaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
            var paymentMethod = await _paymentPluginManager
                .LoadPluginBySystemNameAsync(selectedPaymentMethodSystemName, user, store.Id);
            model.PaymentMethod = paymentMethod != null
                ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id)
                : string.Empty;

            //custom values
            var processPaymentRequest = _httpContextAccessor.HttpContext?.Session?.Get<ProcessPaymentRequest>("OrderPaymentInfo");
            if (processPaymentRequest != null)
                model.CustomValues = processPaymentRequest.CustomValues;

            return model;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare the estimate shipping model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="setEstimateShippingDefaultAddress">Whether to use user default shipping address for estimating</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the estimate shipping model
        /// </returns>
        public virtual async Task<EstimateShippingModel> PrepareEstimateShippingModelAsync(IList<ShoppingCartItem> cart, bool setEstimateShippingDefaultAddress = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            var model = new EstimateShippingModel
            {
                RequestDelay = _shippingSettings.RequestDelay,
                UseCity = _shippingSettings.EstimateShippingCityNameEnabled,
                Enabled = cart.Any() && await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart)
            };
            if (model.Enabled)
            {
                var user = await _workContext.GetCurrentUserAsync();
                var shippingAddress = await _userService.GetUserShippingAddressAsync(user);
                if (shippingAddress == null)
                {
                    shippingAddress = await (await _userService.GetAddressesByUserIdAsync(user.Id))
                    //enabled for the current store
                    .FirstOrDefaultAwaitAsync(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)));
                }

                //countries
                var defaultEstimateCountryId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.CountryId
                    : model.CountryId;
                model.AvailableCountries.Add(new SelectListItem
                {
                    Text = await _localizationService.GetResourceAsync("Address.SelectCountry"),
                    Value = "0"
                });

                var currentLanguage = await _workContext.GetWorkingLanguageAsync();
                foreach (var c in await _countryService.GetAllCountriesForShippingAsync(currentLanguage.Id))
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == defaultEstimateCountryId
                    });

                //states
                var defaultEstimateStateId = (setEstimateShippingDefaultAddress && shippingAddress != null)
                    ? shippingAddress.StateProvinceId
                    : model.StateProvinceId;
                var states = defaultEstimateCountryId.HasValue
                    ? (await _stateProvinceService.GetStateProvincesByCountryIdAsync(defaultEstimateCountryId.Value, currentLanguage.Id)).ToList()
                    : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                    {
                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetLocalizedAsync(s, x => x.Name),
                            Value = s.Id.ToString(),
                            Selected = s.Id == defaultEstimateStateId
                        });
                    }
                }
                else
                {
                    model.AvailableStates.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetResourceAsync("Address.Other"),
                        Value = "0"
                    });
                }

                if (setEstimateShippingDefaultAddress && shippingAddress != null)
                {
                    if (!_shippingSettings.EstimateShippingCityNameEnabled)
                        model.ZipPostalCode = shippingAddress.ZipPostalCode;
                    else
                        model.City = shippingAddress.City;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the shopping cart model
        /// </summary>
        /// <param name="model">Shopping cart model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <param name="validateCheckoutAttributes">Whether to validate checkout attributes</param>
        /// <param name="prepareAndDisplayOrderReviewData">Whether to prepare and display order review data</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart model
        /// </returns>
        public virtual async Task<ShoppingCartModel> PrepareShoppingCartModelAsync(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //simple properties
            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (!cart.Any())
                return model;

            model.IsEditable = isEditable;
            model.ShowTvChannelImages = _shoppingCartSettings.ShowTvChannelImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowSkuOnTvChannelDetailsPage;
            model.ShowVendorName = _vendorSettings.ShowVendorOnOrderDetailsPage;
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.CheckoutAttributes, store.Id);
            var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);
            if (!minOrderSubtotalAmountOk)
            {
                var minOrderSubtotalAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(_orderSettings.MinOrderSubtotalAmount, await _workContext.GetWorkingCurrencyAsync());
                model.MinOrderSubtotalWarning = string.Format(await _localizationService.GetResourceAsync("Checkout.MinOrderSubtotalAmount"), await _priceFormatter.FormatPriceAsync(minOrderSubtotalAmount, true, false));
            }

            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.TermsOfServicePopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //discount and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCodes = await _userService.ParseAppliedDiscountCouponCodesAsync(user);
            foreach (var couponCode in discountCouponCodes)
            {
                var discount = await (await _discountService.GetAllDiscountsAsync(couponCode: couponCode))
                    .FirstOrDefaultAwaitAsync(async d => d.RequiresCouponCode && (await _discountService.ValidateDiscountAsync(d, user)).IsValid);

                if (discount != null)
                {
                    model.DiscountBox.AppliedDiscountsWithCodes.Add(new ShoppingCartModel.DiscountBoxModel.DiscountInfoModel
                    {
                        Id = discount.Id,
                        CouponCode = discount.CouponCode
                    });
                }
            }

            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //checkout attributes
            model.CheckoutAttributes = await PrepareCheckoutAttributeModelsAsync(cart);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareShoppingCartItemModelAsync(cart, sci);
                model.Items.Add(cartItemModel);
            }

            //payment methods
            //all payment methods (do not filter by country here as it could be not specified yet)
            var paymentMethods = await (await _paymentPluginManager
                .LoadActivePluginsAsync(user, store.Id))
                .WhereAwait(async pm => !await pm.HidePaymentMethodAsync(cart)).ToListAsync();
            //payment methods displayed during checkout (not with "Button" type)
            var nonButtonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType != PaymentMethodType.Button)
                .ToList();
            //"button" payment methods(*displayed on the shopping cart page)
            var buttonPaymentMethods = paymentMethods
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .ToList();
            foreach (var pm in buttonPaymentMethods)
            {
                if (await _shoppingCartService.ShoppingCartIsRecurringAsync(cart) && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var viewComponent = pm.GetPublicViewComponent();
                model.ButtonPaymentMethodViewComponents.Add(viewComponent);
            }
            //hide "Checkout" button if we have only "Button" payment methods
            model.HideCheckoutButton = !nonButtonPaymentMethods.Any() && model.ButtonPaymentMethodViewComponents.Any();

            //order review data
            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData = await PrepareOrderReviewDataModelAsync(cart);
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist model
        /// </summary>
        /// <param name="model">Wishlist model</param>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the wishlist model
        /// </returns>
        public virtual async Task<WishlistModel> PrepareWishlistModelAsync(WishlistModel model, IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException(nameof(cart));

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (!cart.Any())
                return model;

            //simple properties
            var user = await _userService.GetShoppingCartUserAsync(cart);

            model.UserGuid = user.UserGuid;
            model.UserFullname = await _userService.GetUserFullNameAsync(user);
            model.ShowTvChannelImages = _shoppingCartSettings.ShowTvChannelImagesOnWishList;
            model.ShowSku = _catalogSettings.ShowSkuOnTvChannelDetailsPage;

            //cart warnings
            var cartWarnings = await _shoppingCartService.GetShoppingCartWarningsAsync(cart, string.Empty, false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            //cart items
            foreach (var sci in cart)
            {
                var cartItemModel = await PrepareWishlistItemModelAsync(sci);
                model.Items.Add(cartItemModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the mini shopping cart model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the mini shopping cart model
        /// </returns>
        public virtual async Task<MiniShoppingCartModel> PrepareMiniShoppingCartModelAsync()
        {
            var user = await _workContext.GetCurrentUserAsync();
            var model = new MiniShoppingCartModel
            {
                ShowTvChannelImages = _shoppingCartSettings.ShowTvChannelImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentUserIsGuest = await _userService.IsGuestAsync(user),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };

            //performance optimization (use "HasShoppingCartItems" property)
            if (user.HasShoppingCartItems)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var cart = await _shoppingCartService.GetShoppingCartAsync(user, ShoppingCartType.ShoppingCart, store.Id);

                if (cart.Any())
                {
                    model.TotalTvChannels = cart.Sum(item => item.Quantity);

                    //subtotal
                    var subTotalIncludingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                    var (_, _, _, subTotalWithoutDiscountBase, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
                    var subtotalBase = subTotalWithoutDiscountBase;
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var subtotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
                    model.SubTotal = await _priceFormatter.FormatPriceAsync(subtotal, false, currentCurrency, (await _workContext.GetWorkingLanguageAsync()).Id, subTotalIncludingTax);
                    model.SubTotalValue = subtotal;

                    var requiresShipping = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
                    //a user should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute
                    var checkoutAttributesExist = (await _checkoutAttributeService
                        .GetAllCheckoutAttributesAsync(store.Id, !requiresShipping))
                        .Any();

                    var minOrderSubtotalAmountOk = await _orderProcessingService.ValidateMinOrderSubtotalAmountAsync(cart);

                    var cartTvChannelIds = cart.Select(ci => ci.TvChannelId).ToArray();

                    var downloadableTvChannelsRequireRegistration =
                        _userSettings.RequireRegistrationForDownloadableTvChannels && await _tvChannelService.HasAnyDownloadableTvChannelAsync(cartTvChannelIds);

                    model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnShoppingCartPage &&
                        minOrderSubtotalAmountOk &&
                        !checkoutAttributesExist &&
                        !(downloadableTvChannelsRequireRegistration
                            && await _userService.IsGuestAsync(user));

                    //tvChannels. sort descending (recently added tvChannels)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_shoppingCartSettings.MiniShoppingCartTvChannelNumber)
                        .ToList())
                    {
                        var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);

                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            TvChannelId = sci.TvChannelId,
                            TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                            TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                            Quantity = sci.Quantity,
                            AttributeInfo = await _tvChannelAttributeFormatter.FormatAttributesAsync(tvChannel, sci.AttributesXml)
                        };

                        //unit prices
                        if (tvChannel.CallForPrice &&
                            //also check whether the current user is impersonated
                            (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
                        {
                            cartItemModel.UnitPrice = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                            cartItemModel.UnitPriceValue = 0;
                        }
                        else
                        {
                            var (shoppingCartUnitPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, (await _shoppingCartService.GetUnitPriceAsync(sci, true)).unitPrice);
                            var shoppingCartUnitPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartUnitPriceWithDiscountBase, currentCurrency);
                            cartItemModel.UnitPrice = await _priceFormatter.FormatPriceAsync(shoppingCartUnitPriceWithDiscount);
                            cartItemModel.UnitPriceValue = shoppingCartUnitPriceWithDiscount;
                        }

                        //picture
                        if (_shoppingCartSettings.ShowTvChannelImagesInMiniShoppingCart)
                        {
                            cartItemModel.Picture = await PrepareCartItemPictureModelAsync(sci,
                                _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.TvChannelName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare selected checkout attributes
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the formatted attributes
        /// </returns>
        public virtual async Task<string> FormatSelectedCheckoutAttributesAsync()
        {
            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var checkoutAttributesXml = await _genericAttributeService.GetAttributeAsync<string>(user,
                TvProgUserDefaults.CheckoutAttributes, store.Id);

            return await _checkoutAttributeFormatter.FormatAttributesAsync(checkoutAttributesXml, user);
        }

        /// <summary>
        /// Prepare the order totals model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="isEditable">Whether model is editable</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the order totals model
        /// </returns>
        public virtual async Task<OrderTotalsModel> PrepareOrderTotalsModelAsync(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel
            {
                IsEditable = isEditable
            };

            if (cart.Any())
            {
                //subtotal
                var subTotalIncludingTax = await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                var (orderSubTotalDiscountAmountBase, _, subTotalWithoutDiscountBase, _, _) = await _orderTotalCalculationService.GetShoppingCartSubTotalAsync(cart, subTotalIncludingTax);
                var subtotalBase = subTotalWithoutDiscountBase;
                var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                var subtotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(subtotalBase, currentCurrency);
                var currentLanguage = await _workContext.GetWorkingLanguageAsync();
                model.SubTotal = await _priceFormatter.FormatPriceAsync(subtotal, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderSubTotalDiscountAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(orderSubTotalDiscountAmountBase, currentCurrency);
                    model.SubTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderSubTotalDiscountAmount, true, currentCurrency, currentLanguage.Id, subTotalIncludingTax);
                }

                //shipping info
                model.RequiresShipping = await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart);
                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                if (model.RequiresShipping)
                {
                    var shoppingCartShippingBase = await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        var shoppingCartShipping = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartShippingBase.Value, currentCurrency);
                        model.Shipping = await _priceFormatter.FormatShippingPriceAsync(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(user,
                            TvProgUserDefaults.SelectedShippingOptionAttribute, store.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }
                else
                {
                    model.HideShippingTotal = _shippingSettings.HideShippingTotal;
                }

                //payment method fee
                var paymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(user, TvProgUserDefaults.SelectedPaymentMethodAttribute, store.Id);
                var paymentMethodAdditionalFee = await _paymentService.GetAdditionalHandlingFeeAsync(cart, paymentMethodSystemName);
                var (paymentMethodAdditionalFeeWithTaxBase, _) = await _taxService.GetPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFee, user);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    var paymentMethodAdditionalFeeWithTax = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(paymentMethodAdditionalFeeWithTaxBase, currentCurrency);
                    model.PaymentMethodAdditionalFee = await _priceFormatter.FormatPaymentMethodAdditionalFeeAsync(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax;
                bool displayTaxRates;
                if (_taxSettings.HideTaxInOrderSummary && await _workContext.GetTaxDisplayTypeAsync() == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    var (shoppingCartTaxBase, taxRates) = await _orderTotalCalculationService.GetTaxTotalAsync(cart);
                    var shoppingCartTax = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTaxBase, currentCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Any();
                        displayTax = !displayTaxRates;

                        model.Tax = await _priceFormatter.FormatPriceAsync(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = await _priceFormatter.FormatPriceAsync(await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tr.Value, currentCurrency), true, false),
                            });
                        }
                    }
                }

                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                var (shoppingCartTotalBase, orderTotalDiscountAmountBase, _, appliedGiftCards, redeemedRewardPoints, redeemedRewardPointsAmount) = await _orderTotalCalculationService.GetShoppingCartTotalAsync(cart);
                if (shoppingCartTotalBase.HasValue)
                {
                    var shoppingCartTotal = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shoppingCartTotalBase.Value, currentCurrency);
                    model.OrderTotal = await _priceFormatter.FormatPriceAsync(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    var orderTotalDiscountAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(orderTotalDiscountAmountBase, currentCurrency);
                    model.OrderTotalDiscount = await _priceFormatter.FormatPriceAsync(-orderTotalDiscountAmount, true, false);
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Any())
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        var amountCanBeUsed = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(appliedGiftCard.AmountCanBeUsed, currentCurrency);
                        gcModel.Amount = await _priceFormatter.FormatPriceAsync(-amountCanBeUsed, true, false);

                        var remainingAmountBase = await _giftCardService.GetGiftCardRemainingAmountAsync(appliedGiftCard.GiftCard) - appliedGiftCard.AmountCanBeUsed;
                        var remainingAmount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(remainingAmountBase, currentCurrency);
                        gcModel.Remaining = await _priceFormatter.FormatPriceAsync(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    var redeemedRewardPointsAmountInUserCurrency = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(redeemedRewardPointsAmount, currentCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = await _priceFormatter.FormatPriceAsync(-redeemedRewardPointsAmountInUserCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled && _rewardPointsSettings.DisplayHowMuchWillBeEarned && shoppingCartTotalBase.HasValue)
                {
                    //get shipping total
                    var shippingBaseInclTax = !model.RequiresShipping ? 0 : (await _orderTotalCalculationService.GetShoppingCartShippingTotalAsync(cart, true)).shippingTotal ?? 0;

                    //get total for reward points
                    var totalForRewardPoints = _orderTotalCalculationService
                        .CalculateApplicableOrderTotalForRewardPoints(shippingBaseInclTax, shoppingCartTotalBase.Value);
                    if (totalForRewardPoints > decimal.Zero)
                        model.WillEarnRewardPoints = await _orderTotalCalculationService.CalculateRewardPointsAsync(user, totalForRewardPoints);
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the estimate shipping result model
        /// </summary>
        /// <param name="cart">List of the shopping cart item</param>
        /// <param name="request">Request to get shipping options</param>
        /// <param name="cacheShippingOptions">Indicates whether to cache offered shipping options</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the estimate shipping result model
        /// </returns>
        public virtual async Task<EstimateShippingResultModel> PrepareEstimateShippingResultModelAsync(IList<ShoppingCartItem> cart, EstimateShippingModel request, bool cacheShippingOptions)
        {
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var model = new EstimateShippingResultModel();

            if (await _shoppingCartService.ShoppingCartRequiresShippingAsync(cart))
            {
                var address = new Address
                {
                    CountryId = request.CountryId,
                    StateProvinceId = request.StateProvinceId,
                    ZipPostalCode = request.ZipPostalCode,
                    City = request.City
                };

                var rawShippingOptions = new List<ShippingOption>();
                var user = await _workContext.GetCurrentUserAsync();
                var store = await _storeContext.GetCurrentStoreAsync();
                var getShippingOptionResponse = await _shippingService.GetShippingOptionsAsync(cart, address, user, storeId: store.Id);
                if (getShippingOptionResponse.Success)
                {
                    if (getShippingOptionResponse.ShippingOptions.Any())
                    {
                        foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                        {
                            rawShippingOptions.Add(new ShippingOption
                            {
                                Name = shippingOption.Name,
                                Description = shippingOption.Description,
                                Rate = shippingOption.Rate,
                                TransitDays = shippingOption.TransitDays,
                                ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName
                            });
                        }
                    }
                }
                else
                {
                    foreach (var error in getShippingOptionResponse.Errors)
                        model.Errors.Add(error);
                }

                var pickupPointsNumber = 0;
                if (_shippingSettings.AllowPickupInStore)
                {
                    var pickupPointsResponse = await _shippingService.GetPickupPointsAsync(cart, address,
                        user, storeId: store.Id);
                    if (pickupPointsResponse.Success)
                    {
                        if (pickupPointsResponse.PickupPoints.Any())
                        {
                            pickupPointsNumber = pickupPointsResponse.PickupPoints.Count;
                            var pickupPoint = pickupPointsResponse.PickupPoints.OrderBy(p => p.PickupFee).First();

                            rawShippingOptions.Add(new ShippingOption
                            {
                                Name = await _localizationService.GetResourceAsync("Checkout.PickupPoints"),
                                Description = await _localizationService.GetResourceAsync("Checkout.PickupPoints.Description"),
                                Rate = pickupPoint.PickupFee,
                                TransitDays = pickupPoint.TransitDays,
                                ShippingRateComputationMethodSystemName = pickupPoint.ProviderSystemName,
                                IsPickupInStore = true
                            });
                        }
                    }
                    else
                    {
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Errors.Add(error);
                    }
                }

                ShippingOption selectedShippingOption = null;
                if (cacheShippingOptions)
                {
                    //performance optimization. cache returned shipping options.
                    //we'll use them later (after a user has selected an option).
                    await _genericAttributeService.SaveAttributeAsync(user,
                                                           TvProgUserDefaults.OfferedShippingOptionsAttribute,
                                                           rawShippingOptions,
                                                           store.Id);

                    //find a selected (previously) shipping option
                    selectedShippingOption = await _genericAttributeService.GetAttributeAsync<ShippingOption>(user,
                            TvProgUserDefaults.SelectedShippingOptionAttribute, store.Id);
                }

                if (rawShippingOptions.Any())
                {
                    foreach (var option in rawShippingOptions)
                    {
                        var (shippingRate, _) = await _orderTotalCalculationService.AdjustShippingRateAsync(option.Rate, cart, option.IsPickupInStore);
                        (shippingRate, _) = await _taxService.GetShippingPriceAsync(shippingRate, user);
                        shippingRate = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(shippingRate, await _workContext.GetWorkingCurrencyAsync());
                        var shippingRateString = await _priceFormatter.FormatShippingPriceAsync(shippingRate, true);

                        if (option.IsPickupInStore && pickupPointsNumber > 1)
                            shippingRateString = string.Format(await _localizationService.GetResourceAsync("Shipping.EstimateShippingPopUp.Pickup.PriceFrom"), shippingRateString);

                        string deliveryDateFormat = null;
                        if (option.TransitDays.HasValue)
                        {
                            var currentCulture = CultureInfo.GetCultureInfo((await _workContext.GetWorkingLanguageAsync()).LanguageCulture);
                            var userDateTime = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.Now);
                            deliveryDateFormat = userDateTime.AddDays(option.TransitDays.Value).ToString("d", currentCulture);
                        }

                        var selected = selectedShippingOption != null &&
                                        !string.IsNullOrEmpty(option.ShippingRateComputationMethodSystemName) &&
                                        option.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase) &&
                                        (!string.IsNullOrEmpty(option.Name) &&
                                         option.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) ||
                                         (option.IsPickupInStore && option.IsPickupInStore == selectedShippingOption.IsPickupInStore));

                        model.ShippingOptions.Add(new EstimateShippingResultModel.ShippingOptionModel
                        {
                            Name = option.Name,
                            ShippingRateComputationMethodSystemName = option.ShippingRateComputationMethodSystemName,
                            Description = option.Description,
                            DisplayOrder = option.DisplayOrder ?? 0,
                            Price = shippingRateString,
                            Rate = shippingRate,
                            DeliveryDateFormat = deliveryDateFormat,
                            Selected = selected
                        });
                    }

                    //sort shipping methods
                    if (model.ShippingOptions.Count > 1)
                    {
                        model.ShippingOptions = (_shippingSettings.ShippingSorting switch
                        {
                            ShippingSortingEnum.ShippingCost => model.ShippingOptions.OrderBy(option => option.Rate),
                            _ => model.ShippingOptions.OrderBy(option => option.DisplayOrder)
                        }).ToList();
                    }

                    //if no option has been selected, let's do it for the first one
                    if (!model.ShippingOptions.Any(so => so.Selected))
                        model.ShippingOptions.First().Selected = true;
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the wishlist email a friend model
        /// </summary>
        /// <param name="model">Wishlist email a friend model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the wishlist email a friend model
        /// </returns>
        public virtual async Task<WishlistEmailAFriendModel> PrepareWishlistEmailAFriendModelAsync(WishlistEmailAFriendModel model, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailWishlistToFriendPage;
            if (!excludeProperties)
            {
                var user = await _workContext.GetCurrentUserAsync();
                model.YourEmailAddress = user.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the cart item picture model
        /// </summary>
        /// <param name="sci">Shopping cart item</param>
        /// <param name="pictureSize">Picture size</param>
        /// <param name="showDefaultPicture">Whether to show the default picture</param>
        /// <param name="tvChannelName">TvChannel name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the picture model
        /// </returns>
        public virtual async Task<PictureModel> PrepareCartItemPictureModelAsync(ShoppingCartItem sci, int pictureSize, bool showDefaultPicture, string tvChannelName)
        {
            var pictureCacheKey = _staticCacheManager.PrepareKeyForShortTermCache(TvProgModelCacheDefaults.CartPictureModelKey
                , sci, pictureSize, true, await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

            var model = await _staticCacheManager.GetAsync(pictureCacheKey, async () =>
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(sci.TvChannelId);

                //shopping cart item picture
                var sciPicture = await _pictureService.GetTvChannelPictureAsync(tvChannel, sci.AttributesXml);

                return new PictureModel
                {
                    ImageUrl = (await _pictureService.GetPictureUrlAsync(sciPicture, pictureSize, showDefaultPicture)).Url,
                    Title = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat"), tvChannelName),
                    AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat"), tvChannelName),
                };
            });

            return model;
        }

        #endregion
    }
}