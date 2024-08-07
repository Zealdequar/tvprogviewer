using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Directory;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;
using TvProgViewer.Services.Tax;
using TvProgViewer.Services.Vendors;
using TvProgViewer.WebUI.Infrastructure.Cache;
using TvProgViewer.WebUI.Models.Catalog;
using TvProgViewer.WebUI.Models.Common;
using TvProgViewer.WebUI.Models.Media;
using TvProgViewer.Services.TvProgMain;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the tvchannel model factory
    /// </summary>
    public partial class TvChannelModelFactory : ITvChannelModelFactory
    {
        #region Fields

        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly UserSettings _userSettings;
        private readonly ICategoryService _categoryService;
        private readonly ICurrencyService _currencyService;
        private readonly IUserService _userService;
        private readonly IDateRangeService _dateRangeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IDownloadService _downloadService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;
        private readonly ITvChannelService _tvchannelService;
        private readonly ITvChannelTagService _tvchannelTagService;
        private readonly ITvChannelTemplateService _tvchannelTemplateService;
        private readonly IReviewTypeService _reviewTypeService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IShoppingCartModelFactory _shoppingCartModelFactory;
        private readonly ITaxService _taxService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IVendorService _vendorService;
        private readonly IVideoService _videoService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ShippingSettings _shippingSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IProgrammeService _programmeService;
        private readonly ICommonModelFactory _commonFactory;

        #endregion

        #region Ctor

        public TvChannelModelFactory(CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            UserSettings userSettings,
            ICategoryService categoryService,
            ICurrencyService currencyService,
            IUserService userService,
            IDateRangeService dateRangeService,
            IDateTimeHelper dateTimeHelper,
            IDownloadService downloadService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            ITvChannelService tvchannelService,
            ITvChannelTagService tvchannelTagService,
            ITvChannelTemplateService tvchannelTemplateService,
            IReviewTypeService reviewTypeService,
            IShoppingCartService shoppingCartService,
            ISpecificationAttributeService specificationAttributeService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IStoreService storeService,
            IShoppingCartModelFactory shoppingCartModelFactory,
            ITaxService taxService,
            IUrlRecordService urlRecordService,
            IVendorService vendorService,
            IVideoService videoService,
            IWebHelper webHelper,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            SeoSettings seoSettings,
            ShippingSettings shippingSettings,
            VendorSettings vendorSettings,
            IProgrammeService programmeService,
            ICommonModelFactory commonFactory)
        {
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _userSettings = userSettings;
            _categoryService = categoryService;
            _currencyService = currencyService;
            _userService = userService;
            _dateRangeService = dateRangeService;
            _dateTimeHelper = dateTimeHelper;
            _downloadService = downloadService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _manufacturerService = manufacturerService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _tvchannelService = tvchannelService;
            _tvchannelTagService = tvchannelTagService;
            _tvchannelTemplateService = tvchannelTemplateService;
            _reviewTypeService = reviewTypeService;
            _shoppingCartService = shoppingCartService;
            _specificationAttributeService = specificationAttributeService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeService = storeService;
            _shoppingCartModelFactory = shoppingCartModelFactory;
            _taxService = taxService;
            _urlRecordService = urlRecordService;
            _vendorService = vendorService;
            _webHelper = webHelper;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _seoSettings = seoSettings;
            _shippingSettings = shippingSettings;
            _vendorSettings = vendorSettings;
            _videoService = videoService;
            _programmeService = programmeService;
            _commonFactory = commonFactory; 
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the tvchannel specification models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="group">Specification attribute group</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvchannel specification model
        /// </returns>
        protected virtual async Task<IList<TvChannelSpecificationAttributeModel>> PrepareTvChannelSpecificationAttributeModelAsync(TvChannel tvchannel, SpecificationAttributeGroup group)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var tvchannelSpecificationAttributes = await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(
                    tvchannel.Id, specificationAttributeGroupId: group?.Id, showOnTvChannelPage: true);

            var result = new List<TvChannelSpecificationAttributeModel>();

            foreach (var psa in tvchannelSpecificationAttributes)
            {
                var option = await _specificationAttributeService.GetSpecificationAttributeOptionByIdAsync(psa.SpecificationAttributeOptionId);

                var model = result.FirstOrDefault(model => model.Id == option.SpecificationAttributeId);
                if (model == null)
                {
                    var attribute = await _specificationAttributeService.GetSpecificationAttributeByIdAsync(option.SpecificationAttributeId);
                    model = new TvChannelSpecificationAttributeModel
                    {
                        Id = attribute.Id,
                        Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name)
                    };
                    result.Add(model);
                }

                var value = new TvChannelSpecificationAttributeValueModel
                {
                    AttributeTypeId = psa.AttributeTypeId,
                    ColorSquaresRgb = option.ColorSquaresRgb,
                    ValueRaw = psa.AttributeType switch
                    {
                        SpecificationAttributeType.Option => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(option, x => x.Name)),
                        SpecificationAttributeType.CustomText => WebUtility.HtmlEncode(await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue)),
                        SpecificationAttributeType.CustomHtmlText => await _localizationService.GetLocalizedAsync(psa, x => x.CustomValue),
                        SpecificationAttributeType.Hyperlink => $"<a href='{psa.CustomValue}' target='_blank'>{psa.CustomValue}</a>",
                        _ => null
                    }
                };

                model.Values.Add(value);
            }

            return result;
        }

        /// <summary>
        /// Prepare the tvchannel review overview model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel review overview model
        /// </returns>
        protected virtual async Task<TvChannelReviewOverviewModel> PrepareTvChannelReviewOverviewModelAsync(TvChannel tvchannel)
        {
            TvChannelReviewOverviewModel tvchannelReview;
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (_catalogSettings.ShowTvChannelReviewsPerStore)
            {
                var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelReviewsModelKey, tvchannel, currentStore);

                tvchannelReview = await _staticCacheManager.GetAsync(cacheKey, async () =>
                {
                    var tvchannelReviews = await _tvchannelService.GetAllTvChannelReviewsAsync(tvchannelId: tvchannel.Id, approved: true, storeId: currentStore.Id);
                    
                    return new TvChannelReviewOverviewModel
                    {
                        RatingSum = tvchannelReviews.Sum(pr => pr.Rating),
                        TotalReviews = tvchannelReviews.Count
                    };
                });
            }
            else
            {
                tvchannelReview = new TvChannelReviewOverviewModel
                {
                    RatingSum = tvchannel.ApprovedRatingSum,
                    TotalReviews = tvchannel.ApprovedTotalReviews
                };
            }

            if (tvchannelReview != null)
            {
                tvchannelReview.TvChannelId = tvchannel.Id;
                tvchannelReview.AllowUserReviews = tvchannel.AllowUserReviews;
                tvchannelReview.CanAddNewReview = await _tvchannelService.CanAddReviewAsync(tvchannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0);
            }

            return tvchannelReview;
        }

        /// <summary>
        /// Prepare the tvchannel overview price model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel overview price model
        /// </returns>
        protected virtual async Task<TvChannelOverviewModel.TvChannelPriceModel> PrepareTvChannelOverviewPriceModelAsync(TvChannel tvchannel, bool forceRedirectionAfterAddingToCart = false)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var priceModel = new TvChannelOverviewModel.TvChannelPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (tvchannel.TvChannelType)
            {
                case TvChannelType.GroupedTvChannel:
                    //grouped tvchannel
                    await PrepareGroupedTvChannelOverviewPriceModelAsync(tvchannel, priceModel);

                    break;
                case TvChannelType.SimpleTvChannel:
                default:
                    //simple tvchannel
                    await PrepareSimpleTvChannelOverviewPriceModelAsync(tvchannel, priceModel);

                    break;
            }

            return priceModel;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareSimpleTvChannelOverviewPriceModelAsync(TvChannel tvchannel, TvChannelOverviewModel.TvChannelPriceModel priceModel)
        {
            //add to cart button
            priceModel.DisableBuyButton = tvchannel.DisableBuyButton ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button
            priceModel.DisableWishlistButton = tvchannel.DisableWishlistButton ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //compare tvchannels
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareTvChannelsEnabled;

            //rental
            priceModel.IsRental = tvchannel.IsRental;

            //pre-order
            if (tvchannel.AvailableForPreOrder)
            {
                priceModel.AvailableForPreOrder = !tvchannel.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                  tvchannel.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                  DateTime.UtcNow;
                priceModel.PreOrderAvailabilityStartDateTimeUtc = tvchannel.PreOrderAvailabilityStartDateTimeUtc;
            }

            //prices
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                if (tvchannel.UserEntersPrice)
                    return;

                if (tvchannel.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels ||
                     _workContext.OriginalUserIfImpersonated == null))
                {
                    //call for price
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    var store = await _storeContext.GetCurrentStoreAsync();
                    var user = await _workContext.GetCurrentUserAsync();

                    //prices
                    var (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (decimal.Zero, decimal.Zero);
                    var hasMultiplePrices = false;
                    if (_catalogSettings.DisplayFromPrices)
                    {
                        var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
                        var cacheKey = _staticCacheManager
                            .PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelMultiplePriceCacheKey, tvchannel, userRoleIds, store);
                        if (!_catalogSettings.CacheTvChannelPrices || tvchannel.IsRental)
                            cacheKey.CacheTime = 0;

                        var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
                        {
                            var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();

                            // price when there are no required attributes
                            var attributesMappings = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
                            if (!attributesMappings.Any(am => !am.IsNonCombinable() && am.IsRequired))
                            {
                                (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                    .GetFinalPriceAsync(tvchannel, user, store);
                                prices.Add((priceWithoutDiscount, priceWithDiscount));
                            }

                            var allAttributesXml = await _tvchannelAttributeParser.GenerateAllCombinationsAsync(tvchannel, true);
                            foreach (var attributesXml in allAttributesXml)
                            {
                                var warnings = new List<string>();
                                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(user,
                                    ShoppingCartType.ShoppingCart, tvchannel, 1, attributesXml, true, true, true));
                                if (warnings.Count != 0)
                                    continue;

                                //get price with additional charge
                                var additionalCharge = decimal.Zero;
                                var combination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
                                if (combination?.OverriddenPrice.HasValue ?? false)
                                    additionalCharge = combination.OverriddenPrice.Value;
                                else
                                {
                                    var attributeValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        additionalCharge += await _priceCalculationService.
                                            GetTvChannelAttributeValuePriceAdjustmentAsync(tvchannel, attributeValue, user, store);
                                    }
                                }

                                if (additionalCharge != decimal.Zero)
                                {
                                    (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                        .GetFinalPriceAsync(tvchannel, user, store, additionalCharge);
                                    prices.Add((priceWithoutDiscount, priceWithDiscount));
                                }
                            }

                            if (prices.Distinct().Count() > 1)
                            {
                                (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = prices.OrderBy(p => p.PriceWithDiscount).First();
                                return new 
                                {
                                    PriceWithoutDiscount = minPossiblePriceWithoutDiscount, 
                                    PriceWithDiscount = minPossiblePriceWithDiscount 
                                };
                            }

                            // show default price when required attributes available but no values added
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvchannel, user, store);
                            
                            //don't cache (return null) if there are no multiple prices
                            return null;
                        });

                        if (cachedPrice is not null)
                        {
                            hasMultiplePrices = true;
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount) = (cachedPrice.PriceWithoutDiscount, cachedPrice.PriceWithDiscount);
                        }
                    }
                    else
                        (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvchannel, user, store);

                    if (tvchannel.HasTierPrices)
                    {
                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvchannel, user, store, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);
                    }

                    var (oldPriceBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, tvchannel.OldPrice);
                    var (finalPriceWithoutDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, minPossiblePriceWithoutDiscount);
                    var (finalPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, minPossiblePriceWithDiscount);
                    var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
                    var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                    var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                    var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                    var strikeThroughPrice = decimal.Zero;

                    if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        strikeThroughPrice = oldPrice;

                    if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        strikeThroughPrice = finalPriceWithoutDiscount;

                    if (strikeThroughPrice > decimal.Zero)
                    {
                        priceModel.OldPrice = await _priceFormatter.FormatPriceAsync(strikeThroughPrice);
                    }
                    else
                    {
                        priceModel.OldPrice = null;
                        priceModel.OldPriceValue = null;
                    }

                    //do we have tier prices configured?
                    var tierPrices = tvchannel.HasTierPrices
                        ? await _tvchannelService.GetTierPricesAsync(tvchannel, user, store)
                        : new List<TierPrice>();

                    //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                    var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);

                    var price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                    priceModel.Price = hasTierPrices || hasMultiplePrices
                        ? string.Format(await _localizationService.GetResourceAsync("TvChannels.PriceRangeFrom"), price)
                        : price;
                    priceModel.PriceValue = finalPriceWithDiscount;

                    if (tvchannel.IsRental)
                    {
                        //rental tvchannel
                        priceModel.OldPrice = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvchannel, priceModel.OldPrice);
                        priceModel.OldPriceValue = priceModel.OldPriceValue;
                        priceModel.Price = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvchannel, priceModel.Price);
                        priceModel.PriceValue = priceModel.PriceValue;
                    }

                    //property for German market
                    //we display tax/shipping info only with "shipping enabled" for this tvchannel
                    //we also ensure this it's not free shipping
                    priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoTvChannelBoxes && tvchannel.IsShipEnabled && !tvchannel.IsFreeShipping;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvchannel, finalPriceWithDiscount);
                    priceModel.BasePricePAngVValue = finalPriceWithDiscount;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareGroupedTvChannelOverviewPriceModelAsync(TvChannel tvchannel, TvChannelOverviewModel.TvChannelPriceModel priceModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var associatedTvChannels = await _tvchannelService.GetAssociatedTvChannelsAsync(tvchannel.Id,
                store.Id);

            //add to cart button (ignore "DisableBuyButton" property for grouped tvchannels)
            priceModel.DisableBuyButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button (ignore "DisableWishlistButton" property for grouped tvchannels)
            priceModel.DisableWishlistButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //compare tvchannels
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareTvChannelsEnabled;
            if (!associatedTvChannels.Any())
                return;

            //we have at least one associated tvchannel
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                //find a minimum possible price
                decimal? minPossiblePrice = null;
                TvChannel minPriceTvChannel = null;
                var user = await _workContext.GetCurrentUserAsync();
                foreach (var associatedTvChannel in associatedTvChannels)
                {
                    var (_, tmpMinPossiblePrice, _, _) = await _priceCalculationService.GetFinalPriceAsync(associatedTvChannel, user, store);

                    if (associatedTvChannel.HasTierPrices)
                    {
                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        tmpMinPossiblePrice = Math.Min(tmpMinPossiblePrice,
                            (await _priceCalculationService.GetFinalPriceAsync(associatedTvChannel, user, store, quantity: int.MaxValue)).finalPrice);
                    }

                    if (minPossiblePrice.HasValue && tmpMinPossiblePrice >= minPossiblePrice.Value)
                        continue;
                    minPriceTvChannel = associatedTvChannel;
                    minPossiblePrice = tmpMinPossiblePrice;
                }

                if (minPriceTvChannel == null || minPriceTvChannel.UserEntersPrice)
                    return;

                if (minPriceTvChannel.CallForPrice &&
                    //also check whether the current user is impersonated
                    (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels ||
                     _workContext.OriginalUserIfImpersonated == null))
                {
                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = await _localizationService.GetResourceAsync("TvChannels.CallForPrice");
                    priceModel.PriceValue = null;
                }
                else
                {
                    //calculate prices
                    var (finalPriceBase, _) = await _taxService.GetTvChannelPriceAsync(minPriceTvChannel, minPossiblePrice.Value);
                    var finalPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceBase, await _workContext.GetWorkingCurrencyAsync());

                    priceModel.OldPrice = null;
                    priceModel.OldPriceValue = null;
                    priceModel.Price = string.Format(await _localizationService.GetResourceAsync("TvChannels.PriceRangeFrom"), await _priceFormatter.FormatPriceAsync(finalPrice));
                    priceModel.PriceValue = finalPrice;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvchannel, finalPriceBase);
                    priceModel.BasePricePAngVValue = finalPriceBase;
                }
            }
            else
            {
                //hide prices
                priceModel.OldPrice = null;
                priceModel.OldPriceValue = null;
                priceModel.Price = null;
                priceModel.PriceValue = null;
            }
        }

        /// <summary>
        /// Prepare the tvchannel overview picture model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="tvchannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains picture models
        /// </returns>
        protected virtual async Task<IList<PictureModel>> PrepareTvChannelOverviewPicturesModelAsync(TvChannel tvchannel, int? tvchannelThumbPictureSize = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var tvchannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = tvchannelThumbPictureSize ?? _mediaSettings.TvChannelThumbPictureSize;

            //prepare picture model
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelOverviewPicturesModelKey, 
                tvchannel, pictureSize, true, _catalogSettings.DisplayAllPicturesOnCatalogPages, await _workContext.GetWorkingLanguageAsync(), 
                _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());

            var cachedPictures = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                async Task<PictureModel> preparePictureModelAsync(Picture picture)
                {
                    //we use the Task.WhenAll method to control that both image thumbs was created in same time.
                    //without this method, sometimes there were situations when one of the pictures was not generated on time
                    //this section of code requires detailed analysis in the future
                    var picResultTasks = await Task.WhenAll(_pictureService.GetPictureUrlAsync(picture, pictureSize), _pictureService.GetPictureUrlAsync(picture));

                    var (imageUrl, _) = picResultTasks[0];
                    var (fullSizeImageUrl, _) = picResultTasks[1];
                    return new PictureModel
                    {
                        ImageUrl = imageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        //"title" attribute
                        Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                            ? picture.TitleAttribute
                            : string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat"),
                                tvchannelName),
                        //"alt" attribute
                        AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                            ? picture.AltAttribute
                            : string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat"),
                                tvchannelName)
                    };
                }

                //all pictures
                var pictures = (await _pictureService
                    .GetPicturesByTvChannelIdAsync(tvchannel.Id,  _catalogSettings.DisplayAllPicturesOnCatalogPages ? 0 : 1))
                    .DefaultIfEmpty(null);
                var pictureModels = await pictures
                    .SelectAwait(async picture => await preparePictureModelAsync(picture))
                    .ToListAsync();
                return pictureModels;
            });

            return cachedPictures;
        }

        /// <summary>
        /// Prepare the tvchannel breadcrumb model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel breadcrumb model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.TvChannelBreadcrumbModel> PrepareTvChannelBreadcrumbModelAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var breadcrumbModel = new TvChannelDetailsModel.TvChannelBreadcrumbModel
            {
                Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                TvChannelId = tvchannel.Id,
                TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel)
            };
            var tvchannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvchannel.Id);
            if (!tvchannelCategories.Any())
                return breadcrumbModel;

            var category = await _categoryService.GetCategoryByIdAsync(tvchannelCategories[0].CategoryId);
            if (category == null)
                return breadcrumbModel;

            foreach (var catBr in await _categoryService.GetCategoryBreadCrumbAsync(category))
            {
                breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                {
                    Id = catBr.Id,
                    Name = await _localizationService.GetLocalizedAsync(catBr, x => x.Name),
                    SeName = await _urlRecordService.GetSeNameAsync(catBr),
                    IncludeInTopMenu = catBr.IncludeInTopMenu
                });
            }

            return breadcrumbModel;
        }

        /// <summary>
        /// Prepare the tvchannel tag models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvchannel tag model
        /// </returns>
        protected virtual async Task<IList<TvChannelTagModel>> PrepareTvChannelTagModelsAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var store = await _storeContext.GetCurrentStoreAsync();
            var tvchannelsTags = await _tvchannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvchannel.Id);

            var model = await tvchannelsTags
                    //filter by store
                    .WhereAwait(async x => await _tvchannelTagService.GetTvChannelCountByTvChannelTagIdAsync(x.Id, store.Id) > 0)
                    .SelectAwait(async x => new TvChannelTagModel
                    {
                        Id = x.Id,
                        Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(x),
                        TvChannelCount = await _tvchannelTagService.GetTvChannelCountByTvChannelTagIdAsync(x.Id, store.Id)
                    }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel price model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel price model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.TvChannelPriceModel> PrepareTvChannelPriceModelAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = new TvChannelDetailsModel.TvChannelPriceModel
            {
                TvChannelId = tvchannel.Id
            };

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (tvchannel.UserEntersPrice)
                {
                    model.UserEntersPrice = true;
                }
                else
                {
                    if (tvchannel.CallForPrice &&
                        //also check whether the current user is impersonated
                        (!_orderSettings.AllowAdminsToBuyCallForPriceTvChannels || _workContext.OriginalUserIfImpersonated == null))
                    {
                        model.CallForPrice = true;
                    }
                    else
                    {
                        var user = await _workContext.GetCurrentUserAsync();
                        var store = await _storeContext.GetCurrentStoreAsync();
                        var currentCurrency = await _workContext.GetWorkingCurrencyAsync();

                        var (oldPriceBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, tvchannel.OldPrice);

                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, (await _priceCalculationService.GetFinalPriceAsync(tvchannel, user, store, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, (await _priceCalculationService.GetFinalPriceAsync(tvchannel, user, store)).finalPrice);
                        
                        var oldPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(oldPriceBase, currentCurrency);
                        var finalPriceWithoutDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithoutDiscountBase, currentCurrency);
                        var finalPriceWithDiscount = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(finalPriceWithDiscountBase, currentCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                        {
                            model.OldPrice = await _priceFormatter.FormatPriceAsync(oldPrice);
                            model.OldPriceValue = oldPrice;
                        }

                        model.Price = await _priceFormatter.FormatPriceAsync(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                        {
                            model.PriceWithDiscount = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.PriceWithDiscountValue = finalPriceWithDiscount;
                        }

                        model.PriceValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this tvchannel
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoTvChannelDetailsPage
                            && tvchannel.IsShipEnabled &&
                            !tvchannel.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvchannel, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
                        //currency code
                        model.CurrencyCode = currentCurrency.CurrencyCode;

                        //rental
                        if (tvchannel.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.RentalPrice = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvchannel, priceStr);
                            model.RentalPriceValue = finalPriceWithDiscount;
                        }
                    }
                }
            }
            else
            {
                model.HidePrices = true;
                model.OldPrice = null;
                model.OldPriceValue = null;
                model.Price = null;
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel add to cart model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel add to cart model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.AddToCartModel> PrepareTvChannelAddToCartModelAsync(TvChannel tvchannel, ShoppingCartItem updatecartitem)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = new TvChannelDetailsModel.AddToCartModel
            {
                TvChannelId = tvchannel.Id
            };

            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : tvchannel.OrderMinimumQuantity;
            //allowed quantities
            var allowedQuantities = _tvchannelService.ParseAllowedQuantities(tvchannel);
            foreach (var qty in allowedQuantities)
            {
                model.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }
            //minimum quantity notification
            if (tvchannel.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("TvChannels.MinimumQuantityNotification"), tvchannel.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = tvchannel.DisableBuyButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = tvchannel.DisableWishlistButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist);
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (tvchannel.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !tvchannel.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    tvchannel.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = tvchannel.PreOrderAvailabilityStartDateTimeUtc;

                if (model.AvailableForPreOrder &&
                    model.PreOrderAvailabilityStartDateTimeUtc.HasValue &&
                    _catalogSettings.DisplayDatePreOrderAvailability)
                {
                    model.PreOrderAvailabilityStartDateTimeUserTime =
                        (await _dateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
                }
            }
            //rental
            model.IsRental = tvchannel.IsRental;

            //user entered price
            model.UserEntersPrice = tvchannel.UserEntersPrice;
            if (!model.UserEntersPrice)
                return model;

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var minimumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvchannel.MinimumUserEnteredPrice, currentCurrency);
            var maximumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvchannel.MaximumUserEnteredPrice, currentCurrency);

            model.UserEnteredPrice = updatecartitem != null ? updatecartitem.UserEnteredPrice : minimumUserEnteredPrice;
            model.UserEnteredPriceRange = string.Format(await _localizationService.GetResourceAsync("TvChannels.EnterTvChannelPrice.Range"),
                await _priceFormatter.FormatPriceAsync(minimumUserEnteredPrice, false, false),
                await _priceFormatter.FormatPriceAsync(maximumUserEnteredPrice, false, false));

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel attribute models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvchannel attribute model
        /// </returns>
        protected virtual async Task<IList<TvChannelDetailsModel.TvChannelAttributeModel>> PrepareTvChannelAttributeModelsAsync(TvChannel tvchannel, ShoppingCartItem updatecartitem)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = new List<TvChannelDetailsModel.TvChannelAttributeModel>();
            var store = updatecartitem != null ? await _storeService.GetStoreByIdAsync(updatecartitem.StoreId) : await _storeContext.GetCurrentStoreAsync();
            
            var tvchannelAttributeMapping = await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id);
            foreach (var attribute in tvchannelAttributeMapping)
            {
                var tvchannelAttrubute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId);

                var attributeModel = new TvChannelDetailsModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelId = tvchannel.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = await _localizationService.GetLocalizedAsync(tvchannelAttrubute, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(tvchannelAttrubute, x => x.Description),
                    TextPrompt = await _localizationService.GetLocalizedAsync(attribute, x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : await _localizationService.GetLocalizedAsync(attribute, x => x.DefaultValue),
                    HasCondition = !string.IsNullOrEmpty(attribute.ConditionAttributeXml)
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
                    var attributeValues = await _tvchannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new TvChannelDetailsModel.TvChannelAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected,
                            UserEntersQty = attributeValue.UserEntersQty,
                            Quantity = attributeValue.Quantity
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
                        {
                            var currentUser = await _workContext.GetCurrentUserAsync();
                            var user = updatecartitem?.UserId is null ? currentUser : await _userService.GetUserByIdAsync(updatecartitem.UserId);

                            var attributeValuePriceAdjustment = await _priceCalculationService.GetTvChannelAttributeValuePriceAdjustmentAsync(tvchannel, attributeValue, user, store, quantity: updatecartitem?.Quantity ?? 1);
                            var (priceAdjustmentBase, _) = await _taxService.GetTvChannelPriceAsync(tvchannel, attributeValuePriceAdjustment);
                            var priceAdjustment = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceAdjustmentBase, await _workContext.GetWorkingCurrencyAsync());

                            if (attributeValue.PriceAdjustmentUsePercentage)
                            {
                                var priceAdjustmentStr = attributeValue.PriceAdjustment.ToString("G29");
                                if (attributeValue.PriceAdjustment > decimal.Zero)
                                    valueModel.PriceAdjustment = "+";
                                valueModel.PriceAdjustment += priceAdjustmentStr + "%";
                            }
                            else
                            {
                                if (priceAdjustmentBase > decimal.Zero)
                                    valueModel.PriceAdjustment = "+" + await _priceFormatter.FormatPriceAsync(priceAdjustment, false, false);
                                else if (priceAdjustmentBase < decimal.Zero)
                                    valueModel.PriceAdjustment = "-" + await _priceFormatter.FormatPriceAsync(-priceAdjustment, false, false);
                            }

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //"image square" picture (with with "image squares" attribute type only)
                        if (attributeValue.ImageSquaresPictureId > 0)
                        {
                            var tvchannelAttributeImageSquarePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelAttributeImageSquarePictureModelKey
                                , attributeValue.ImageSquaresPictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    await _storeContext.GetCurrentStoreAsync());
                            valueModel.ImageSquaresPictureModel = await _staticCacheManager.GetAsync(tvchannelAttributeImageSquarePictureCacheKey, async () =>
                            {
                                var imageSquaresPicture = await _pictureService.GetPictureByIdAsync(attributeValue.ImageSquaresPictureId);
                                string fullSizeImageUrl, imageUrl;
                                (imageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture, _mediaSettings.ImageSquarePictureSize);
                                (fullSizeImageUrl, imageSquaresPicture) = await _pictureService.GetPictureUrlAsync(imageSquaresPicture);

                                if (imageSquaresPicture != null)
                                {
                                    return new PictureModel
                                    {
                                        FullSizeImageUrl = fullSizeImageUrl,
                                        ImageUrl = imageUrl
                                    };
                                }

                                return new PictureModel();
                            });
                        }

                        //picture of a tvchannel attribute value
                        valueModel.PictureId = attributeValue.PictureId;
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                            {
                                                item.IsPreSelected = true;

                                                //set user entered quantity
                                                if (attributeValue.UserEntersQty)
                                                    item.Quantity = attributeValue.Quantity;
                                            }
                                }
                            }

                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //values are already pre-set

                                //set user entered quantity
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    foreach (var attributeValue in (await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(updatecartitem.AttributesXml))
                                        .Where(value => value.UserEntersQty))
                                    {
                                        var item = attributeModel.Values.FirstOrDefault(value => value.Id == attributeValue.Id);
                                        if (item != null)
                                            item.Quantity = attributeValue.Quantity;
                                    }
                                }
                            }

                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _tvchannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }

                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _tvchannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Any())
                                {
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture, DateTimeStyles.None, out var selectedDate))
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
                                if (!string.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var downloadGuidStr = _tvchannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
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
                }

                model.Add(attributeModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel tier price models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tier price model
        /// </returns>
        protected virtual async Task<IList<TvChannelDetailsModel.TierPriceModel>> PrepareTvChannelTierPriceModelsAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var model = await (await _tvchannelService.GetTierPricesAsync(tvchannel, user, store))
                .SelectAwait(async tierPrice =>
                {
                    var priceBase = (await _taxService.GetTvChannelPriceAsync(tvchannel, (await _priceCalculationService.GetFinalPriceAsync(tvchannel,
                        user, store, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts,
                        tierPrice.Quantity)).finalPrice)).price;

                       var price = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(priceBase, await _workContext.GetWorkingCurrencyAsync());

                       return new TvChannelDetailsModel.TierPriceModel
                       {
                           Quantity = tierPrice.Quantity,
                           Price = await _priceFormatter.FormatPriceAsync(price, false, false),
                           PriceValue = price
                       };
                   }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel manufacturer models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturer brief info model
        /// </returns>
        protected virtual async Task<IList<ManufacturerBriefInfoModel>> PrepareTvChannelManufacturerModelsAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = await (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvchannel.Id))
                .SelectAwait(async pm =>
                {
                    var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(pm.ManufacturerId);
                    var modelMan = new ManufacturerBriefInfoModel
                    {
                        Id = manufacturer.Id,
                        Name = await _localizationService.GetLocalizedAsync(manufacturer, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(manufacturer)
                    };

                    return modelMan;
                }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel details picture model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="isAssociatedTvChannel">Whether the tvchannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the picture model for the default picture; All picture models
        /// </returns>
        protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels, IList<VideoModel> allVideoModels)> PrepareTvChannelDetailsPictureModelAsync(TvChannel tvchannel, bool isAssociatedTvChannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //default picture size
            var defaultPictureSize = isAssociatedTvChannel ?
                _mediaSettings.AssociatedTvChannelPictureSize :
                _mediaSettings.TvChannelDetailsPictureSize;

            //prepare picture models
            var tvchannelPicturesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelDetailsPicturesModelKey
                , tvchannel, defaultPictureSize, isAssociatedTvChannel, 
                await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
            var cachedPictures = await _staticCacheManager.GetAsync(tvchannelPicturesCacheKey, async () =>
            {
                var tvchannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name);

                var pictures = await _pictureService.GetPicturesByTvChannelIdAsync(tvchannel.Id);
                var defaultPicture = pictures.FirstOrDefault();

                string fullSizeImageUrl, imageUrl, thumbImageUrl;
                (imageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, defaultPictureSize, !isAssociatedTvChannel);
                (fullSizeImageUrl, defaultPicture) = await _pictureService.GetPictureUrlAsync(defaultPicture, 0, !isAssociatedTvChannel);
                
                var defaultPictureModel = new PictureModel
                {
                    ImageUrl = imageUrl,
                    FullSizeImageUrl = fullSizeImageUrl
                };
                //"title" attribute
                defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                    defaultPicture.TitleAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvchannelName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvchannelName);

                //all pictures
                var pictureModels = new List<PictureModel>();
                for (var i = 0; i < pictures.Count; i++ )
                {
                    var picture = pictures[i];

                    (imageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, defaultPictureSize, !isAssociatedTvChannel);
                    (fullSizeImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture);
                    (thumbImageUrl, picture) = await _pictureService.GetPictureUrlAsync(picture, _mediaSettings.TvChannelThumbPictureSizeOnTvChannelDetailsPage);
                    
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = imageUrl,
                        ThumbImageUrl = thumbImageUrl,
                        FullSizeImageUrl = fullSizeImageUrl,
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvchannelName),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvchannelName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvchannelName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvchannelName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            var allPictureModels = cachedPictures.PictureModels;
            
            //all videos
            var allvideoModels = new List<VideoModel>();
            var videos = await _videoService.GetVideosByTvChannelIdAsync(tvchannel.Id);
            foreach (var video in videos)
            {
                var videoModel = new VideoModel
                {
                    VideoUrl = video.VideoUrl,
                    Allow = _mediaSettings.VideoIframeAllow,
                    Width = _mediaSettings.VideoIframeWidth,
                    Height = _mediaSettings.VideoIframeHeight
                };

                allvideoModels.Add(videoModel);
            }
            return (cachedPictures.DefaultPictureModel, allPictureModels, allvideoModels);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get the tvchannel template view path
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the view path
        /// </returns>
        public virtual async Task<string> PrepareTvChannelTemplateViewPathAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var template = await _tvchannelTemplateService.GetTvChannelTemplateByIdAsync(tvchannel.TvChannelTemplateId) ??
                           (await _tvchannelTemplateService.GetAllTvChannelTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare the tvchannel overview models
        /// </summary>
        /// <param name="tvchannels">Collection of tvchannels</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="tvchannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of tvchannel overview model
        /// </returns>
        public virtual async Task<IEnumerable<TvChannelOverviewModel>> PrepareTvChannelOverviewModelsAsync(IEnumerable<TvChannel> tvchannels,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? tvchannelThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (tvchannels == null)
                throw new ArgumentNullException(nameof(tvchannels));

            var models = new List<TvChannelOverviewModel>();
            foreach (var tvchannel in tvchannels)
            {
                var model = new TvChannelOverviewModel
                {
                    Id = tvchannel.Id,
                    Name = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                    Sku = tvchannel.Sku,
                    TvChannelType = tvchannel.TvChannelType,
                    MarkAsNew = tvchannel.MarkAsNew &&
                        (!tvchannel.MarkAsNewStartDateTimeUtc.HasValue || tvchannel.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!tvchannel.MarkAsNewEndDateTimeUtc.HasValue || tvchannel.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                //price
                if (preparePriceModel)
                {
                    model.TvChannelPrice = await PrepareTvChannelOverviewPriceModelAsync(tvchannel, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.PictureModels = await PrepareTvChannelOverviewPicturesModelAsync(tvchannel, tvchannelThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.TvChannelSpecificationModel = await PrepareTvChannelSpecificationModelAsync(tvchannel);
                }

                //reviews
                model.ReviewOverviewModel = await PrepareTvChannelReviewOverviewModelAsync(tvchannel);

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Prepare the tvchannel combination models
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel combination models
        /// </returns>
        public virtual async Task<IList<TvChannelCombinationModel>> PrepareTvChannelCombinationModelsAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var result = new List<TvChannelCombinationModel>();

            var combinations = await _tvchannelAttributeService
                .GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id);
            if (combinations?.Any() == true)
            {
                foreach (var combination in combinations)
                {
                    var combinationModel = new TvChannelCombinationModel
                    {
                        InStock = combination.StockQuantity > 0 || combination.AllowOutOfStockOrders
                    };

                    var mappings = await _tvchannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                    if (mappings == null || mappings.Count == 0)
                        continue;

                    foreach (var mapping in mappings)
                    {
                        var attributeModel = new TvChannelAttributeModel
                        {
                            Id = mapping.Id
                        };

                        var values = await _tvchannelAttributeParser
                            .ParseTvChannelAttributeValuesAsync(combination.AttributesXml, mapping.Id);
                        if (values == null || values.Count == 0)
                            continue;

                        foreach (var value in values)
                            attributeModel.ValueIds.Add(value.Id);

                        combinationModel.Attributes.Add(attributeModel);
                    }

                    result.Add(combinationModel);
                }
            }

            return result;
        }

        /// <summary>
        /// Prepare the tvchannel details model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedTvChannel">Whether the tvchannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel details model
        /// </returns>
        public virtual async Task<TvChannelDetailsModel> PrepareTvChannelDetailsModelAsync(TvChannel tvchannel,
            ShoppingCartItem updatecartitem = null, bool isAssociatedTvChannel = false)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            //standard properties
            var model = new TvChannelDetailsModel
            {
                Id = tvchannel.Id,
                Name = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.FullDescription),
                MetaKeywords = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(tvchannel, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                TvChannelType = tvchannel.TvChannelType,
                ShowSku = _catalogSettings.ShowSkuOnTvChannelDetailsPage,
                Sku = tvchannel.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = tvchannel.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = tvchannel.Gtin,
                ManageInventoryMethod = tvchannel.ManageInventoryMethod,
                StockAvailability = await _tvchannelService.FormatStockMessageAsync(tvchannel, string.Empty),
                HasSampleDownload = tvchannel.IsDownload && tvchannel.HasSampleDownload,
                DisplayDiscontinuedMessage = !tvchannel.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedTvChannels,
                AvailableEndDate = tvchannel.AvailableEndDateTimeUtc,
                VisibleIndividually = tvchannel.VisibleIndividually,
                AllowAddingOnlyExistingAttributeCombinations = tvchannel.AllowAddingOnlyExistingAttributeCombinations
            };

            //automatically generate tvchannel description?
            if (_seoSettings.GenerateTvChannelMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = tvchannel.IsShipEnabled;
            if (tvchannel.IsShipEnabled)
            {
                model.IsFreeShipping = tvchannel.IsFreeShipping;
                //delivery date
                var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(tvchannel.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = await _localizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
                }
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            //tvchannel live url
            model.TvChannelLiveUrlEnabled = _catalogSettings.TvChannelLiveUrlEnabled;
            model.TvChannelLiveUrl = tvchannel.TvChannelLiveUrl;
            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare tvchannels
            model.CompareTvChannelsEnabled = _catalogSettings.CompareTvChannelsEnabled;
            //store name
            model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

            //vendor details
            if (_vendorSettings.ShowVendorOnTvChannelDetailsPage)
            {
                var vendor = await _vendorService.GetVendorByIdAsync(tvchannel.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = await _localizationService.GetLocalizedAsync(vendor, x => x.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(vendor),
                    };
                }
            }

            //page sharing
            if (_catalogSettings.ShowShareButton && !string.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the add this link to be https linked when the page is, so that the page doesn't ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }

                model.PageShareCode = shareCode;
            }

            switch (tvchannel.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    model.InStock = true;
                    break;

                case ManageInventoryMethod.ManageStock:
                    model.InStock = tvchannel.BackorderMode != BackorderMode.NoBackorders
                        || await _tvchannelService.GetTotalStockQuantityAsync(tvchannel) > 0;
                    model.DisplayBackInStockSubscription = !model.InStock && tvchannel.AllowBackInStockSubscriptions;
                    break;

                case ManageInventoryMethod.ManageStockByAttributes:
                    model.InStock = (await _tvchannelAttributeService
                        .GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id))
                        ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                        ?? false;
                    break;
            }

            //breadcrumb
            //do not prepare this model for the associated tvchannels. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedTvChannel)
            {
                model.Breadcrumb = await PrepareTvChannelBreadcrumbModelAsync(tvchannel);
            }

            //tvchannel tags
            //do not prepare this model for the associated tvchannels. anyway it's not used
            if (!isAssociatedTvChannel)
            {
                model.TvChannelTags = await PrepareTvChannelTagModelsAsync(tvchannel);
            }

            //pictures and videos
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            IList<PictureModel> allPictureModels;
            IList<VideoModel> allVideoModels;
            (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareTvChannelDetailsPictureModelAsync(tvchannel, isAssociatedTvChannel);
            model.PictureModels = allPictureModels;
            model.VideoModels = allVideoModels;

            //price
            model.TvChannelPrice = await PrepareTvChannelPriceModelAsync(tvchannel);

            //'Add to cart' model
            model.AddToCart = await PrepareTvChannelAddToCartModelAsync(tvchannel, updatecartitem);
            var user = await _workContext.GetCurrentUserAsync();
            //gift card
            if (tvchannel.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = tvchannel.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = await _userService.GetUserFullNameAsync(user);
                    model.GiftCard.SenderEmail = user.Email;
                }
                else
                {
                    _tvchannelAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out var giftCardRecipientName, out var giftCardRecipientEmail,
                        out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //tvchannel attributes
            model.TvChannelAttributes = await PrepareTvChannelAttributeModelsAsync(tvchannel, updatecartitem);

            //tvchannel specifications
            //do not prepare this model for the associated tvchannels. anyway it's not used
            if (!isAssociatedTvChannel)
            {
                model.TvChannelSpecificationModel = await PrepareTvChannelSpecificationModelAsync(tvchannel);
            }

            //tvchannel review overview
            model.TvChannelReviewOverview = await PrepareTvChannelReviewOverviewModelAsync(tvchannel);

            //tier prices
            if (tvchannel.HasTierPrices && await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = await PrepareTvChannelTierPriceModelsAsync(tvchannel);
            }

            model.TvTypeProgSelector = await _commonFactory.PrepareTvTypeProgSelectorModelAsync();

            var typeProg = model.TvTypeProgSelector.CurrentTypeProgId;

            model.TvChannelDays = await _programmeService.GetDaysAsync(typeProg);

            //manufacturers
            model.TvChannelManufacturers = await PrepareTvChannelManufacturerModelsAsync(tvchannel);

            //rental tvchannels
            if (tvchannel.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            //estimate shipping
            if (_shippingSettings.EstimateShippingTvChannelPageEnabled && !model.IsFreeShipping)
            {
                var wrappedTvChannel = new ShoppingCartItem
                {
                    StoreId = store.Id,
                    ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart,
                    UserId = user.Id,
                    TvChannelId = tvchannel.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                var estimateShippingModel = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedTvChannel });

                model.TvChannelEstimateShipping.TvChannelId = tvchannel.Id;
                model.TvChannelEstimateShipping.RequestDelay = estimateShippingModel.RequestDelay;
                model.TvChannelEstimateShipping.Enabled = estimateShippingModel.Enabled;
                model.TvChannelEstimateShipping.CountryId = estimateShippingModel.CountryId;
                model.TvChannelEstimateShipping.StateProvinceId = estimateShippingModel.StateProvinceId;
                model.TvChannelEstimateShipping.ZipPostalCode = estimateShippingModel.ZipPostalCode;
                model.TvChannelEstimateShipping.UseCity = estimateShippingModel.UseCity;
                model.TvChannelEstimateShipping.City = estimateShippingModel.City;
                model.TvChannelEstimateShipping.AvailableCountries = estimateShippingModel.AvailableCountries;
                model.TvChannelEstimateShipping.AvailableStates = estimateShippingModel.AvailableStates;
            }

            //associated tvchannels
            if (tvchannel.TvChannelType == TvChannelType.GroupedTvChannel)
            {
                //ensure no circular references
                if (!isAssociatedTvChannel)
                {
                    var associatedTvChannels = await _tvchannelService.GetAssociatedTvChannelsAsync(tvchannel.Id, store.Id);
                    foreach (var associatedTvChannel in associatedTvChannels)
                        model.AssociatedTvChannels.Add(await PrepareTvChannelDetailsModelAsync(associatedTvChannel, null, true));
                }
                model.InStock = model.AssociatedTvChannels.Any(associatedTvChannel => associatedTvChannel.InStock);
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel reviews model
        /// </summary>
        /// <param name="model">TvChannel reviews model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel reviews model
        /// </returns>
        public virtual async Task<TvChannelReviewsModel> PrepareTvChannelReviewsModelAsync(TvChannelReviewsModel model, TvChannel tvchannel)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            model.TvChannelId = tvchannel.Id;
            string tvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name);
            model.TvChannelName = tvChannelName;
            model.TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel);

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var tvchannelReviews = await _tvchannelService.GetAllTvChannelReviewsAsync(
                approved: true, 
                tvchannelId: tvchannel.Id,
                storeId: _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0);

            //get all review types
            foreach (var reviewType in await _reviewTypeService.GetAllReviewTypesAsync())
            {
                model.ReviewTypeList.Add(new ReviewTypeModel
                {
                    Id = reviewType.Id,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                    Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                    VisibleToAllUsers = reviewType.VisibleToAllUsers,
                    DisplayOrder = reviewType.DisplayOrder,
                    IsRequired = reviewType.IsRequired,
                });
            }

            var currentUser = await _workContext.GetCurrentUserAsync();

            //filling data from db
            foreach (var pr in tvchannelReviews)
            {
                var user = await _userService.GetUserByIdAsync(pr.UserId);

                var tvchannelReviewModel = new TvChannelReviewModel
                {
                    Id = pr.Id,
                    UserId = pr.UserId,
                    UserName = await _userService.FormatUsernameAsync(user),
                    AllowViewingProfiles = _userSettings.AllowViewingProfiles && user != null && !await _userService.IsGuestAsync(user),
                    Title = pr.Title,
                    ReviewText = pr.ReviewText,
                    ReplyText = pr.ReplyText,
                    Rating = pr.Rating,
                    Helpfulness = new TvChannelReviewHelpfulnessModel
                    {
                        TvChannelReviewId = pr.Id,
                        HelpfulYesTotal = pr.HelpfulYesTotal,
                        HelpfulNoTotal = pr.HelpfulNoTotal,
                    },
                    WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(pr.CreatedOnUtc, DateTimeKind.Utc)).ToString("g"),
                };

                if (_userSettings.AllowUsersToUploadAvatars)
                {
                    tvchannelReviewModel.UserAvatarUrl = await _pictureService.GetPictureUrlAsync(
                        await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                foreach (var q in await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(pr.Id))
                {
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    tvchannelReviewModel.AdditionalTvChannelReviewList.Add(new TvChannelReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        TvChannelReviewId = pr.Id,
                        Rating = q.Rating,
                        Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                        VisibleToAllUsers = reviewType.VisibleToAllUsers || currentUser.Id == pr.UserId,
                    });
                }

                model.Items.Add(tvchannelReviewModel);
            }

            foreach (var rt in model.ReviewTypeList)
            {
                if (model.ReviewTypeList.Count <= model.AddAdditionalTvChannelReviewList.Count)
                    continue;
                var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(rt.Id);
                var reviewTypeMappingModel = new AddTvChannelReviewReviewTypeMappingModel
                {
                    ReviewTypeId = rt.Id,
                    Name = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Name),
                    Description = await _localizationService.GetLocalizedAsync(reviewType, entity => entity.Description),
                    DisplayOrder = rt.DisplayOrder,
                    IsRequired = rt.IsRequired,
                };

                model.AddAdditionalTvChannelReviewList.Add(reviewTypeMappingModel);
            }

            //Average rating
            foreach (var rtm in model.ReviewTypeList)
            {
                var totalRating = 0;
                var totalCount = 0;
                foreach (var item in model.Items)
                {
                    foreach (var q in item.AdditionalTvChannelReviewList.Where(w => w.ReviewTypeId == rtm.Id))
                    {
                        totalRating += q.Rating;
                        totalCount = ++totalCount;
                    }
                }

                rtm.AverageRating = (double)totalRating / (totalCount > 0 ? totalCount : 1);
            }

            model.AddTvChannelReview.CanCurrentUserLeaveReview = _catalogSettings.AllowAnonymousUsersToReviewTvChannel || !await _userService.IsGuestAsync(currentUser);
            model.AddTvChannelReview.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnTvChannelReviewPage;
            model.AddTvChannelReview.CanAddNewReview = await _tvchannelService.CanAddReviewAsync(tvchannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0);
            model.MetaKeywords = string.Format("{0},{1}", (await _localizationService.GetResourceAsync("PageTitle.TvChannelReviews")).Replace(' ', ',').ToLower(),
                                 tvChannelName.Replace(' ', ','));
            model.MetaDescription = string.Format(await _localizationService.GetResourceAsync("PageTitle.TvChannelReviews.MetaDescriptionFormat"), tvChannelName);

            return model;
        }

        /// <summary>
        /// Prepare the user tvchannel reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user tvchannel reviews model
        /// </returns>
        public virtual async Task<UserTvChannelReviewsModel> PrepareUserTvChannelReviewsModelAsync(int? page)
        {
            var pageSize = _catalogSettings.TvChannelReviewsPageSizeOnAccountPage;
            var pageIndex = 0;

            if (page > 0)
            {
                pageIndex = page.Value - 1;
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            var user = await _workContext.GetCurrentUserAsync();

            var list = await _tvchannelService.GetAllTvChannelReviewsAsync(
                userId: user.Id,
                approved: null,
                storeId: _catalogSettings.ShowTvChannelReviewsPerStore ? store.Id : 0,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var tvchannelReviews = new List<UserTvChannelReviewModel>();

            foreach (var review in list)
            {
                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(review.TvChannelId);

                var tvchannelReviewModel = new UserTvChannelReviewModel
                {
                    Title = review.Title,
                    TvChannelId = tvchannel.Id,
                    TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, p => p.Name),
                    TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(review.CreatedOnUtc, DateTimeKind.Utc)).ToString("g")
                };

                if (_catalogSettings.TvChannelReviewsMustBeApproved)
                {
                    tvchannelReviewModel.ApprovalStatus = review.IsApproved
                        ? await _localizationService.GetResourceAsync("Account.UserTvChannelReviews.ApprovalStatus.Approved")
                        : await _localizationService.GetResourceAsync("Account.UserTvChannelReviews.ApprovalStatus.Pending");
                }

                foreach (var q in await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(review.Id))
                {
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    tvchannelReviewModel.AdditionalTvChannelReviewList.Add(new TvChannelReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        TvChannelReviewId = review.Id,
                        Rating = q.Rating,
                        Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                    });
                }

                tvchannelReviews.Add(tvchannelReviewModel);
            }

            var pagerModel = new PagerModel(_localizationService)
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "UserTvChannelReviewsPaged",
                UseRouteLinks = true,
                RouteValues = new UserTvChannelReviewsModel.UserTvChannelReviewsRouteValues { PageNumber = pageIndex }
            };

            var model = new UserTvChannelReviewsModel
            {
                TvChannelReviews = tvchannelReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel email a friend model
        /// </summary>
        /// <param name="model">TvChannel email a friend model</param>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel email a friend model
        /// </returns>
        public virtual async Task<TvChannelEmailAFriendModel> PrepareTvChannelEmailAFriendModelAsync(TvChannelEmailAFriendModel model, TvChannel tvchannel, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            model.TvChannelId = tvchannel.Id;
            model.TvChannelName = await _localizationService.GetLocalizedAsync(tvchannel, x => x.Name);
            model.TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvchannel);
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailTvChannelToFriendPage;
            if (!excludeProperties)
            {
                var user = await _workContext.GetCurrentUserAsync();
                model.YourEmailAddress = user.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvchannel specification model
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel specification model
        /// </returns>
        public virtual async Task<TvChannelSpecificationModel> PrepareTvChannelSpecificationModelAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var model = new TvChannelSpecificationModel();

            // Add non-grouped attributes first
            model.Groups.Add(new TvChannelSpecificationAttributeGroupModel
            {
                Attributes = await PrepareTvChannelSpecificationAttributeModelAsync(tvchannel, null)
            });

            // Add grouped attributes
            var groups = await _specificationAttributeService.GetTvChannelSpecificationAttributeGroupsAsync(tvchannel.Id);
            foreach (var group in groups)
            {
                model.Groups.Add(new TvChannelSpecificationAttributeGroupModel
                {
                    Id = group.Id,
                    Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                    Attributes = await PrepareTvChannelSpecificationAttributeModelAsync(tvchannel, group)
                });
            }

            return model;
        }

        #endregion
    }
}
