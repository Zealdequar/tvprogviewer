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
    /// Represents the tvChannel model factory
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
        private readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;
        private readonly ITvChannelService _tvChannelService;
        private readonly ITvChannelTagService _tvChannelTagService;
        private readonly ITvChannelTemplateService _tvChannelTemplateService;
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
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            ITvChannelService tvChannelService,
            ITvChannelTagService tvChannelTagService,
            ITvChannelTemplateService tvChannelTemplateService,
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
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _tvChannelService = tvChannelService;
            _tvChannelTagService = tvChannelTagService;
            _tvChannelTemplateService = tvChannelTemplateService;
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
        /// Prepare the tvChannel specification models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="group">Specification attribute group</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannel specification model
        /// </returns>
        protected virtual async Task<IList<TvChannelSpecificationAttributeModel>> PrepareTvChannelSpecificationAttributeModelAsync(TvChannel tvChannel, SpecificationAttributeGroup group)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var tvChannelSpecificationAttributes = await _specificationAttributeService.GetTvChannelSpecificationAttributesAsync(
                    tvChannel.Id, specificationAttributeGroupId: group?.Id, showOnTvChannelPage: true);

            var result = new List<TvChannelSpecificationAttributeModel>();

            foreach (var psa in tvChannelSpecificationAttributes)
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
        /// Prepare the tvChannel review overview model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review overview model
        /// </returns>
        protected virtual async Task<TvChannelReviewOverviewModel> PrepareTvChannelReviewOverviewModelAsync(TvChannel tvChannel)
        {
            TvChannelReviewOverviewModel tvChannelReview;
            var currentStore = await _storeContext.GetCurrentStoreAsync();

            if (_catalogSettings.ShowTvChannelReviewsPerStore)
            {
                var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelReviewsModelKey, tvChannel, currentStore);

                tvChannelReview = await _staticCacheManager.GetAsync(cacheKey, async () =>
                {
                    var tvChannelReviews = await _tvChannelService.GetAllTvChannelReviewsAsync(tvChannelId: tvChannel.Id, approved: true, storeId: currentStore.Id);
                    
                    return new TvChannelReviewOverviewModel
                    {
                        RatingSum = tvChannelReviews.Sum(pr => pr.Rating),
                        TotalReviews = tvChannelReviews.Count
                    };
                });
            }
            else
            {
                tvChannelReview = new TvChannelReviewOverviewModel
                {
                    RatingSum = tvChannel.ApprovedRatingSum,
                    TotalReviews = tvChannel.ApprovedTotalReviews
                };
            }

            if (tvChannelReview != null)
            {
                tvChannelReview.TvChannelId = tvChannel.Id;
                tvChannelReview.AllowUserReviews = tvChannel.AllowUserReviews;
                tvChannelReview.CanAddNewReview = await _tvChannelService.CanAddReviewAsync(tvChannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0);
            }

            return tvChannelReview;
        }

        /// <summary>
        /// Prepare the tvChannel overview price model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel overview price model
        /// </returns>
        protected virtual async Task<TvChannelOverviewModel.TvChannelPriceModel> PrepareTvChannelOverviewPriceModelAsync(TvChannel tvChannel, bool forceRedirectionAfterAddingToCart = false)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var priceModel = new TvChannelOverviewModel.TvChannelPriceModel
            {
                ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
            };

            switch (tvChannel.TvChannelType)
            {
                case TvChannelType.GroupedTvChannel:
                    //grouped tvChannel
                    await PrepareGroupedTvChannelOverviewPriceModelAsync(tvChannel, priceModel);

                    break;
                case TvChannelType.SimpleTvChannel:
                default:
                    //simple tvChannel
                    await PrepareSimpleTvChannelOverviewPriceModelAsync(tvChannel, priceModel);

                    break;
            }

            return priceModel;
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task PrepareSimpleTvChannelOverviewPriceModelAsync(TvChannel tvChannel, TvChannelOverviewModel.TvChannelPriceModel priceModel)
        {
            //add to cart button
            priceModel.DisableBuyButton = tvChannel.DisableBuyButton ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                                          !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button
            priceModel.DisableWishlistButton = tvChannel.DisableWishlistButton ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                                               !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);
            //compare tvChannels
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareTvChannelsEnabled;

            //rental
            priceModel.IsRental = tvChannel.IsRental;

            //pre-order
            if (tvChannel.AvailableForPreOrder)
            {
                priceModel.AvailableForPreOrder = !tvChannel.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                                  tvChannel.PreOrderAvailabilityStartDateTimeUtc.Value >=
                                                  DateTime.UtcNow;
                priceModel.PreOrderAvailabilityStartDateTimeUtc = tvChannel.PreOrderAvailabilityStartDateTimeUtc;
            }

            //prices
            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                if (tvChannel.UserEntersPrice)
                    return;

                if (tvChannel.CallForPrice &&
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
                            .PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelMultiplePriceCacheKey, tvChannel, userRoleIds, store);
                        if (!_catalogSettings.CacheTvChannelPrices || tvChannel.IsRental)
                            cacheKey.CacheTime = 0;

                        var cachedPrice = await _staticCacheManager.GetAsync(cacheKey, async () =>
                        {
                            var prices = new List<(decimal PriceWithoutDiscount, decimal PriceWithDiscount)>();

                            // price when there are no required attributes
                            var attributesMappings = await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
                            if (!attributesMappings.Any(am => !am.IsNonCombinable() && am.IsRequired))
                            {
                                (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                    .GetFinalPriceAsync(tvChannel, user, store);
                                prices.Add((priceWithoutDiscount, priceWithDiscount));
                            }

                            var allAttributesXml = await _tvChannelAttributeParser.GenerateAllCombinationsAsync(tvChannel, true);
                            foreach (var attributesXml in allAttributesXml)
                            {
                                var warnings = new List<string>();
                                warnings.AddRange(await _shoppingCartService.GetShoppingCartItemAttributeWarningsAsync(user,
                                    ShoppingCartType.ShoppingCart, tvChannel, 1, attributesXml, true, true, true));
                                if (warnings.Count != 0)
                                    continue;

                                //get price with additional charge
                                var additionalCharge = decimal.Zero;
                                var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
                                if (combination?.OverriddenPrice.HasValue ?? false)
                                    additionalCharge = combination.OverriddenPrice.Value;
                                else
                                {
                                    var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
                                    foreach (var attributeValue in attributeValues)
                                    {
                                        additionalCharge += await _priceCalculationService.
                                            GetTvChannelAttributeValuePriceAdjustmentAsync(tvChannel, attributeValue, user, store);
                                    }
                                }

                                if (additionalCharge != decimal.Zero)
                                {
                                    (var priceWithoutDiscount, var priceWithDiscount, _, _) = await _priceCalculationService
                                        .GetFinalPriceAsync(tvChannel, user, store, additionalCharge);
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
                            (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store);
                            
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
                        (minPossiblePriceWithoutDiscount, minPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store);

                    if (tvChannel.HasTierPrices)
                    {
                        var (tierPriceMinPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithDiscount, _, _) = await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store, quantity: int.MaxValue);

                        //calculate price for the maximum quantity if we have tier prices, and choose minimal
                        minPossiblePriceWithoutDiscount = Math.Min(minPossiblePriceWithoutDiscount, tierPriceMinPossiblePriceWithoutDiscount);
                        minPossiblePriceWithDiscount = Math.Min(minPossiblePriceWithDiscount, tierPriceMinPossiblePriceWithDiscount);
                    }

                    var (oldPriceBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, tvChannel.OldPrice);
                    var (finalPriceWithoutDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, minPossiblePriceWithoutDiscount);
                    var (finalPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, minPossiblePriceWithDiscount);
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
                    var tierPrices = tvChannel.HasTierPrices
                        ? await _tvChannelService.GetTierPricesAsync(tvChannel, user, store)
                        : new List<TierPrice>();

                    //When there is just one tier price (with  qty 1), there are no actual savings in the list.
                    var hasTierPrices = tierPrices.Any() && !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);

                    var price = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                    priceModel.Price = hasTierPrices || hasMultiplePrices
                        ? string.Format(await _localizationService.GetResourceAsync("TvChannels.PriceRangeFrom"), price)
                        : price;
                    priceModel.PriceValue = finalPriceWithDiscount;

                    if (tvChannel.IsRental)
                    {
                        //rental tvChannel
                        priceModel.OldPrice = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvChannel, priceModel.OldPrice);
                        priceModel.OldPriceValue = priceModel.OldPriceValue;
                        priceModel.Price = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvChannel, priceModel.Price);
                        priceModel.PriceValue = priceModel.PriceValue;
                    }

                    //property for German market
                    //we display tax/shipping info only with "shipping enabled" for this tvChannel
                    //we also ensure this it's not free shipping
                    priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoTvChannelBoxes && tvChannel.IsShipEnabled && !tvChannel.IsFreeShipping;

                    //PAngV default baseprice (used in Germany)
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvChannel, finalPriceWithDiscount);
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
        protected virtual async Task PrepareGroupedTvChannelOverviewPriceModelAsync(TvChannel tvChannel, TvChannelOverviewModel.TvChannelPriceModel priceModel)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var associatedTvChannels = await _tvChannelService.GetAssociatedTvChannelsAsync(tvChannel.Id,
                store.Id);

            //add to cart button (ignore "DisableBuyButton" property for grouped tvChannels)
            priceModel.DisableBuyButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //add to wishlist button (ignore "DisableWishlistButton" property for grouped tvChannels)
            priceModel.DisableWishlistButton =
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices);

            //compare tvChannels
            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareTvChannelsEnabled;
            if (!associatedTvChannels.Any())
                return;

            //we have at least one associated tvChannel
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
                    priceModel.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvChannel, finalPriceBase);
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
        /// Prepare the tvChannel overview picture model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="tvChannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains picture models
        /// </returns>
        protected virtual async Task<IList<PictureModel>> PrepareTvChannelOverviewPicturesModelAsync(TvChannel tvChannel, int? tvChannelThumbPictureSize = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = tvChannelThumbPictureSize ?? _mediaSettings.TvChannelThumbPictureSize;

            //prepare picture model
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelOverviewPicturesModelKey, 
                tvChannel, pictureSize, true, _catalogSettings.DisplayAllPicturesOnCatalogPages, await _workContext.GetWorkingLanguageAsync(), 
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
                                tvChannelName),
                        //"alt" attribute
                        AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                            ? picture.AltAttribute
                            : string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat"),
                                tvChannelName)
                    };
                }

                //all pictures
                var pictures = (await _pictureService
                    .GetPicturesByTvChannelIdAsync(tvChannel.Id,  _catalogSettings.DisplayAllPicturesOnCatalogPages ? 0 : 1))
                    .DefaultIfEmpty(null);
                var pictureModels = await pictures
                    .SelectAwait(async picture => await preparePictureModelAsync(picture))
                    .ToListAsync();
                return pictureModels;
            });

            return cachedPictures;
        }

        /// <summary>
        /// Prepare the tvChannel breadcrumb model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel breadcrumb model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.TvChannelBreadcrumbModel> PrepareTvChannelBreadcrumbModelAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var breadcrumbModel = new TvChannelDetailsModel.TvChannelBreadcrumbModel
            {
                Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                TvChannelId = tvChannel.Id,
                TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel)
            };
            var tvChannelCategories = await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(tvChannel.Id);
            if (!tvChannelCategories.Any())
                return breadcrumbModel;

            var category = await _categoryService.GetCategoryByIdAsync(tvChannelCategories[0].CategoryId);
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
        /// Prepare the tvChannel tag models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannel tag model
        /// </returns>
        protected virtual async Task<IList<TvChannelTagModel>> PrepareTvChannelTagModelsAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var store = await _storeContext.GetCurrentStoreAsync();
            var tvChannelsTags = await _tvChannelTagService.GetAllTvChannelTagsByTvChannelIdAsync(tvChannel.Id);

            var model = await tvChannelsTags
                    //filter by store
                    .WhereAwait(async x => await _tvChannelTagService.GetTvChannelCountByTvChannelTagIdAsync(x.Id, store.Id) > 0)
                    .SelectAwait(async x => new TvChannelTagModel
                    {
                        Id = x.Id,
                        Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                        SeName = await _urlRecordService.GetSeNameAsync(x),
                        TvChannelCount = await _tvChannelTagService.GetTvChannelCountByTvChannelTagIdAsync(x.Id, store.Id)
                    }).ToListAsync();

            return model;
        }

        /// <summary>
        /// Prepare the tvChannel price model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel price model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.TvChannelPriceModel> PrepareTvChannelPriceModelAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var model = new TvChannelDetailsModel.TvChannelPriceModel
            {
                TvChannelId = tvChannel.Id
            };

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.HidePrices = false;
                if (tvChannel.UserEntersPrice)
                {
                    model.UserEntersPrice = true;
                }
                else
                {
                    if (tvChannel.CallForPrice &&
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

                        var (oldPriceBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, tvChannel.OldPrice);

                        var (finalPriceWithoutDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, (await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store, includeDiscounts: false)).finalPrice);
                        var (finalPriceWithDiscountBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, (await _priceCalculationService.GetFinalPriceAsync(tvChannel, user, store)).finalPrice);
                        
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
                        //we display tax/shipping info only with "shipping enabled" for this tvChannel
                        //we also ensure this it's not free shipping
                        model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoTvChannelDetailsPage
                            && tvChannel.IsShipEnabled &&
                            !tvChannel.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.BasePricePAngV = await _priceFormatter.FormatBasePriceAsync(tvChannel, finalPriceWithDiscountBase);
                        model.BasePricePAngVValue = finalPriceWithDiscountBase;
                        //currency code
                        model.CurrencyCode = currentCurrency.CurrencyCode;

                        //rental
                        if (tvChannel.IsRental)
                        {
                            model.IsRental = true;
                            var priceStr = await _priceFormatter.FormatPriceAsync(finalPriceWithDiscount);
                            model.RentalPrice = await _priceFormatter.FormatRentalTvChannelPeriodAsync(tvChannel, priceStr);
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
        /// Prepare the tvChannel add to cart model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel add to cart model
        /// </returns>
        protected virtual async Task<TvChannelDetailsModel.AddToCartModel> PrepareTvChannelAddToCartModelAsync(TvChannel tvChannel, ShoppingCartItem updatecartitem)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var model = new TvChannelDetailsModel.AddToCartModel
            {
                TvChannelId = tvChannel.Id
            };

            if (updatecartitem != null)
            {
                model.UpdatedShoppingCartItemId = updatecartitem.Id;
                model.UpdateShoppingCartItemType = updatecartitem.ShoppingCartType;
            }

            //quantity
            model.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : tvChannel.OrderMinimumQuantity;
            //allowed quantities
            var allowedQuantities = _tvChannelService.ParseAllowedQuantities(tvChannel);
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
            if (tvChannel.OrderMinimumQuantity > 1)
            {
                model.MinimumQuantityNotification = string.Format(await _localizationService.GetResourceAsync("TvChannels.MinimumQuantityNotification"), tvChannel.OrderMinimumQuantity);
            }

            //'add to cart', 'add to wishlist' buttons
            model.DisableBuyButton = tvChannel.DisableBuyButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart);
            model.DisableWishlistButton = tvChannel.DisableWishlistButton || !await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist);
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.DisableBuyButton = true;
                model.DisableWishlistButton = true;
            }
            //pre-order
            if (tvChannel.AvailableForPreOrder)
            {
                model.AvailableForPreOrder = !tvChannel.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    tvChannel.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.PreOrderAvailabilityStartDateTimeUtc = tvChannel.PreOrderAvailabilityStartDateTimeUtc;

                if (model.AvailableForPreOrder &&
                    model.PreOrderAvailabilityStartDateTimeUtc.HasValue &&
                    _catalogSettings.DisplayDatePreOrderAvailability)
                {
                    model.PreOrderAvailabilityStartDateTimeUserTime =
                        (await _dateTimeHelper.ConvertToUserTimeAsync(model.PreOrderAvailabilityStartDateTimeUtc.Value)).ToString("D");
                }
            }
            //rental
            model.IsRental = tvChannel.IsRental;

            //user entered price
            model.UserEntersPrice = tvChannel.UserEntersPrice;
            if (!model.UserEntersPrice)
                return model;

            var currentCurrency = await _workContext.GetWorkingCurrencyAsync();
            var minimumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvChannel.MinimumUserEnteredPrice, currentCurrency);
            var maximumUserEnteredPrice = await _currencyService.ConvertFromPrimaryStoreCurrencyAsync(tvChannel.MaximumUserEnteredPrice, currentCurrency);

            model.UserEnteredPrice = updatecartitem != null ? updatecartitem.UserEnteredPrice : minimumUserEnteredPrice;
            model.UserEnteredPriceRange = string.Format(await _localizationService.GetResourceAsync("TvChannels.EnterTvChannelPrice.Range"),
                await _priceFormatter.FormatPriceAsync(minimumUserEnteredPrice, false, false),
                await _priceFormatter.FormatPriceAsync(maximumUserEnteredPrice, false, false));

            return model;
        }

        /// <summary>
        /// Prepare the tvChannel attribute models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannel attribute model
        /// </returns>
        protected virtual async Task<IList<TvChannelDetailsModel.TvChannelAttributeModel>> PrepareTvChannelAttributeModelsAsync(TvChannel tvChannel, ShoppingCartItem updatecartitem)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var model = new List<TvChannelDetailsModel.TvChannelAttributeModel>();
            var store = updatecartitem != null ? await _storeService.GetStoreByIdAsync(updatecartitem.StoreId) : await _storeContext.GetCurrentStoreAsync();
            
            var tvChannelAttributeMapping = await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id);
            foreach (var attribute in tvChannelAttributeMapping)
            {
                var tvChannelAttrubute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(attribute.TvChannelAttributeId);

                var attributeModel = new TvChannelDetailsModel.TvChannelAttributeModel
                {
                    Id = attribute.Id,
                    TvChannelId = tvChannel.Id,
                    TvChannelAttributeId = attribute.TvChannelAttributeId,
                    Name = await _localizationService.GetLocalizedAsync(tvChannelAttrubute, x => x.Name),
                    Description = await _localizationService.GetLocalizedAsync(tvChannelAttrubute, x => x.Description),
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
                    var attributeValues = await _tvChannelAttributeService.GetTvChannelAttributeValuesAsync(attribute.Id);
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

                            var attributeValuePriceAdjustment = await _priceCalculationService.GetTvChannelAttributeValuePriceAdjustmentAsync(tvChannel, attributeValue, user, store, quantity: updatecartitem?.Quantity ?? 1);
                            var (priceAdjustmentBase, _) = await _taxService.GetTvChannelPriceAsync(tvChannel, attributeValuePriceAdjustment);
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
                            var tvChannelAttributeImageSquarePictureCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelAttributeImageSquarePictureModelKey
                                , attributeValue.ImageSquaresPictureId,
                                    _webHelper.IsCurrentConnectionSecured(),
                                    await _storeContext.GetCurrentStoreAsync());
                            valueModel.ImageSquaresPictureModel = await _staticCacheManager.GetAsync(tvChannelAttributeImageSquarePictureCacheKey, async () =>
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

                        //picture of a tvChannel attribute value
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
                                    var selectedValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(updatecartitem.AttributesXml);
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
                                    foreach (var attributeValue in (await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(updatecartitem.AttributesXml))
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
                                    var enteredText = _tvChannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Any())
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }

                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _tvChannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
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
                                    var downloadGuidStr = _tvChannelAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id).FirstOrDefault();
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
        /// Prepare the tvChannel tier price models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tier price model
        /// </returns>
        protected virtual async Task<IList<TvChannelDetailsModel.TierPriceModel>> PrepareTvChannelTierPriceModelsAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var user = await _workContext.GetCurrentUserAsync();
            var store = await _storeContext.GetCurrentStoreAsync();
            var model = await (await _tvChannelService.GetTierPricesAsync(tvChannel, user, store))
                .SelectAwait(async tierPrice =>
                {
                    var priceBase = (await _taxService.GetTvChannelPriceAsync(tvChannel, (await _priceCalculationService.GetFinalPriceAsync(tvChannel,
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
        /// Prepare the tvChannel manufacturer models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of manufacturer brief info model
        /// </returns>
        protected virtual async Task<IList<ManufacturerBriefInfoModel>> PrepareTvChannelManufacturerModelsAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var model = await (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(tvChannel.Id))
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
        /// Prepare the tvChannel details picture model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="isAssociatedTvChannel">Whether the tvChannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the picture model for the default picture; All picture models
        /// </returns>
        protected virtual async Task<(PictureModel pictureModel, IList<PictureModel> allPictureModels, IList<VideoModel> allVideoModels)> PrepareTvChannelDetailsPictureModelAsync(TvChannel tvChannel, bool isAssociatedTvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //default picture size
            var defaultPictureSize = isAssociatedTvChannel ?
                _mediaSettings.AssociatedTvChannelPictureSize :
                _mediaSettings.TvChannelDetailsPictureSize;

            //prepare picture models
            var tvChannelPicturesCacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgModelCacheDefaults.TvChannelDetailsPicturesModelKey
                , tvChannel, defaultPictureSize, isAssociatedTvChannel, 
                await _workContext.GetWorkingLanguageAsync(), _webHelper.IsCurrentConnectionSecured(), await _storeContext.GetCurrentStoreAsync());
            var cachedPictures = await _staticCacheManager.GetAsync(tvChannelPicturesCacheKey, async () =>
            {
                var tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name);

                var pictures = await _pictureService.GetPicturesByTvChannelIdAsync(tvChannel.Id);
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
                    string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvChannelName);
                //"alt" attribute
                defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                    defaultPicture.AltAttribute :
                    string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvChannelName);

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
                        Title = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvChannelName),
                        AlternateText = string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvChannelName),
                    };
                    //"title" attribute
                    pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                        picture.TitleAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageLinkTitleFormat.Details"), tvChannelName);
                    //"alt" attribute
                    pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                        picture.AltAttribute :
                        string.Format(await _localizationService.GetResourceAsync("Media.TvChannel.ImageAlternateTextFormat.Details"), tvChannelName);

                    pictureModels.Add(pictureModel);
                }

                return new { DefaultPictureModel = defaultPictureModel, PictureModels = pictureModels };
            });

            var allPictureModels = cachedPictures.PictureModels;
            
            //all videos
            var allvideoModels = new List<VideoModel>();
            var videos = await _videoService.GetVideosByTvChannelIdAsync(tvChannel.Id);
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
        /// Get the tvChannel template view path
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the view path
        /// </returns>
        public virtual async Task<string> PrepareTvChannelTemplateViewPathAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var template = await _tvChannelTemplateService.GetTvChannelTemplateByIdAsync(tvChannel.TvChannelTemplateId) ??
                           (await _tvChannelTemplateService.GetAllTvChannelTemplatesAsync()).FirstOrDefault();

            if (template == null)
                throw new Exception("No default template could be loaded");

            return template.ViewPath;
        }

        /// <summary>
        /// Prepare the tvChannel overview models
        /// </summary>
        /// <param name="tvChannels">Collection of tvChannels</param>
        /// <param name="preparePriceModel">Whether to prepare the price model</param>
        /// <param name="preparePictureModel">Whether to prepare the picture model</param>
        /// <param name="tvChannelThumbPictureSize">TvChannel thumb picture size (longest side); pass null to use the default value of media settings</param>
        /// <param name="prepareSpecificationAttributes">Whether to prepare the specification attribute models</param>
        /// <param name="forceRedirectionAfterAddingToCart">Whether to force redirection after adding to cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the collection of tvChannel overview model
        /// </returns>
        public virtual async Task<IEnumerable<TvChannelOverviewModel>> PrepareTvChannelOverviewModelsAsync(IEnumerable<TvChannel> tvChannels,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? tvChannelThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (tvChannels == null)
                throw new ArgumentNullException(nameof(tvChannels));

            var models = new List<TvChannelOverviewModel>();
            foreach (var tvChannel in tvChannels)
            {
                var model = new TvChannelOverviewModel
                {
                    Id = tvChannel.Id,
                    Name = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                    ShortDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.ShortDescription),
                    FullDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.FullDescription),
                    SeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                    Sku = tvChannel.Sku,
                    TvChannelType = tvChannel.TvChannelType,
                    MarkAsNew = tvChannel.MarkAsNew &&
                        (!tvChannel.MarkAsNewStartDateTimeUtc.HasValue || tvChannel.MarkAsNewStartDateTimeUtc.Value < DateTime.UtcNow) &&
                        (!tvChannel.MarkAsNewEndDateTimeUtc.HasValue || tvChannel.MarkAsNewEndDateTimeUtc.Value > DateTime.UtcNow)
                };

                //price
                if (preparePriceModel)
                {
                    model.TvChannelPrice = await PrepareTvChannelOverviewPriceModelAsync(tvChannel, forceRedirectionAfterAddingToCart);
                }

                //picture
                if (preparePictureModel)
                {
                    model.PictureModels = await PrepareTvChannelOverviewPicturesModelAsync(tvChannel, tvChannelThumbPictureSize);
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.TvChannelSpecificationModel = await PrepareTvChannelSpecificationModelAsync(tvChannel);
                }

                //reviews
                model.ReviewOverviewModel = await PrepareTvChannelReviewOverviewModelAsync(tvChannel);

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Prepare the tvChannel combination models
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel combination models
        /// </returns>
        public virtual async Task<IList<TvChannelCombinationModel>> PrepareTvChannelCombinationModelsAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var result = new List<TvChannelCombinationModel>();

            var combinations = await _tvChannelAttributeService
                .GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);
            if (combinations?.Any() == true)
            {
                foreach (var combination in combinations)
                {
                    var combinationModel = new TvChannelCombinationModel
                    {
                        InStock = combination.StockQuantity > 0 || combination.AllowOutOfStockOrders
                    };

                    var mappings = await _tvChannelAttributeParser
                        .ParseTvChannelAttributeMappingsAsync(combination.AttributesXml);
                    if (mappings == null || mappings.Count == 0)
                        continue;

                    foreach (var mapping in mappings)
                    {
                        var attributeModel = new TvChannelAttributeModel
                        {
                            Id = mapping.Id
                        };

                        var values = await _tvChannelAttributeParser
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
        /// Prepare the tvChannel details model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="updatecartitem">Updated shopping cart item</param>
        /// <param name="isAssociatedTvChannel">Whether the tvChannel is associated</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel details model
        /// </returns>
        public virtual async Task<TvChannelDetailsModel> PrepareTvChannelDetailsModelAsync(TvChannel tvChannel,
            ShoppingCartItem updatecartitem = null, bool isAssociatedTvChannel = false)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            //standard properties
            var model = new TvChannelDetailsModel
            {
                Id = tvChannel.Id,
                Name = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name),
                ShortDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.ShortDescription),
                FullDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.FullDescription),
                MetaKeywords = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaKeywords),
                MetaDescription = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaDescription),
                MetaTitle = await _localizationService.GetLocalizedAsync(tvChannel, x => x.MetaTitle),
                SeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                TvChannelType = tvChannel.TvChannelType,
                ShowSku = _catalogSettings.ShowSkuOnTvChannelDetailsPage,
                Sku = tvChannel.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = tvChannel.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = tvChannel.Gtin,
                ManageInventoryMethod = tvChannel.ManageInventoryMethod,
                StockAvailability = await _tvChannelService.FormatStockMessageAsync(tvChannel, string.Empty),
                HasSampleDownload = tvChannel.IsDownload && tvChannel.HasSampleDownload,
                DisplayDiscontinuedMessage = !tvChannel.Published && _catalogSettings.DisplayDiscontinuedMessageForUnpublishedTvChannels,
                AvailableEndDate = tvChannel.AvailableEndDateTimeUtc,
                VisibleIndividually = tvChannel.VisibleIndividually,
                AllowAddingOnlyExistingAttributeCombinations = tvChannel.AllowAddingOnlyExistingAttributeCombinations
            };

            //automatically generate tvChannel description?
            if (_seoSettings.GenerateTvChannelMetaDescription && string.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = tvChannel.IsShipEnabled;
            if (tvChannel.IsShipEnabled)
            {
                model.IsFreeShipping = tvChannel.IsFreeShipping;
                //delivery date
                var deliveryDate = await _dateRangeService.GetDeliveryDateByIdAsync(tvChannel.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = await _localizationService.GetLocalizedAsync(deliveryDate, dd => dd.Name);
                }
            }

            var store = await _storeContext.GetCurrentStoreAsync();
            //tvChannel live url
            model.TvChannelLiveUrlEnabled = _catalogSettings.TvChannelLiveUrlEnabled;
            model.TvChannelLiveUrl = tvChannel.TvChannelLiveUrl;
            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare tvChannels
            model.CompareTvChannelsEnabled = _catalogSettings.CompareTvChannelsEnabled;
            //store name
            model.CurrentStoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name);

            //vendor details
            if (_vendorSettings.ShowVendorOnTvChannelDetailsPage)
            {
                var vendor = await _vendorService.GetVendorByIdAsync(tvChannel.VendorId);
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

            switch (tvChannel.ManageInventoryMethod)
            {
                case ManageInventoryMethod.DontManageStock:
                    model.InStock = true;
                    break;

                case ManageInventoryMethod.ManageStock:
                    model.InStock = tvChannel.BackorderMode != BackorderMode.NoBackorders
                        || await _tvChannelService.GetTotalStockQuantityAsync(tvChannel) > 0;
                    model.DisplayBackInStockSubscription = !model.InStock && tvChannel.AllowBackInStockSubscriptions;
                    break;

                case ManageInventoryMethod.ManageStockByAttributes:
                    model.InStock = (await _tvChannelAttributeService
                        .GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id))
                        ?.Any(c => c.StockQuantity > 0 || c.AllowOutOfStockOrders)
                        ?? false;
                    break;
            }

            //breadcrumb
            //do not prepare this model for the associated tvChannels. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedTvChannel)
            {
                model.Breadcrumb = await PrepareTvChannelBreadcrumbModelAsync(tvChannel);
            }

            //tvChannel tags
            //do not prepare this model for the associated tvChannels. anyway it's not used
            if (!isAssociatedTvChannel)
            {
                model.TvChannelTags = await PrepareTvChannelTagModelsAsync(tvChannel);
            }

            //pictures and videos
            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            IList<PictureModel> allPictureModels;
            IList<VideoModel> allVideoModels;
            (model.DefaultPictureModel, allPictureModels, allVideoModels) = await PrepareTvChannelDetailsPictureModelAsync(tvChannel, isAssociatedTvChannel);
            model.PictureModels = allPictureModels;
            model.VideoModels = allVideoModels;

            //price
            model.TvChannelPrice = await PrepareTvChannelPriceModelAsync(tvChannel);

            //'Add to cart' model
            model.AddToCart = await PrepareTvChannelAddToCartModelAsync(tvChannel, updatecartitem);
            var user = await _workContext.GetCurrentUserAsync();
            //gift card
            if (tvChannel.IsGiftCard)
            {
                model.GiftCard.IsGiftCard = true;
                model.GiftCard.GiftCardType = tvChannel.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = await _userService.GetUserFullNameAsync(user);
                    model.GiftCard.SenderEmail = user.Email;
                }
                else
                {
                    _tvChannelAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out var giftCardRecipientName, out var giftCardRecipientEmail,
                        out var giftCardSenderName, out var giftCardSenderEmail, out var giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            //tvChannel attributes
            model.TvChannelAttributes = await PrepareTvChannelAttributeModelsAsync(tvChannel, updatecartitem);

            //tvChannel specifications
            //do not prepare this model for the associated tvChannels. anyway it's not used
            if (!isAssociatedTvChannel)
            {
                model.TvChannelSpecificationModel = await PrepareTvChannelSpecificationModelAsync(tvChannel);
            }

            //tvChannel review overview
            model.TvChannelReviewOverview = await PrepareTvChannelReviewOverviewModelAsync(tvChannel);

            //tier prices
            if (tvChannel.HasTierPrices && await _permissionService.AuthorizeAsync(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = await PrepareTvChannelTierPriceModelsAsync(tvChannel);
            }

            model.TvTypeProgSelector = await _commonFactory.PrepareTvTypeProgSelectorModelAsync();

            var typeProg = model.TvTypeProgSelector.CurrentTypeProgId;

            model.TvChannelDays = await _programmeService.GetDaysAsync(typeProg);

            //manufacturers
            model.TvChannelManufacturers = await PrepareTvChannelManufacturerModelsAsync(tvChannel);

            //rental tvChannels
            if (tvChannel.IsRental)
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
                    TvChannelId = tvChannel.Id,
                    CreatedOnUtc = DateTime.UtcNow
                };

                var estimateShippingModel = await _shoppingCartModelFactory.PrepareEstimateShippingModelAsync(new[] { wrappedTvChannel });

                model.TvChannelEstimateShipping.TvChannelId = tvChannel.Id;
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

            //associated tvChannels
            if (tvChannel.TvChannelType == TvChannelType.GroupedTvChannel)
            {
                //ensure no circular references
                if (!isAssociatedTvChannel)
                {
                    var associatedTvChannels = await _tvChannelService.GetAssociatedTvChannelsAsync(tvChannel.Id, store.Id);
                    foreach (var associatedTvChannel in associatedTvChannels)
                        model.AssociatedTvChannels.Add(await PrepareTvChannelDetailsModelAsync(associatedTvChannel, null, true));
                }
                model.InStock = model.AssociatedTvChannels.Any(associatedTvChannel => associatedTvChannel.InStock);
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvChannel reviews model
        /// </summary>
        /// <param name="model">TvChannel reviews model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel reviews model
        /// </returns>
        public virtual async Task<TvChannelReviewsModel> PrepareTvChannelReviewsModelAsync(TvChannelReviewsModel model, TvChannel tvChannel)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            model.TvChannelId = tvChannel.Id;
            string tvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name);
            model.TvChannelName = tvChannelName;
            model.TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel);

            var currentStore = await _storeContext.GetCurrentStoreAsync();

            var tvChannelReviews = await _tvChannelService.GetAllTvChannelReviewsAsync(
                approved: true, 
                tvChannelId: tvChannel.Id,
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
            foreach (var pr in tvChannelReviews)
            {
                var user = await _userService.GetUserByIdAsync(pr.UserId);

                var tvChannelReviewModel = new TvChannelReviewModel
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
                    tvChannelReviewModel.UserAvatarUrl = await _pictureService.GetPictureUrlAsync(
                        await _genericAttributeService.GetAttributeAsync<int>(user, TvProgUserDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize, _userSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                }

                foreach (var q in await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(pr.Id))
                {
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    tvChannelReviewModel.AdditionalTvChannelReviewList.Add(new TvChannelReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        TvChannelReviewId = pr.Id,
                        Rating = q.Rating,
                        Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                        VisibleToAllUsers = reviewType.VisibleToAllUsers || currentUser.Id == pr.UserId,
                    });
                }

                model.Items.Add(tvChannelReviewModel);
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
            model.AddTvChannelReview.CanAddNewReview = await _tvChannelService.CanAddReviewAsync(tvChannel.Id, _catalogSettings.ShowTvChannelReviewsPerStore ? currentStore.Id : 0);
            model.MetaKeywords = string.Format("{0},{1}", (await _localizationService.GetResourceAsync("PageTitle.TvChannelReviews")).Replace(' ', ',').ToLower(),
                                 tvChannelName.Replace(' ', ','));
            model.MetaDescription = string.Format(await _localizationService.GetResourceAsync("PageTitle.TvChannelReviews.MetaDescriptionFormat"), tvChannelName);

            return model;
        }

        /// <summary>
        /// Prepare the user tvChannel reviews model
        /// </summary>
        /// <param name="page">Number of items page; pass null to load the first page</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the user tvChannel reviews model
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

            var list = await _tvChannelService.GetAllTvChannelReviewsAsync(
                userId: user.Id,
                approved: null,
                storeId: _catalogSettings.ShowTvChannelReviewsPerStore ? store.Id : 0,
                pageIndex: pageIndex,
                pageSize: pageSize);

            var tvChannelReviews = new List<UserTvChannelReviewModel>();

            foreach (var review in list)
            {
                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(review.TvChannelId);

                var tvChannelReviewModel = new UserTvChannelReviewModel
                {
                    Title = review.Title,
                    TvChannelId = tvChannel.Id,
                    TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, p => p.Name),
                    TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel),
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    ReplyText = review.ReplyText,
                    WrittenOnStr = (await _dateTimeHelper.ConvertToUserTimeAsync(review.CreatedOnUtc, DateTimeKind.Utc)).ToString("g")
                };

                if (_catalogSettings.TvChannelReviewsMustBeApproved)
                {
                    tvChannelReviewModel.ApprovalStatus = review.IsApproved
                        ? await _localizationService.GetResourceAsync("Account.UserTvChannelReviews.ApprovalStatus.Approved")
                        : await _localizationService.GetResourceAsync("Account.UserTvChannelReviews.ApprovalStatus.Pending");
                }

                foreach (var q in await _reviewTypeService.GetTvChannelReviewReviewTypeMappingsByTvChannelReviewIdAsync(review.Id))
                {
                    var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(q.ReviewTypeId);

                    tvChannelReviewModel.AdditionalTvChannelReviewList.Add(new TvChannelReviewReviewTypeMappingModel
                    {
                        ReviewTypeId = q.ReviewTypeId,
                        TvChannelReviewId = review.Id,
                        Rating = q.Rating,
                        Name = await _localizationService.GetLocalizedAsync(reviewType, x => x.Name),
                    });
                }

                tvChannelReviews.Add(tvChannelReviewModel);
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
                TvChannelReviews = tvChannelReviews,
                PagerModel = pagerModel
            };

            return model;
        }

        /// <summary>
        /// Prepare the tvChannel email a friend model
        /// </summary>
        /// <param name="model">TvChannel email a friend model</param>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel email a friend model
        /// </returns>
        public virtual async Task<TvChannelEmailAFriendModel> PrepareTvChannelEmailAFriendModelAsync(TvChannelEmailAFriendModel model, TvChannel tvChannel, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            model.TvChannelId = tvChannel.Id;
            model.TvChannelName = await _localizationService.GetLocalizedAsync(tvChannel, x => x.Name);
            model.TvChannelSeName = await _urlRecordService.GetSeNameAsync(tvChannel);
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnEmailTvChannelToFriendPage;
            if (!excludeProperties)
            {
                var user = await _workContext.GetCurrentUserAsync();
                model.YourEmailAddress = user.Email;
            }

            return model;
        }

        /// <summary>
        /// Prepare the tvChannel specification model
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel specification model
        /// </returns>
        public virtual async Task<TvChannelSpecificationModel> PrepareTvChannelSpecificationModelAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var model = new TvChannelSpecificationModel();

            // Add non-grouped attributes first
            model.Groups.Add(new TvChannelSpecificationAttributeGroupModel
            {
                Attributes = await PrepareTvChannelSpecificationAttributeModelAsync(tvChannel, null)
            });

            // Add grouped attributes
            var groups = await _specificationAttributeService.GetTvChannelSpecificationAttributeGroupsAsync(tvChannel.Id);
            foreach (var group in groups)
            {
                model.Groups.Add(new TvChannelSpecificationAttributeGroupModel
                {
                    Id = group.Id,
                    Name = await _localizationService.GetLocalizedAsync(group, x => x.Name),
                    Attributes = await PrepareTvChannelSpecificationAttributeModelAsync(tvChannel, group)
                });
            }

            return model;
        }

        #endregion
    }
}
