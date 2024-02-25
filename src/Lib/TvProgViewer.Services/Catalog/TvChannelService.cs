using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Shipping.Date;
using TvProgViewer.Services.Stores;
using Microsoft.CodeAnalysis.Operations;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel service
    /// </summary>
    public partial class TvChannelService : ITvChannelService
    {
        #region Fields

        protected readonly CatalogSettings _catalogSettings;
        protected readonly CommonSettings _commonSettings;
        protected readonly IAclService _aclService;
        protected readonly IUserService _userService;
        protected readonly IDateRangeService _dateRangeService;
        protected readonly ILanguageService _languageService;
        protected readonly ILocalizationService _localizationService;
        protected readonly ITvChannelAttributeParser _tvchannelAttributeParser;
        protected readonly ITvChannelAttributeService _tvchannelAttributeService;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IRepository<CrossSellTvChannel> _crossSellTvChannelRepository;
        protected readonly IRepository<DiscountTvChannelMapping> _discountTvChannelMappingRepository;
        protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        protected readonly IRepository<Manufacturer> _manufacturerRepository;
        protected readonly IRepository<TvChannel> _tvchannelRepository;
        protected readonly IRepository<TvChannelAttributeCombination> _tvchannelAttributeCombinationRepository;
        protected readonly IRepository<TvChannelAttributeMapping> _tvchannelAttributeMappingRepository;
        protected readonly IRepository<TvChannelCategory> _tvchannelCategoryRepository;
        protected readonly IRepository<TvChannelManufacturer> _tvchannelManufacturerRepository;
        protected readonly IRepository<TvChannelPicture> _tvchannelPictureRepository;
        protected readonly IRepository<TvChannelTvChannelTagMapping> _tvchannelTagMappingRepository;
        protected readonly IRepository<TvChannelReview> _tvchannelReviewRepository;
        protected readonly IRepository<TvChannelReviewHelpfulness> _tvchannelReviewHelpfulnessRepository;
        protected readonly IRepository<TvChannelSpecificationAttribute> _tvchannelSpecificationAttributeRepository;
        protected readonly IRepository<TvChannelTag> _tvchannelTagRepository;
        protected readonly IRepository<TvChannelVideo> _tvchannelVideoRepository;
        protected readonly IRepository<TvChannelWarehouseInventory> _tvchannelWarehouseInventoryRepository;
        protected readonly IRepository<RelatedTvChannel> _relatedTvChannelRepository;
        protected readonly IRepository<Shipment> _shipmentRepository;
        protected readonly IRepository<StockQuantityHistory> _stockQuantityHistoryRepository;
        protected readonly IRepository<TierPrice> _tierPriceRepository;
        protected readonly ISearchPluginManager _searchPluginManager;
        protected readonly IStaticCacheManager _staticCacheManager;
        protected readonly IStoreMappingService _storeMappingService;
        protected readonly IStoreService _storeService;
        protected readonly IWorkContext _workContext;
        protected readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public TvChannelService(CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            IAclService aclService,
            IUserService userService,
            IDateRangeService dateRangeService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ITvChannelAttributeParser tvchannelAttributeParser,
            ITvChannelAttributeService tvchannelAttributeService,
            IRepository<Category> categoryRepository,
            IRepository<CrossSellTvChannel> crossSellTvChannelRepository,
            IRepository<DiscountTvChannelMapping> discountTvChannelMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<TvChannel> tvchannelRepository,
            IRepository<TvChannelAttributeCombination> tvchannelAttributeCombinationRepository,
            IRepository<TvChannelAttributeMapping> tvchannelAttributeMappingRepository,
            IRepository<TvChannelCategory> tvchannelCategoryRepository,
            IRepository<TvChannelManufacturer> tvchannelManufacturerRepository,
            IRepository<TvChannelPicture> tvchannelPictureRepository,
            IRepository<TvChannelTvChannelTagMapping> tvchannelTagMappingRepository,
            IRepository<TvChannelReview> tvchannelReviewRepository,
            IRepository<TvChannelReviewHelpfulness> tvchannelReviewHelpfulnessRepository,
            IRepository<TvChannelSpecificationAttribute> tvchannelSpecificationAttributeRepository,
            IRepository<TvChannelTag> tvchannelTagRepository,
            IRepository<TvChannelVideo> tvchannelVideoRepository,
            IRepository<TvChannelWarehouseInventory> tvchannelWarehouseInventoryRepository,
            IRepository<RelatedTvChannel> relatedTvChannelRepository,
            IRepository<Shipment> shipmentRepository,
            IRepository<StockQuantityHistory> stockQuantityHistoryRepository,
            IRepository<TierPrice> tierPriceRepository,
            ISearchPluginManager searchPluginManager,
            IStaticCacheManager staticCacheManager,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkContext workContext,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _aclService = aclService;
            _userService = userService;
            _dateRangeService = dateRangeService;
            _languageService = languageService;
            _localizationService = localizationService;
            _tvchannelAttributeParser = tvchannelAttributeParser;
            _tvchannelAttributeService = tvchannelAttributeService;
            _categoryRepository = categoryRepository;
            _crossSellTvChannelRepository = crossSellTvChannelRepository;
            _discountTvChannelMappingRepository = discountTvChannelMappingRepository;
            _localizedPropertyRepository = localizedPropertyRepository;
            _manufacturerRepository = manufacturerRepository;
            _tvchannelRepository = tvchannelRepository;
            _tvchannelAttributeCombinationRepository = tvchannelAttributeCombinationRepository;
            _tvchannelAttributeMappingRepository = tvchannelAttributeMappingRepository;
            _tvchannelCategoryRepository = tvchannelCategoryRepository;
            _tvchannelManufacturerRepository = tvchannelManufacturerRepository;
            _tvchannelPictureRepository = tvchannelPictureRepository;
            _tvchannelTagMappingRepository = tvchannelTagMappingRepository;
            _tvchannelReviewRepository = tvchannelReviewRepository;
            _tvchannelReviewHelpfulnessRepository = tvchannelReviewHelpfulnessRepository;
            _tvchannelSpecificationAttributeRepository = tvchannelSpecificationAttributeRepository;
            _tvchannelTagRepository = tvchannelTagRepository;
            _tvchannelVideoRepository = tvchannelVideoRepository;
            _tvchannelWarehouseInventoryRepository = tvchannelWarehouseInventoryRepository;
            _relatedTvChannelRepository = relatedTvChannelRepository;
            _shipmentRepository = shipmentRepository;
            _stockQuantityHistoryRepository = stockQuantityHistoryRepository;
            _tierPriceRepository = tierPriceRepository;
            _searchPluginManager = searchPluginManager;
            _staticCacheManager = staticCacheManager;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Applies the low stock activity to specified tvchannel by the total stock quantity
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="totalStock">Total stock</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ApplyLowStockActivityAsync(TvChannel tvchannel, int totalStock)
        {
            var isMinimumStockReached = totalStock <= tvchannel.MinStockQuantity;

            if (!isMinimumStockReached && !_catalogSettings.PublishBackTvChannelWhenCancellingOrders)
                return;

            switch (tvchannel.LowStockActivity)
            {
                case LowStockActivity.DisableBuyButton:
                    tvchannel.DisableBuyButton = isMinimumStockReached;
                    tvchannel.DisableWishlistButton = isMinimumStockReached;
                    await UpdateTvChannelAsync(tvchannel);
                    break;

                case LowStockActivity.Unpublish:
                    tvchannel.Published = !isMinimumStockReached;
                    await UpdateTvChannelAsync(tvchannel);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets SKU, Manufacturer part number and GTIN
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU, Manufacturer part number, GTIN
        /// </returns>
        protected virtual async Task<(string sku, string manufacturerPartNumber, string gtin)> GetSkuMpnGtinAsync(TvChannel tvchannel, string attributesXml)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            if (!string.IsNullOrEmpty(attributesXml) &&
                tvchannel.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                //manage stock by attribute combinations
                //let's find appropriate record
                var combination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
                if (combination != null)
                {
                    sku = combination.Sku;
                    manufacturerPartNumber = combination.ManufacturerPartNumber;
                    gtin = combination.Gtin;
                }
            }

            if (string.IsNullOrEmpty(sku))
                sku = tvchannel.Sku;
            if (string.IsNullOrEmpty(manufacturerPartNumber))
                manufacturerPartNumber = tvchannel.ManufacturerPartNumber;
            if (string.IsNullOrEmpty(gtin))
                gtin = tvchannel.Gtin;

            return (sku, manufacturerPartNumber, gtin);
        }

        /// <summary>
        /// Get stock message for a tvchannel with attributes
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageForAttributesAsync(TvChannel tvchannel, string attributesXml)
        {
            if (!tvchannel.DisplayStockAvailability)
                return string.Empty;

            string stockMessage;

            var combination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
            if (combination != null)
            {
                //combination exists
                var stockQuantity = combination.StockQuantity;
                if (stockQuantity > 0)
                {
                    if (tvchannel.MinStockQuantity >= stockQuantity && tvchannel.LowStockActivity == LowStockActivity.Nothing)
                    {
                        stockMessage = tvchannel.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.LowStock");
                    }
                    else
                    {
                        stockMessage = tvchannel.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                    }
                }
                else
                {
                    if (combination.AllowOutOfStockOrders)
                    {
                        stockMessage = await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                    }
                    else
                    {
                        var tvchannelAvailabilityRange = await
                            _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvchannel.TvChannelAvailabilityRangeId);
                        stockMessage = tvchannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(tvchannelAvailabilityRange, range => range.Name));
                    }
                }
            }
            else
            {
                //no combination configured
                if (tvchannel.AllowAddingOnlyExistingAttributeCombinations)
                {
                    var allIds = (await _tvchannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvchannel.Id)).Where(pa => pa.IsRequired).Select(pa => pa.Id).ToList();
                    var exIds = (await _tvchannelAttributeParser.ParseTvChannelAttributeMappingsAsync(attributesXml)).Select(pa => pa.Id).ToList();

                    var selectedIds = allIds.Intersect(exIds).ToList();

                    if (selectedIds.Count() != allIds.Count)
                        if (_catalogSettings.AttributeValueOutOfStockDisplayType == AttributeValueOutOfStockDisplayType.AlwaysDisplay)
                            return await _localizationService.GetResourceAsync("TvChannels.Availability.SelectRequiredAttributes");
                        else
                        {
                            var combinations = await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id);

                            combinations = combinations.Where(p => p.StockQuantity >= 0 || p.AllowOutOfStockOrders).ToList();

                            var attributes = await combinations.SelectAwait(async c => await _tvchannelAttributeParser.ParseTvChannelAttributeMappingsAsync(c.AttributesXml)).ToListAsync();

                            var flag = attributes.SelectMany(a => a).Any(a => selectedIds.Contains(a.Id));

                            if (flag)
                                return await _localizationService.GetResourceAsync("TvChannels.Availability.SelectRequiredAttributes");
                        }

                    var tvchannelAvailabilityRange = await
                        _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvchannel.TvChannelAvailabilityRangeId);
                    stockMessage = tvchannelAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(tvchannelAvailabilityRange, range => range.Name));
                }
                else
                {
                    stockMessage = await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Get stock message
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageAsync(TvChannel tvchannel)
        {
            if (!tvchannel.DisplayStockAvailability)
                return string.Empty;

            var stockMessage = string.Empty;
            var stockQuantity = await GetTotalStockQuantityAsync(tvchannel);

            if (stockQuantity > 0)
            {
                if (tvchannel.MinStockQuantity >= stockQuantity && tvchannel.LowStockActivity == LowStockActivity.Nothing)
                {
                    stockMessage = tvchannel.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.LowStock");
                }
                else
                {
                    stockMessage = tvchannel.DisplayStockQuantity
                        ?
                        //display "in stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.InStockWithQuantity"), stockQuantity)
                        :
                        //display "in stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                }
            }
            else
            {
                //out of stock
                var tvchannelAvailabilityRange = await _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvchannel.TvChannelAvailabilityRangeId);
                switch (tvchannel.BackorderMode)
                {
                    case BackorderMode.NoBackorders:
                        stockMessage = tvchannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(tvchannelAvailabilityRange, range => range.Name));
                        break;
                    case BackorderMode.AllowQtyBelow0:
                        stockMessage = await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                        break;
                    case BackorderMode.AllowQtyBelow0AndNotifyUser:
                        stockMessage = tvchannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.Backordering")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.BackorderingWithDate"),
                                await _localizationService.GetLocalizedAsync(tvchannelAvailabilityRange, range => range.Name));
                        break;
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Reserve the given quantity in the warehouses.
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ReserveInventoryAsync(TvChannel tvchannel, int quantity)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            var qty = -quantity;

            var tvchannelInventory = _tvchannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvchannel.Id)
                .OrderByDescending(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                .ToList();

            if (tvchannelInventory.Count <= 0)
                return;

            // 1st pass: Applying reserved
            foreach (var item in tvchannelInventory)
            {
                var selectQty = Math.Min(Math.Max(0, item.StockQuantity - item.ReservedQuantity), qty);
                item.ReservedQuantity += selectQty;
                qty -= selectQty;

                if (qty <= 0)
                    break;
            }

            if (qty > 0)
            {
                // 2rd pass: Booking negative stock!
                var pwi = tvchannelInventory[0];
                pwi.ReservedQuantity += qty;
            }

            await UpdateTvChannelWarehouseInventoryAsync(tvchannelInventory);
        }

        /// <summary>
        /// Unblocks the given quantity reserved items in the warehouses
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="quantity">Quantity, must be positive</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task UnblockReservedInventoryAsync(TvChannel tvchannel, int quantity)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (quantity < 0)
                throw new ArgumentException("Value must be positive.", nameof(quantity));

            var tvchannelInventory = await _tvchannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvchannel.Id)
                .OrderByDescending(pwi => pwi.ReservedQuantity)
                .ThenByDescending(pwi => pwi.StockQuantity)
                .ToListAsync();

            if (!tvchannelInventory.Any())
                return;

            var qty = quantity;

            foreach (var item in tvchannelInventory)
            {
                var selectQty = Math.Min(item.ReservedQuantity, qty);
                item.ReservedQuantity -= selectQty;
                qty -= selectQty;

                if (qty <= 0)
                    break;
            }

            if (qty > 0)
            {
                var pwi = tvchannelInventory[0];
                pwi.StockQuantity += qty;
            }

            await UpdateTvChannelWarehouseInventoryAsync(tvchannelInventory);
        }

        /// <summary>
        /// Gets cross-sell tvchannels by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelIds">The first tvchannel identifiers</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannels
        /// </returns>
        protected virtual async Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelIdsAsync(int[] tvchannelIds, bool showHidden = false)
        {
            if (tvchannelIds == null || tvchannelIds.Length == 0)
                return new List<CrossSellTvChannel>();

            var query = from csp in _crossSellTvChannelRepository.Table
                        join p in _tvchannelRepository.Table on csp.TvChannelId2 equals p.Id
                        where tvchannelIds.Contains(csp.TvChannelId1) &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby csp.Id
                        select csp;
            var crossSellTvChannels = await query.ToListAsync();

            return crossSellTvChannels;
        }

        /// <summary>
        /// Gets ratio of useful and not useful tvchannel reviews 
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<(int usefulCount, int notUsefulCount)> GetHelpfulnessCountsAsync(TvChannelReview tvchannelReview)
        {
            if (tvchannelReview is null)
                throw new ArgumentNullException(nameof(tvchannelReview));

            var tvchannelReviewHelpfulness = _tvchannelReviewHelpfulnessRepository.Table.Where(prh => prh.TvChannelReviewId == tvchannelReview.Id);

            return (await tvchannelReviewHelpfulness.CountAsync(prh => prh.WasHelpful),
                await tvchannelReviewHelpfulness.CountAsync(prh => !prh.WasHelpful));
        }

        /// <summary>
        /// Inserts a tvchannel review helpfulness record
        /// </summary>
        /// <param name="tvchannelReviewHelpfulness">TvChannel review helpfulness record</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertTvChannelReviewHelpfulnessAsync(TvChannelReviewHelpfulness tvchannelReviewHelpfulness)
        {
            await _tvchannelReviewHelpfulnessRepository.InsertAsync(tvchannelReviewHelpfulness);
        }

        #endregion

        #region Methods

        #region TvChannels

        /// <summary>
        /// Delete a tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAsync(TvChannel tvchannel)
        {
            await _tvchannelRepository.DeleteAsync(tvchannel);
        }

        /// <summary>
        /// Delete tvchannels
        /// </summary>
        /// <param name="tvchannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelsAsync(IList<TvChannel> tvchannels)
        {
            await _tvchannelRepository.DeleteAsync(tvchannels);
        }

        /// <summary>
        /// Получение всех телеканалов
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронные операции
        /// Результат задачи содержит телеканалы деталей
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetAllTvChannelsAsync()
        {
            var tvchannels = await _tvchannelRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where !p.Deleted
                       select p;
            });

            return tvchannels;
        }

        /// <summary>
        /// Gets all tvchannels displayed on the home page
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetAllTvChannelsDisplayedOnHomepageAsync()
        {
            var tvchannels = await _tvchannelRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted &&
                             p.ShowOnHomepage
                       select p;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelsHomepageCacheKey));

            return tvchannels;
        }

        /// <summary>
        /// Gets tvchannel
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel
        /// </returns>
        public virtual async Task<TvChannel> GetTvChannelByIdAsync(int tvchannelId)
        {
            return await _tvchannelRepository.GetByIdAsync(tvchannelId, cache => default);
        }

        /// <summary>
        /// Get tvchannels by identifiers
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetTvChannelsByIdsAsync(int[] tvchannelIds)
        {
            return await _tvchannelRepository.GetByIdsAsync(tvchannelIds, cache => default, false);
        }

        /// <summary>
        /// Inserts a tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAsync(TvChannel tvchannel)
        {
            await _tvchannelRepository.InsertAsync(tvchannel);
        }

        /// <summary>
        /// Обновление телеканала детали
        /// </summary>
        /// <param name="tvchannel">Телеканал детали</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAsync(TvChannel tvchannel)
        {
            await _tvchannelRepository.UpdateAsync(tvchannel);
        }

        /// <summary>
        /// Обновление телеканалов деталей
        /// </summary>
        /// <param name="tvChannels">Список телеканалов деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelListAsync(IList<TvChannel> tvChannels)
        {
            await _tvchannelRepository.UpdateAsync(tvChannels);
        }

        /// <summary>
        /// Gets featured tvchannels by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvchannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetCategoryFeaturedTvChannelsAsync(int categoryId, int storeId = 0)
        {
            IList<TvChannel> featuredTvChannels = new List<TvChannel>();

            if (categoryId == 0)
                return featuredTvChannels;

            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.CategoryFeaturedTvChannelsIdsKey, categoryId, userRoleIds, storeId);

            var featuredTvChannelIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from p in _tvchannelRepository.Table
                            join pc in _tvchannelCategoryRepository.Table on p.Id equals pc.TvChannelId
                            where p.Published && !p.Deleted && p.VisibleIndividually &&
                                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                                pc.IsFeaturedTvChannel && categoryId == pc.CategoryId
                            select p;

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, userRoleIds);

                featuredTvChannels = query.ToList();

                return featuredTvChannels.Select(p => p.Id).ToList();
            });

            if (featuredTvChannels.Count == 0 && featuredTvChannelIds.Count > 0)
                featuredTvChannels = await _tvchannelRepository.GetByIdsAsync(featuredTvChannelIds, cache => default, false);

            return featuredTvChannels;
        }

        /// <summary>
        /// Gets featured tvchannels by manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvchannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetManufacturerFeaturedTvChannelsAsync(int manufacturerId, int storeId = 0)
        {
            IList<TvChannel> featuredTvChannels = new List<TvChannel>();

            if (manufacturerId == 0)
                return featuredTvChannels;

            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.ManufacturerFeaturedTvChannelIdsKey, manufacturerId, userRoleIds, storeId);

            var featuredTvChannelIds = await _staticCacheManager.GetAsync(cacheKey, async () =>
            {
                var query = from p in _tvchannelRepository.Table
                            join pm in _tvchannelManufacturerRepository.Table on p.Id equals pm.TvChannelId
                            where p.Published && !p.Deleted && p.VisibleIndividually &&
                                (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                                (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow) &&
                                pm.IsFeaturedTvChannel && manufacturerId == pm.ManufacturerId
                            select p;

                //apply store mapping constraints
                query = await _storeMappingService.ApplyStoreMapping(query, storeId);

                //apply ACL constraints
                query = await _aclService.ApplyAcl(query, userRoleIds);

                featuredTvChannels = query.ToList();

                return featuredTvChannels.Select(p => p.Id).ToList();
            });

            if (featuredTvChannels.Count == 0 && featuredTvChannelIds.Count > 0)
                featuredTvChannels = await _tvchannelRepository.GetByIdsAsync(featuredTvChannelIds, cache => default, false);

            return featuredTvChannels;
        }

        /// <summary>
        /// Gets tvchannels which marked as new
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of new tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _tvchannelRepository.Table
                        where p.Published && p.VisibleIndividually && p.MarkAsNew && !p.Deleted &&
                            DateTime.UtcNow >= (p.MarkAsNewStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.MarkAsNewEndDateTimeUtc ?? DateTime.MaxValue)
                        select p;

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var user = await _workContext.GetCurrentUserAsync();
            query = await _aclService.ApplyAcl(query, user);

            query = query.OrderByDescending(p => p.CreatedOnUtc);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Get number of tvchannel (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvchannels
        /// </returns>
        public virtual async Task<int> GetNumberOfTvChannelsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _tvchannelRepository.Table.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);

            //apply store mapping constraints
            query = await _storeMappingService.ApplyStoreMapping(query, storeId);

            //apply ACL constraints
            var user = await _workContext.GetCurrentUserAsync();
            var userRoleIds = await _userService.GetUserRoleIdsAsync(user);
            query = await _aclService.ApplyAcl(query, userRoleIds);

            //category filtering
            if (categoryIds != null && categoryIds.Any())
            {
                query = from p in query
                        join pc in _tvchannelCategoryRepository.Table on p.Id equals pc.TvChannelId
                        where categoryIds.Contains(pc.CategoryId)
                        select p;
            }

            var cacheKey = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.CategoryTvChannelsNumberCacheKey, userRoleIds, storeId, categoryIds);

            //only distinct tvchannels
            return await _staticCacheManager.GetAsync(cacheKey, () => query.Select(p => p.Id).Count());
        }

        /// <summary>
        /// Search tvchannels
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerIds">Manufacturer identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="tvchannelType">TvChannel type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only tvchannels marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="excludeFeaturedTvChannels">A value indicating whether loaded tvchannels are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured tvchannels from results</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="tvchannelTagId">TvChannel tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in tvchannel descriptions</param>
        /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in tvchannel SKU</param>
        /// <param name="searchTvChannelTags">A value indicating whether to search by a specified "keyword" in tvchannel tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecOptions">Specification options list to filter tvchannels; null to load all records</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" tvchannels
        /// false - load only "Unpublished" tvchannels
        /// </param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> SearchTvChannelsAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> manufacturerIds = null,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            TvChannelType? tvchannelType = null,
            bool visibleIndividuallyOnly = false,
            bool excludeFeaturedTvChannels = false,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int tvchannelTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchManufacturerPartNumber = true,
            bool searchSku = true,
            bool searchTvChannelTags = false,
            int languageId = 0,
            IList<SpecificationAttributeOption> filteredSpecOptions = null,
            TvChannelSortingEnum orderBy = TvChannelSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {
            //some databases don't support int.MaxValue
            if (pageSize == int.MaxValue)
                pageSize = int.MaxValue - 1;

            var tvchannelsQuery = _tvchannelRepository.Table;

            if (!showHidden)
                tvchannelsQuery = tvchannelsQuery.Where(p => p.Published);
            else if (overridePublished.HasValue)
                tvchannelsQuery = tvchannelsQuery.Where(p => p.Published == overridePublished.Value);

            if (!showHidden)
            {
                //apply store mapping constraints
                tvchannelsQuery = await _storeMappingService.ApplyStoreMapping(tvchannelsQuery, storeId);

                //apply ACL constraints
                var user = await _workContext.GetCurrentUserAsync();
                tvchannelsQuery = await _aclService.ApplyAcl(tvchannelsQuery, user);
            }

            tvchannelsQuery =
                from p in tvchannelsQuery
                where !p.Deleted &&
                    (!visibleIndividuallyOnly || p.VisibleIndividually) &&
                    (vendorId == 0 || p.VendorId == vendorId) &&
                    (
                        warehouseId == 0 ||
                        (
                            !p.UseMultipleWarehouses ? p.WarehouseId == warehouseId :
                                _tvchannelWarehouseInventoryRepository.Table.Any(pwi => pwi.WarehouseId == warehouseId && pwi.TvChannelId == p.Id)
                        )
                    ) &&
                    (tvchannelType == null || p.TvChannelTypeId == (int)tvchannelType) &&
                    (showHidden ||
                            DateTime.UtcNow >= (p.AvailableStartDateTimeUtc ?? DateTime.MinValue) &&
                            DateTime.UtcNow <= (p.AvailableEndDateTimeUtc ?? DateTime.MaxValue)
                    ) &&
                    (priceMin == null || p.Price >= priceMin) &&
                    (priceMax == null || p.Price <= priceMax)
                select p;

            if (!string.IsNullOrEmpty(keywords))
            {
                var langs = await _languageService.GetAllLanguagesAsync(showHidden: true);

                //Set a flag which will to points need to search in localized properties. If showHidden doesn't set to true should be at least two published languages.
                var searchLocalizedValue = languageId > 0 && langs.Count >= 2 && (showHidden || langs.Count(l => l.Published) >= 2);
                IQueryable<int> tvchannelsByKeywords;

                var user = await _workContext.GetCurrentUserAsync();
                var activeSearchProvider = await _searchPluginManager.LoadPrimaryPluginAsync(user, storeId);

                if (activeSearchProvider is not null)
                {
                    tvchannelsByKeywords = (await activeSearchProvider.SearchTvChannelsAsync(keywords, searchLocalizedValue)).AsQueryable();
                }
                else
                {
                    tvchannelsByKeywords =
                        from p in _tvchannelRepository.Table
                        where p.Name.Contains(keywords) ||
                            (searchDescriptions &&
                                (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                            (searchManufacturerPartNumber && p.ManufacturerPartNumber == keywords) ||
                            (searchSku && p.Sku == keywords)
                        select p.Id;

                    if (searchLocalizedValue)
                    {
                        tvchannelsByKeywords = tvchannelsByKeywords.Union(
                            from lp in _localizedPropertyRepository.Table
                            let checkName = lp.LocaleKey == nameof(TvChannel.Name) &&
                                            lp.LocaleValue.Contains(keywords)
                            let checkShortDesc = searchDescriptions &&
                                            lp.LocaleKey == nameof(TvChannel.ShortDescription) &&
                                            lp.LocaleValue.Contains(keywords)
                            where
                                lp.LocaleKeyGroup == nameof(TvChannel) && lp.LanguageId == languageId && (checkName || checkShortDesc)

                            select lp.EntityId);
                    }
                }

                //search by SKU for TvChannelAttributeCombination
                if (searchSku)
                {
                    tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pac in _tvchannelAttributeCombinationRepository.Table
                        where pac.Sku == keywords
                        select pac.TvChannelId);
                }

                //search by category name if admin allows
                if (_catalogSettings.AllowUsersToSearchWithCategoryName)
                {
                    tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pc in _tvchannelCategoryRepository.Table
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where c.Name.Contains(keywords)
                        select pc.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pc in _tvchannelCategoryRepository.Table
                        join lp in _localizedPropertyRepository.Table on pc.CategoryId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Category) &&
                              lp.LocaleKey == nameof(Category.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pc.TvChannelId);
                    }
                }

                //search by manufacturer name if admin allows
                if (_catalogSettings.AllowUsersToSearchWithManufacturerName)
                {
                    tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pm in _tvchannelManufacturerRepository.Table
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where m.Name.Contains(keywords)
                        select pm.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pm in _tvchannelManufacturerRepository.Table
                        join lp in _localizedPropertyRepository.Table on pm.ManufacturerId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(Manufacturer) &&
                              lp.LocaleKey == nameof(Manufacturer.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pm.TvChannelId);
                    }
                }

                if (searchTvChannelTags)
                {
                    tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pptm in _tvchannelTagMappingRepository.Table
                        join pt in _tvchannelTagRepository.Table on pptm.TvChannelTagId equals pt.Id
                        where pt.Name.Contains(keywords)
                        select pptm.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvchannelsByKeywords = tvchannelsByKeywords.Union(
                        from pptm in _tvchannelTagMappingRepository.Table
                        join lp in _localizedPropertyRepository.Table on pptm.TvChannelTagId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(TvChannelTag) &&
                              lp.LocaleKey == nameof(TvChannelTag.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pptm.TvChannelId);
                    }
                }

                tvchannelsQuery =
                    from p in tvchannelsQuery
                    join pbk in tvchannelsByKeywords on p.Id equals pbk
                    select p;
            }

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var tvchannelCategoryQuery =
                        from pc in _tvchannelCategoryRepository.Table
                        where (!excludeFeaturedTvChannels || !pc.IsFeaturedTvChannel) &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.TvChannelId into pc
                        select new
                        {
                            TvChannelId = pc.Key,
                            DisplayOrder = pc.First().DisplayOrder
                        };

                    tvchannelsQuery =
                        from p in tvchannelsQuery
                        join pc in tvchannelCategoryQuery on p.Id equals pc.TvChannelId
                        orderby pc.DisplayOrder, p.Name
                        select p;
                }
            }

            if (manufacturerIds is not null)
            {
                if (manufacturerIds.Contains(0))
                    manufacturerIds.Remove(0);

                if (manufacturerIds.Any())
                {
                    var tvchannelManufacturerQuery =
                        from pm in _tvchannelManufacturerRepository.Table
                        where (!excludeFeaturedTvChannels || !pm.IsFeaturedTvChannel) &&
                            manufacturerIds.Contains(pm.ManufacturerId)
                        group pm by pm.TvChannelId into pm
                        select new
                        {
                            TvChannelId = pm.Key,
                            DisplayOrder = pm.First().DisplayOrder
                        };

                    tvchannelsQuery =
                        from p in tvchannelsQuery
                        join pm in tvchannelManufacturerQuery on p.Id equals pm.TvChannelId
                        orderby pm.DisplayOrder, p.Name
                        select p;
                }
            }

            if (tvchannelTagId > 0)
            {
                tvchannelsQuery =
                    from p in tvchannelsQuery
                    join ptm in _tvchannelTagMappingRepository.Table on p.Id equals ptm.TvChannelId
                    where ptm.TvChannelTagId == tvchannelTagId
                    select p;
            }

            if (filteredSpecOptions?.Count > 0)
            {
                var specificationAttributeIds = filteredSpecOptions
                    .Select(sao => sao.SpecificationAttributeId)
                    .Distinct();

                foreach (var specificationAttributeId in specificationAttributeIds)
                {
                    var optionIdsBySpecificationAttribute = filteredSpecOptions
                        .Where(o => o.SpecificationAttributeId == specificationAttributeId)
                        .Select(o => o.Id);

                    var tvchannelSpecificationQuery =
                        from psa in _tvchannelSpecificationAttributeRepository.Table
                        where psa.AllowFiltering && optionIdsBySpecificationAttribute.Contains(psa.SpecificationAttributeOptionId)
                        select psa;

                    tvchannelsQuery =
                        from p in tvchannelsQuery
                        where tvchannelSpecificationQuery.Any(pc => pc.TvChannelId == p.Id)
                        select p;
                }
            }

            return await tvchannelsQuery.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets tvchannels by tvchannel attribute
        /// </summary>
        /// <param name="tvchannelAttributeId">TvChannel attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsByTvChannelAttributeIdAsync(int tvchannelAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _tvchannelRepository.Table
                        join pam in _tvchannelAttributeMappingRepository.Table on p.Id equals pam.TvChannelId
                        where
                            pam.TvChannelAttributeId == tvchannelAttributeId &&
                            !p.Deleted
                        orderby p.Name
                        select p;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets associated tvchannels
        /// </summary>
        /// <param name="parentGroupedTvChannelId">Parent tvchannel identifier (used with grouped tvchannels)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetAssociatedTvChannelsAsync(int parentGroupedTvChannelId,
            int storeId = 0, int vendorId = 0, bool showHidden = false)
        {
            var query = _tvchannelRepository.Table;
            query = query.Where(x => x.ParentGroupedTvChannelId == parentGroupedTvChannelId);
            if (!showHidden)
            {
                query = query.Where(x => x.Published);

                //available dates
                query = query.Where(p =>
                    (!p.AvailableStartDateTimeUtc.HasValue || p.AvailableStartDateTimeUtc.Value < DateTime.UtcNow) &&
                    (!p.AvailableEndDateTimeUtc.HasValue || p.AvailableEndDateTimeUtc.Value > DateTime.UtcNow));
            }
            //vendor filtering
            if (vendorId > 0)
            {
                query = query.Where(p => p.VendorId == vendorId);
            }

            query = query.Where(x => !x.Deleted);
            query = query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id);

            var tvchannels = await query.ToListAsync();

            //ACL mapping
            if (!showHidden)
                tvchannels = await tvchannels.WhereAwait(async x => await _aclService.AuthorizeAsync(x)).ToListAsync();

            //Store mapping
            if (!showHidden && storeId > 0)
                tvchannels = await tvchannels.WhereAwait(async x => await _storeMappingService.AuthorizeAsync(x, storeId)).ToListAsync();

            return tvchannels;
        }

        /// <summary>
        /// Update tvchannel review totals
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelReviewTotalsAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var approvedRatingSum = 0;
            var notApprovedRatingSum = 0;
            var approvedTotalReviews = 0;
            var notApprovedTotalReviews = 0;

            var reviews = await _tvchannelReviewRepository.Table
                .Where(r => r.TvChannelId == tvchannel.Id)
                .ToListAsync();
            foreach (var pr in reviews)
                if (pr.IsApproved)
                {
                    approvedRatingSum += pr.Rating;
                    approvedTotalReviews++;
                }
                else
                {
                    notApprovedRatingSum += pr.Rating;
                    notApprovedTotalReviews++;
                }

            tvchannel.ApprovedRatingSum = approvedRatingSum;
            tvchannel.NotApprovedRatingSum = notApprovedRatingSum;
            tvchannel.ApprovedTotalReviews = approvedTotalReviews;
            tvchannel.NotApprovedTotalReviews = notApprovedTotalReviews;
            await UpdateTvChannelAsync(tvchannel);
        }

        /// <summary>
        /// Get low stock tvchannels
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load published tvchannels only; pass null to load all tvchannels, pass true to load only published tvchannels, pass false to load only unpublished tvchannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetLowStockTvChannelsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _tvchannelRepository.Table;

            //filter by tvchannels with tracking inventory
            query = query.Where(tvchannel => tvchannel.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStock);

            //filter by tvchannels with stock quantity less than the minimum
            query = query.Where(tvchannel =>
                (tvchannel.UseMultipleWarehouses ? _tvchannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvchannel.Id).Sum(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                    : tvchannel.StockQuantity) <= tvchannel.MinStockQuantity);

            //ignore deleted tvchannels
            query = query.Where(tvchannel => !tvchannel.Deleted);

            //ignore grouped tvchannels
            query = query.Where(tvchannel => tvchannel.TvChannelTypeId != (int)TvChannelType.GroupedTvChannel);

            //filter by vendor
            if (vendorId.HasValue && vendorId.Value > 0)
                query = query.Where(tvchannel => tvchannel.VendorId == vendorId.Value);

            //whether to load published tvchannels only
            if (loadPublishedOnly.HasValue)
                query = query.Where(tvchannel => tvchannel.Published == loadPublishedOnly.Value);

            query = query.OrderBy(tvchannel => tvchannel.MinStockQuantity).ThenBy(tvchannel => tvchannel.DisplayOrder).ThenBy(tvchannel => tvchannel.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get low stock tvchannel combinations
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load combinations of published tvchannels only; pass null to load all tvchannels, pass true to load only published tvchannels, pass false to load only unpublished tvchannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel combinations
        /// </returns>
        public virtual async Task<IPagedList<TvChannelAttributeCombination>> GetLowStockTvChannelCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var combinations = from pac in _tvchannelAttributeCombinationRepository.Table
                               join p in _tvchannelRepository.Table on pac.TvChannelId equals p.Id
                               where
                                   //filter by combinations with stock quantity less than the minimum
                                   pac.StockQuantity <= pac.MinStockQuantity &&
                                   //filter by tvchannels with tracking inventory by attributes
                                   p.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                                   //ignore deleted tvchannels
                                   !p.Deleted &&
                                   //ignore grouped tvchannels
                                   p.TvChannelTypeId != (int)TvChannelType.GroupedTvChannel &&
                                   //filter by vendor
                                   ((vendorId ?? 0) == 0 || p.VendorId == vendorId) &&
                                   //whether to load published tvchannels only
                                   (loadPublishedOnly == null || p.Published == loadPublishedOnly)
                               orderby pac.TvChannelId, pac.Id
                               select pac;

            return await combinations.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Gets a tvchannel by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel
        /// </returns>
        public virtual async Task<TvChannel> GetTvChannelBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _tvchannelRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var tvchannel = await query.FirstOrDefaultAsync();

            return tvchannel;
        }

        /// <summary>
        /// Gets a tvchannels by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        public async Task<IList<TvChannel>> GetTvChannelsBySkuAsync(string[] skuArray, int vendorId = 0)
        {
            if (skuArray == null)
                throw new ArgumentNullException(nameof(skuArray));

            var query = _tvchannelRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            if (vendorId != 0)
                query = query.Where(p => p.VendorId == vendorId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateHasTierPricesPropertyAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            tvchannel.HasTierPrices = (await GetTierPricesByTvChannelAsync(tvchannel.Id)).Any();
            await UpdateTvChannelAsync(tvchannel);
        }

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateHasDiscountsAppliedAsync(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            tvchannel.HasDiscountsApplied = _discountTvChannelMappingRepository.Table.Any(dpm => dpm.EntityId == tvchannel.Id);
            await UpdateTvChannelAsync(tvchannel);
        }

        /// <summary>
        /// Gets number of tvchannels by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvchannels
        /// </returns>
        public async Task<int> GetNumberOfTvChannelsByVendorIdAsync(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            return await _tvchannelRepository.Table.CountAsync(p => p.VendorId == vendorId && !p.Deleted);
        }

        /// <summary>
        /// Parse "required tvchannel Ids" property
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>A list of required tvchannel IDs</returns>
        public virtual int[] ParseRequiredTvChannelIds(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (string.IsNullOrEmpty(tvchannel.RequiredTvChannelIds))
                return Array.Empty<int>();

            var ids = new List<int>();

            foreach (var idStr in tvchannel.RequiredTvChannelIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
                if (int.TryParse(idStr, out var id))
                    ids.Add(id);

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a tvchannel is available now (availability dates)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool TvChannelIsAvailable(TvChannel tvchannel, DateTime? dateTime = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            dateTime ??= DateTime.UtcNow;

            if (tvchannel.AvailableStartDateTimeUtc.HasValue && tvchannel.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (tvchannel.AvailableEndDateTimeUtc.HasValue && tvchannel.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
        }

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Result</returns>
        public virtual int[] ParseAllowedQuantities(TvChannel tvchannel)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var result = new List<int>();
            if (!string.IsNullOrWhiteSpace(tvchannel.AllowedQuantities))
            {
                var quantities = tvchannel.AllowedQuantities
                   .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .ToList();
                foreach(var qtyStr in quantities)
                {
                    if (int.TryParse(qtyStr.Trim(), out var qty))
                        result.Add(qty);
                }
            }    

            return result.ToArray();
        }

        /// <summary>
        /// Get total quantity
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="useReservedQuantity">
        /// A value indicating whether we should consider "Reserved Quantity" property 
        /// when "multiple warehouses" are used
        /// </param>
        /// <param name="warehouseId">
        /// Warehouse identifier. Used to limit result to certain warehouse.
        /// Used only with "multiple warehouses" enabled.
        /// </param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<int> GetTotalStockQuantityAsync(TvChannel tvchannel, bool useReservedQuantity = true, int warehouseId = 0)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (tvchannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                //We can calculate total stock quantity when 'Manage inventory' property is set to 'Track inventory'
                return 0;

            if (!tvchannel.UseMultipleWarehouses)
                return tvchannel.StockQuantity;

            var pwi = _tvchannelWarehouseInventoryRepository.Table.Where(wi => wi.TvChannelId == tvchannel.Id);

            if (warehouseId > 0)
                pwi = pwi.Where(x => x.WarehouseId == warehouseId);

            var result = await pwi.SumAsync(x => x.StockQuantity);
            if (useReservedQuantity)
                result -= await pwi.SumAsync(x => x.ReservedQuantity);

            return result;
        }

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        public virtual int GetRentalPeriods(TvChannel tvchannel, DateTime startDate, DateTime endDate)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (!tvchannel.IsRental)
                return 1;

            if (startDate.CompareTo(endDate) >= 0)
                return 1;

            int totalPeriods;
            switch (tvchannel.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = tvchannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                case RentalPricePeriod.Weeks:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 7 * tvchannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                case RentalPricePeriod.Months:
                    {
                        //Source: http://stackoverflow.com/questions/4638993/difference-in-months-between-two-dates
                        var totalMonthsToRent = (endDate.Year - startDate.Year) * 12 + endDate.Month - startDate.Month;
                        if (startDate.AddMonths(totalMonthsToRent) < endDate)
                            //several days added (not full month)
                            totalMonthsToRent++;

                        var configuredPeriodMonths = tvchannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
                    }

                    break;
                case RentalPricePeriod.Years:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 365 * tvchannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                default:
                    throw new Exception("Not supported rental period");
            }

            return totalPeriods;
        }

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Selected tvchannel attributes in XML format (if specified)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock message
        /// </returns>
        public virtual async Task<string> FormatStockMessageAsync(TvChannel tvchannel, string attributesXml)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var stockMessage = string.Empty;

            switch (tvchannel.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockMessage = await GetStockMessageAsync(tvchannel);
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    stockMessage = await GetStockMessageForAttributesAsync(tvchannel, attributesXml);
                    break;
            }

            return stockMessage;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU
        /// </returns>
        public virtual async Task<string> FormatSkuAsync(TvChannel tvchannel, string attributesXml = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var (sku, _, _) = await GetSkuMpnGtinAsync(tvchannel, attributesXml);

            return sku;
        }

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer part number
        /// </returns>
        public virtual async Task<string> FormatMpnAsync(TvChannel tvchannel, string attributesXml = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var (_, manufacturerPartNumber, _) = await GetSkuMpnGtinAsync(tvchannel, attributesXml);

            return manufacturerPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gTIN
        /// </returns>
        public virtual async Task<string> FormatGtinAsync(TvChannel tvchannel, string attributesXml = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var (_, _, gtin) = await GetSkuMpnGtinAsync(tvchannel, attributesXml);

            return gtin;
        }

        /// <summary>
        /// Formats start/end date for rental tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public virtual string FormatRentalDate(TvChannel tvchannel, DateTime date)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (!tvchannel.IsRental)
                return null;

            return date.ToShortDateString();
        }

        /// <summary>
        /// Update tvchannel store mappings
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelStoreMappingsAsync(TvChannel tvchannel, IList<int> limitedToStoresIds)
        {
            tvchannel.LimitedToStores = limitedToStoresIds.Any();

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(tvchannel);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(tvchannel, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
                }
            }
        }

        /// <summary>
        /// Gets the value whether the sequence contains downloadable tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyDownloadableTvChannelAsync(int[] tvchannelIds)
        {
            return await _tvchannelRepository.Table
                .AnyAsync(p => tvchannelIds.Contains(p.Id) && p.IsDownload);
        }

        /// <summary>
        /// Gets the value whether the sequence contains gift card tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyGiftCardTvChannelAsync(int[] tvchannelIds)
        {
            return await _tvchannelRepository.Table
                .AnyAsync(p => tvchannelIds.Contains(p.Id) && p.IsGiftCard);
        }

        /// <summary>
        /// Gets the value whether the sequence contains recurring tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyRecurringTvChannelAsync(int[] tvchannelIds)
        {
            return await _tvchannelRepository.Table
                .AnyAsync(p => tvchannelIds.Contains(p.Id) && p.IsRecurring);
        }

        /// <summary>
        /// Returns a list of sku of not existing tvchannels
        /// </summary>
        /// <param name="tvchannelSku">The sku of the tvchannels to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of sku not existing tvchannels
        /// </returns>
        public virtual async Task<string[]> GetNotExistingTvChannelsAsync(string[] tvchannelSku)
        {
            if (tvchannelSku == null)
                throw new ArgumentNullException(nameof(tvchannelSku));

            var query = _tvchannelRepository.Table;
            var queryFilter = tvchannelSku.Distinct().ToArray();
            //filtering by SKU
            var filter = await query.Select(p => p.Sku)
                .Where(p => queryFilter.Contains(p))
                .ToListAsync();

            return queryFilter.Except(filter).ToArray();
        }

        #endregion

        #region Inventory management methods

        /// <summary>
        /// Adjust inventory
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="quantityToChange">Quantity to increase or decrease</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AdjustInventoryAsync(TvChannel tvchannel, int quantityToChange, string attributesXml = "", string message = "")
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (quantityToChange == 0)
                return;

            if (tvchannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                //update stock quantity
                if (tvchannel.UseMultipleWarehouses)
                {
                    //use multiple warehouses
                    if (quantityToChange < 0)
                        await ReserveInventoryAsync(tvchannel, quantityToChange);
                    else
                        await UnblockReservedInventoryAsync(tvchannel, quantityToChange);
                }
                else
                {
                    //do not use multiple warehouses
                    //simple inventory management
                    tvchannel.StockQuantity += quantityToChange;
                    await UpdateTvChannelAsync(tvchannel);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(tvchannel, quantityToChange, tvchannel.StockQuantity, tvchannel.WarehouseId, message);
                }

                var totalStock = await GetTotalStockQuantityAsync(tvchannel);

                await ApplyLowStockActivityAsync(tvchannel, totalStock);

                //send email notification
                if (quantityToChange < 0 && totalStock < tvchannel.NotifyAdminForQuantityBelow)
                {
                    //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                    var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                    await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(tvchannel, _localizationSettings.DefaultAdminLanguageId);
                }
            }

            if (tvchannel.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                var combination = await _tvchannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvchannel, attributesXml);
                if (combination != null)
                {
                    combination.StockQuantity += quantityToChange;
                    await _tvchannelAttributeService.UpdateTvChannelAttributeCombinationAsync(combination);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(tvchannel, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

                    if (tvchannel.AllowAddingOnlyExistingAttributeCombinations)
                    {
                        var totalStockByAllCombinations = await (await _tvchannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvchannel.Id))
                            .ToAsyncEnumerable()
                            .SumAsync(c => c.StockQuantity);

                        await ApplyLowStockActivityAsync(tvchannel, totalStockByAllCombinations);
                    }

                    //send email notification
                    if (quantityToChange < 0 && combination.StockQuantity < combination.NotifyAdminForQuantityBelow)
                    {
                        //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                        var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                        await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(combination, _localizationSettings.DefaultAdminLanguageId);
                    }
                }
            }

            //bundled tvchannels
            var attributeValues = await _tvchannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToTvChannel)
                    continue;

                //associated tvchannel (bundle)
                var associatedTvChannel = await GetTvChannelByIdAsync(attributeValue.AssociatedTvChannelId);
                if (associatedTvChannel != null)
                {
                    await AdjustInventoryAsync(associatedTvChannel, quantityToChange * attributeValue.Quantity, message);
                }
            }
        }

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task BookReservedInventoryAsync(TvChannel tvchannel, int warehouseId, int quantity, string message = "")
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            //only tvchannels with "use multiple warehouses" are handled this way
            if (tvchannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !tvchannel.UseMultipleWarehouses)
                return;

            var pwi = await _tvchannelWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.TvChannelId == tvchannel.Id && wi.WarehouseId == warehouseId);
            if (pwi == null)
                return;

            pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
            pwi.StockQuantity += quantity;

            await UpdateTvChannelWarehouseInventoryAsync(pwi);

            //quantity change history
            await AddStockQuantityHistoryEntryAsync(tvchannel, quantity, pwi.StockQuantity, warehouseId, message);
        }

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="tvchannel">tvchannel</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the quantity reversed
        /// </returns>
        public virtual async Task<int> ReverseBookedInventoryAsync(TvChannel tvchannel, ShipmentItem shipmentItem, string message = "")
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            //only tvchannels with "use multiple warehouses" are handled this way
            if (tvchannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !tvchannel.UseMultipleWarehouses)
                return 0;

            var pwi = await _tvchannelWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.TvChannelId == tvchannel.Id && wi.WarehouseId == shipmentItem.WarehouseId);
            if (pwi == null)
                return 0;

            var shipment = await _shipmentRepository.GetByIdAsync(shipmentItem.ShipmentId, cache => default);

            //not shipped yet? hence "BookReservedInventory" method was not invoked
            if (!shipment.ShippedDateUtc.HasValue)
                return 0;

            var qty = shipmentItem.Quantity;

            pwi.StockQuantity += qty;
            pwi.ReservedQuantity += qty;

            await UpdateTvChannelWarehouseInventoryAsync(pwi);

            //quantity change history
            await AddStockQuantityHistoryEntryAsync(tvchannel, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

            return qty;
        }

        #endregion

        #region Related tvchannels

        /// <summary>
        /// Deletes a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.DeleteAsync(relatedTvChannel);
        }

        /// <summary>
        /// Gets related tvchannels by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The first tvchannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannels
        /// </returns>
        public virtual async Task<IList<RelatedTvChannel>> GetRelatedTvChannelsByTvChannelId1Async(int tvchannelId, bool showHidden = false)
        {
            var query = from rp in _relatedTvChannelRepository.Table
                        join p in _tvchannelRepository.Table on rp.TvChannelId2 equals p.Id
                        where rp.TvChannelId1 == tvchannelId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;

            var relatedTvChannels = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.RelatedTvChannelsCacheKey, tvchannelId, showHidden), async () => await query.ToListAsync());

            return relatedTvChannels;
        }

        /// <summary>
        /// Gets a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannelId">Related tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannel
        /// </returns>
        public virtual async Task<RelatedTvChannel> GetRelatedTvChannelByIdAsync(int relatedTvChannelId)
        {
            return await _relatedTvChannelRepository.GetByIdAsync(relatedTvChannelId, cache => default);
        }

        /// <summary>
        /// Inserts a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.InsertAsync(relatedTvChannel);
        }

        /// <summary>
        /// Updates a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.UpdateAsync(relatedTvChannel);
        }

        /// <summary>
        /// Finds a related tvchannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="tvchannelId2">The second tvchannel identifier</param>
        /// <returns>Related tvchannel</returns>
        public virtual RelatedTvChannel FindRelatedTvChannel(IList<RelatedTvChannel> source, int tvchannelId1, int tvchannelId2)
        {
            foreach (var relatedTvChannel in source)
                if (relatedTvChannel.TvChannelId1 == tvchannelId1 && relatedTvChannel.TvChannelId2 == tvchannelId2)
                    return relatedTvChannel;
            return null;
        }

        #endregion

        #region Cross-sell tvchannels

        /// <summary>
        /// Deletes a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel)
        {
            await _crossSellTvChannelRepository.DeleteAsync(crossSellTvChannel);
        }

        /// <summary>
        /// Gets cross-sell tvchannels by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannels
        /// </returns>
        public virtual async Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelId1Async(int tvchannelId1, bool showHidden = false)
        {
            return await GetCrossSellTvChannelsByTvChannelIdsAsync(new[] { tvchannelId1 }, showHidden);
        }

        /// <summary>
        /// Gets a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannelId">Cross-sell tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannel
        /// </returns>
        public virtual async Task<CrossSellTvChannel> GetCrossSellTvChannelByIdAsync(int crossSellTvChannelId)
        {
            return await _crossSellTvChannelRepository.GetByIdAsync(crossSellTvChannelId, cache => default);
        }

        /// <summary>
        /// Inserts a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel)
        {
            await _crossSellTvChannelRepository.InsertAsync(crossSellTvChannel);
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfTvChannels">Number of tvchannels to return</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sells
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetCrossSellTvChannelsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfTvChannels)
        {
            var result = new List<TvChannel>();

            if (numberOfTvChannels == 0)
                return result;

            if (cart == null || !cart.Any())
                return result;

            var cartTvChannelIds = new List<int>();
            foreach (var sci in cart)
            {
                var prodId = sci.TvChannelId;
                if (!cartTvChannelIds.Contains(prodId))
                    cartTvChannelIds.Add(prodId);
            }

            var tvchannelIds = cart.Select(sci => sci.TvChannelId).ToArray();
            var crossSells = await GetCrossSellTvChannelsByTvChannelIdsAsync(tvchannelIds);
            foreach (var crossSell in crossSells)
            {
                //validate that this tvchannel is not added to result yet
                //validate that this tvchannel is not in the cart
                if (result.Find(p => p.Id == crossSell.TvChannelId2) != null || cartTvChannelIds.Contains(crossSell.TvChannelId2))
                    continue;

                var tvchannelToAdd = await GetTvChannelByIdAsync(crossSell.TvChannelId2);
                //validate tvchannel
                if (tvchannelToAdd == null || tvchannelToAdd.Deleted || !tvchannelToAdd.Published)
                    continue;

                //add a tvchannel to result
                result.Add(tvchannelToAdd);
                if (result.Count >= numberOfTvChannels)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Finds a cross-sell tvchannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="tvchannelId2">The second tvchannel identifier</param>
        /// <returns>Cross-sell tvchannel</returns>
        public virtual CrossSellTvChannel FindCrossSellTvChannel(IList<CrossSellTvChannel> source, int tvchannelId1, int tvchannelId2)
        {
            foreach (var crossSellTvChannel in source)
                if (crossSellTvChannel.TvChannelId1 == tvchannelId1 && crossSellTvChannel.TvChannelId2 == tvchannelId2)
                    return crossSellTvChannel;
            return null;
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tvchannel tier prices for user
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesAsync(TvChannel tvchannel, User user, Store store)
        {
            if (tvchannel is null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (!tvchannel.HasTierPrices)
                return null;

            //get actual tier prices
            return (await GetTierPricesByTvChannelAsync(tvchannel.Id))
                .OrderBy(price => price.Quantity)
                .FilterByStore(store)
                .FilterByUserRole(await _userService.GetUserRoleIdsAsync(user))
                .FilterByDate()
                .RemoveDuplicatedQuantities()
                .ToList();
        }

        /// <summary>
        /// Gets a tier prices by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesByTvChannelAsync(int tvchannelId)
        {
            var query = _tierPriceRepository.Table.Where(tp => tp.TvChannelId == tvchannelId);

            return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TierPricesByTvChannelCacheKey, tvchannelId), async () => await query.ToListAsync());
        }

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.DeleteAsync(tierPrice);
        }

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price
        /// </returns>
        public virtual async Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId)
        {
            return await _tierPriceRepository.GetByIdAsync(tierPriceId, cache => default);
        }

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.InsertAsync(tierPrice);
        }

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTierPriceAsync(TierPrice tierPrice)
        {
            await _tierPriceRepository.UpdateAsync(tierPrice);
        }

        /// <summary>
        /// Gets a preferred tier price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tier price
        /// </returns>
        public virtual async Task<TierPrice> GetPreferredTierPriceAsync(TvChannel tvchannel, User user, Store store, int quantity)
        {
            if (tvchannel is null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (!tvchannel.HasTierPrices)
                return null;

            //get the most suitable tier price based on the passed quantity
            return (await GetTierPricesAsync(tvchannel, user, store))?.LastOrDefault(price => quantity >= price.Quantity);
        }

        #endregion

        #region TvChannel pictures

        /// <summary>
        /// Deletes a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelPictureAsync(TvChannelPicture tvchannelPicture)
        {
            await _tvchannelPictureRepository.DeleteAsync(tvchannelPicture);
        }

        /// <summary>
        /// Gets a tvchannel pictures by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel pictures
        /// </returns>
        public virtual async Task<IList<TvChannelPicture>> GetTvChannelPicturesByTvChannelIdAsync(int tvchannelId)
        {
            var query = from pp in _tvchannelPictureRepository.Table
                        where pp.TvChannelId == tvchannelId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;

            var tvchannelPictures = await query.ToListAsync();

            return tvchannelPictures;
        }

        /// <summary>
        /// Gets a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPictureId">TvChannel picture identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel picture
        /// </returns>
        public virtual async Task<TvChannelPicture> GetTvChannelPictureByIdAsync(int tvchannelPictureId)
        {
            return await _tvchannelPictureRepository.GetByIdAsync(tvchannelPictureId, cache => default);
        }

        /// <summary>
        /// Inserts a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelPictureAsync(TvChannelPicture tvchannelPicture)
        {
            await _tvchannelPictureRepository.InsertAsync(tvchannelPicture);
        }

        /// <summary>
        /// Updates a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelPictureAsync(TvChannelPicture tvchannelPicture)
        {
            await _tvchannelPictureRepository.UpdateAsync(tvchannelPicture);
        }

        /// <summary>
        /// Get the IDs of all tvchannel images 
        /// </summary>
        /// <param name="tvchannelsIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the all picture identifiers grouped by tvchannel ID
        /// </returns>
        public async Task<IDictionary<int, int[]>> GetTvChannelsImagesIdsAsync(int[] tvchannelsIds)
        {
            var tvchannelPictures = await _tvchannelPictureRepository.Table
                .Where(p => tvchannelsIds.Contains(p.TvChannelId))
                .ToListAsync();

            return tvchannelPictures.GroupBy(p => p.TvChannelId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        /// <summary>
        /// Get tvchannels for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted tvchannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvchannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var tvchannels = _tvchannelRepository.Table.Where(tvchannel => tvchannel.HasDiscountsApplied);

            if (discountId.HasValue)
                tvchannels = from tvchannel in tvchannels
                           join dpm in _discountTvChannelMappingRepository.Table on tvchannel.Id equals dpm.EntityId
                           where dpm.DiscountId == discountId.Value
                           select tvchannel;

            if (!showHidden)
                tvchannels = tvchannels.Where(tvchannel => !tvchannel.Deleted);

            tvchannels = tvchannels.OrderBy(tvchannel => tvchannel.DisplayOrder).ThenBy(tvchannel => tvchannel.Id);

            return await tvchannels.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region TvChannel videos

        /// <summary>
        /// Deletes a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelVideoAsync(TvChannelVideo tvchannelVideo)
        {
            await _tvchannelVideoRepository.DeleteAsync(tvchannelVideo);
        }

        /// <summary>
        /// Gets a tvchannel videos by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel videos
        /// </returns>
        public virtual async Task<IList<TvChannelVideo>> GetTvChannelVideosByTvChannelIdAsync(int tvchannelId)
        {
            var query = from pvm in _tvchannelVideoRepository.Table
                        where pvm.TvChannelId == tvchannelId
                        orderby pvm.DisplayOrder, pvm.Id
                        select pvm;

            var tvchannelVideos = await query.ToListAsync();

            return tvchannelVideos;
        }

        /// <summary>
        /// Gets a tvchannel video
        /// </summary>
        /// <param name="tvchannelPictureId">TvChannel video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel video
        /// </returns>
        public virtual async Task<TvChannelVideo> GetTvChannelVideoByIdAsync(int tvchannelVideoId)
        {
            return await _tvchannelVideoRepository.GetByIdAsync(tvchannelVideoId, cache => default);
        }

        /// <summary>
        /// Inserts a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelVideoAsync(TvChannelVideo tvchannelVideo)
        {
            await _tvchannelVideoRepository.InsertAsync(tvchannelVideo);
        }

        /// <summary>
        /// Updates a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelVideoAsync(TvChannelVideo tvchannelVideo)
        {
            await _tvchannelVideoRepository.UpdateAsync(tvchannelVideo);
        }

        #endregion

        #region TvChannel reviews

        /// <summary>
        /// Gets all tvchannel reviews
        /// </summary>
        /// <param name="userId">User identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier, where a review has been created; pass 0 to load all records</param>
        /// <param name="tvchannelId">The tvchannel identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to tvchannels of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reviews
        /// </returns>
        public virtual async Task<IPagedList<TvChannelReview>> GetAllTvChannelReviewsAsync(int userId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int tvchannelId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var tvchannelReviews = await _tvchannelReviewRepository.GetAllPagedAsync(async query =>
            {
                if (!showHidden)
                {
                    var tvchannelsQuery = _tvchannelRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    tvchannelsQuery = await _storeMappingService.ApplyStoreMapping(tvchannelsQuery, storeId);

                    //apply ACL constraints
                    var user = await _workContext.GetCurrentUserAsync();
                    tvchannelsQuery = await _aclService.ApplyAcl(tvchannelsQuery, user);

                    query = query.Where(review => tvchannelsQuery.Any(tvchannel => tvchannel.Id == review.TvChannelId));
                }

                if (approved.HasValue)
                    query = query.Where(pr => pr.IsApproved == approved);
                if (userId > 0)
                    query = query.Where(pr => pr.UserId == userId);
                if (fromUtc.HasValue)
                    query = query.Where(pr => fromUtc.Value <= pr.CreatedOnUtc);
                if (toUtc.HasValue)
                    query = query.Where(pr => toUtc.Value >= pr.CreatedOnUtc);
                if (!string.IsNullOrEmpty(message))
                    query = query.Where(pr => pr.Title.Contains(message) || pr.ReviewText.Contains(message));
                if (storeId > 0)
                    query = query.Where(pr => pr.StoreId == storeId);
                if (tvchannelId > 0)
                    query = query.Where(pr => pr.TvChannelId == tvchannelId);

                query = from tvchannelReview in query
                        join tvchannel in _tvchannelRepository.Table on tvchannelReview.TvChannelId equals tvchannel.Id
                        where
                            (vendorId == 0 || tvchannel.VendorId == vendorId) &&
                            //ignore deleted tvchannels
                            !tvchannel.Deleted
                        select tvchannelReview;

                query = _catalogSettings.TvChannelReviewsSortByCreatedDateAscending
                    ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                    : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

                return query;
            }, pageIndex, pageSize);

            return tvchannelReviews;
        }

        /// <summary>
        /// Gets tvchannel review
        /// </summary>
        /// <param name="tvchannelReviewId">TvChannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel review
        /// </returns>
        public virtual async Task<TvChannelReview> GetTvChannelReviewByIdAsync(int tvchannelReviewId)
        {
            return await _tvchannelReviewRepository.GetByIdAsync(tvchannelReviewId, cache => default);
        }

        /// <summary>
        /// Get tvchannel reviews by identifiers
        /// </summary>
        /// <param name="tvchannelReviewIds">TvChannel review identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel reviews
        /// </returns>
        public virtual async Task<IList<TvChannelReview>> GetTvChannelReviewsByIdsAsync(int[] tvchannelReviewIds)
        {
            return await _tvchannelReviewRepository.GetByIdsAsync(tvchannelReviewIds);
        }

        /// <summary>
        /// Inserts a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelReviewAsync(TvChannelReview tvchannelReview)
        {
            await _tvchannelReviewRepository.InsertAsync(tvchannelReview);
        }

        /// <summary>
        /// Deletes a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelReviewAsync(TvChannelReview tvchannelReview)
        {
            await _tvchannelReviewRepository.DeleteAsync(tvchannelReview);
        }

        /// <summary>
        /// Deletes tvchannel reviews
        /// </summary>
        /// <param name="tvchannelReviews">TvChannel reviews</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelReviewsAsync(IList<TvChannelReview> tvchannelReviews)
        {
            await _tvchannelReviewRepository.DeleteAsync(tvchannelReviews);
        }

        /// <summary>
        /// Sets or create a tvchannel review helpfulness record
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task SetTvChannelReviewHelpfulnessAsync(TvChannelReview tvchannelReview, bool helpfulness)
        {
            if (tvchannelReview is null)
                throw new ArgumentNullException(nameof(tvchannelReview));

            var user = await _workContext.GetCurrentUserAsync();
            var prh = _tvchannelReviewHelpfulnessRepository.Table
                .SingleOrDefault(h => h.TvChannelReviewId == tvchannelReview.Id && h.UserId == user.Id);

            if (prh is null)
            {
                //insert new helpfulness
                prh = new TvChannelReviewHelpfulness
                {
                    TvChannelReviewId = tvchannelReview.Id,
                    UserId = user.Id,
                    WasHelpful = helpfulness,
                };

                await InsertTvChannelReviewHelpfulnessAsync(prh);
            }
            else
            {
                //existing one
                prh.WasHelpful = helpfulness;

                await _tvchannelReviewHelpfulnessRepository.UpdateAsync(prh);
            }
        }

        /// <summary>
        /// Updates a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelReviewAsync(TvChannelReview tvchannelReview)
        {
            await _tvchannelReviewRepository.UpdateAsync(tvchannelReview);
        }

        /// <summary>
        /// Updates a totals helpfulness count for tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task UpdateTvChannelReviewHelpfulnessTotalsAsync(TvChannelReview tvchannelReview)
        {
            if (tvchannelReview is null)
                throw new ArgumentNullException(nameof(tvchannelReview));

            (tvchannelReview.HelpfulYesTotal, tvchannelReview.HelpfulNoTotal) = await GetHelpfulnessCountsAsync(tvchannelReview);

            await _tvchannelReviewRepository.UpdateAsync(tvchannelReview);
        }

        /// <summary>
        /// Check possibility added review for current user
        /// </summary>
        /// <param name="tvchannelId">Current tvchannel</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        public virtual async Task<bool> CanAddReviewAsync(int tvchannelId, int storeId = 0)
        {
            var user = await _workContext.GetCurrentUserAsync();

            if (_catalogSettings.OneReviewPerTvChannelFromUser)
                return (await GetAllTvChannelReviewsAsync(userId: user.Id, tvchannelId: tvchannelId, storeId: storeId)).TotalCount == 0;

            return true;
        }

        #endregion

        #region TvChannel warehouses

        /// <summary>
        /// Get a tvchannel warehouse-inventory records by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TvChannelWarehouseInventory>> GetAllTvChannelWarehouseInventoryRecordsAsync(int tvchannelId)
        {
            return await _tvchannelWarehouseInventoryRepository.GetAllAsync(query => query.Where(pwi => pwi.TvChannelId == tvchannelId));
        }

        /// <summary>
        /// Deletes a record to manage tvchannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvchannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvchannelWarehouseInventoryRepository.DeleteAsync(pwi);
        }

        /// <summary>
        /// Inserts a record to manage tvchannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvchannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvchannelWarehouseInventoryRepository.InsertAsync(pwi);
        }

        /// <summary>
        /// Updates a record to manage tvchannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvchannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvchannelWarehouseInventoryRepository.UpdateAsync(pwi);
        }

        /// <summary>
        /// Updates a records to manage tvchannel inventory per warehouse
        /// </summary>
        /// <param name="pwis">Records to manage tvchannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelWarehouseInventoryAsync(IList<TvChannelWarehouseInventory> pwis)
        {
            await _tvchannelWarehouseInventoryRepository.UpdateAsync(pwis);
        }

        #endregion

        #region Stock quantity history

        /// <summary>
        /// Add stock quantity change entry
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="quantityAdjustment">Quantity adjustment</param>
        /// <param name="stockQuantity">Current stock quantity</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="message">Message</param>
        /// <param name="combinationId">TvChannel attribute combination identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddStockQuantityHistoryEntryAsync(TvChannel tvchannel, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            if (quantityAdjustment == 0)
                return;

            var historyEntry = new StockQuantityHistory
            {
                TvChannelId = tvchannel.Id,
                CombinationId = combinationId,
                WarehouseId = warehouseId > 0 ? (int?)warehouseId : null,
                QuantityAdjustment = quantityAdjustment,
                StockQuantity = stockQuantity,
                Message = message,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _stockQuantityHistoryRepository.InsertAsync(historyEntry);
        }

        /// <summary>
        /// Get the history of the tvchannel stock quantity changes
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
        /// <param name="combinationId">TvChannel attribute combination identifier; pass 0 to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of stock quantity change entries
        /// </returns>
        public virtual async Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(TvChannel tvchannel, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (tvchannel == null)
                throw new ArgumentNullException(nameof(tvchannel));

            var query = _stockQuantityHistoryRepository.Table.Where(historyEntry => historyEntry.TvChannelId == tvchannel.Id);

            if (warehouseId > 0)
                query = query.Where(historyEntry => historyEntry.WarehouseId == warehouseId);

            if (combinationId > 0)
                query = query.Where(historyEntry => historyEntry.CombinationId == combinationId);

            query = query.OrderByDescending(historyEntry => historyEntry.CreatedOnUtc).ThenByDescending(historyEntry => historyEntry.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region TvChannel discounts

        /// <summary>
        /// Clean up tvchannel references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ClearDiscountTvChannelMappingAsync(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappingsWithTvChannels =
                from dcm in _discountTvChannelMappingRepository.Table
                join p in _tvchannelRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select new { tvchannel = p, dcm };

            foreach (var pdcm in await mappingsWithTvChannels.ToListAsync())
            {
                await _discountTvChannelMappingRepository.DeleteAsync(pdcm.dcm);

                //update "HasDiscountsApplied" property
                await UpdateHasDiscountsAppliedAsync(pdcm.tvchannel);
            }
        }

        /// <summary>
        /// Get a discount-tvchannel mapping records by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<DiscountTvChannelMapping>> GetAllDiscountsAppliedToTvChannelAsync(int tvchannelId)
        {
            return await _discountTvChannelMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == tvchannelId));
        }

        /// <summary>
        /// Get a discount-tvchannel mapping record
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<DiscountTvChannelMapping> GetDiscountAppliedToTvChannelAsync(int tvchannelId, int discountId)
        {
            return await _discountTvChannelMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == tvchannelId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-tvchannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvchannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping)
        {
            await _discountTvChannelMappingRepository.InsertAsync(discountTvChannelMapping);
        }

        /// <summary>
        /// Deletes a discount-tvchannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvchannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping)
        {
            await _discountTvChannelMappingRepository.DeleteAsync(discountTvChannelMapping);
        }

        #endregion

        #endregion
    }
}
