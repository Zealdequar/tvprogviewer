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
        protected readonly ITvChannelAttributeParser _tvChannelAttributeParser;
        protected readonly ITvChannelAttributeService _tvChannelAttributeService;
        protected readonly IRepository<Category> _categoryRepository;
        protected readonly IRepository<CrossSellTvChannel> _crossSellTvChannelRepository;
        protected readonly IRepository<DiscountTvChannelMapping> _discountTvChannelMappingRepository;
        protected readonly IRepository<LocalizedProperty> _localizedPropertyRepository;
        protected readonly IRepository<Manufacturer> _manufacturerRepository;
        protected readonly IRepository<TvChannel> _tvChannelRepository;
        protected readonly IRepository<TvChannelAttributeCombination> _tvChannelAttributeCombinationRepository;
        protected readonly IRepository<TvChannelAttributeMapping> _tvChannelAttributeMappingRepository;
        protected readonly IRepository<TvChannelCategory> _tvChannelCategoryRepository;
        protected readonly IRepository<TvChannelManufacturer> _tvChannelManufacturerRepository;
        protected readonly IRepository<TvChannelPicture> _tvChannelPictureRepository;
        protected readonly IRepository<TvChannelTvChannelTagMapping> _tvChannelTagMappingRepository;
        protected readonly IRepository<TvChannelReview> _tvChannelReviewRepository;
        protected readonly IRepository<TvChannelReviewHelpfulness> _tvChannelReviewHelpfulnessRepository;
        protected readonly IRepository<TvChannelSpecificationAttribute> _tvChannelSpecificationAttributeRepository;
        protected readonly IRepository<TvChannelTag> _tvChannelTagRepository;
        protected readonly IRepository<TvChannelVideo> _tvChannelVideoRepository;
        protected readonly IRepository<TvChannelWarehouseInventory> _tvChannelWarehouseInventoryRepository;
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
            ITvChannelAttributeParser tvChannelAttributeParser,
            ITvChannelAttributeService tvChannelAttributeService,
            IRepository<Category> categoryRepository,
            IRepository<CrossSellTvChannel> crossSellTvChannelRepository,
            IRepository<DiscountTvChannelMapping> discountTvChannelMappingRepository,
            IRepository<LocalizedProperty> localizedPropertyRepository,
            IRepository<Manufacturer> manufacturerRepository,
            IRepository<TvChannel> tvChannelRepository,
            IRepository<TvChannelAttributeCombination> tvChannelAttributeCombinationRepository,
            IRepository<TvChannelAttributeMapping> tvChannelAttributeMappingRepository,
            IRepository<TvChannelCategory> tvChannelCategoryRepository,
            IRepository<TvChannelManufacturer> tvChannelManufacturerRepository,
            IRepository<TvChannelPicture> tvChannelPictureRepository,
            IRepository<TvChannelTvChannelTagMapping> tvChannelTagMappingRepository,
            IRepository<TvChannelReview> tvChannelReviewRepository,
            IRepository<TvChannelReviewHelpfulness> tvChannelReviewHelpfulnessRepository,
            IRepository<TvChannelSpecificationAttribute> tvChannelSpecificationAttributeRepository,
            IRepository<TvChannelTag> tvChannelTagRepository,
            IRepository<TvChannelVideo> tvChannelVideoRepository,
            IRepository<TvChannelWarehouseInventory> tvChannelWarehouseInventoryRepository,
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
            _tvChannelAttributeParser = tvChannelAttributeParser;
            _tvChannelAttributeService = tvChannelAttributeService;
            _categoryRepository = categoryRepository;
            _crossSellTvChannelRepository = crossSellTvChannelRepository;
            _discountTvChannelMappingRepository = discountTvChannelMappingRepository;
            _localizedPropertyRepository = localizedPropertyRepository;
            _manufacturerRepository = manufacturerRepository;
            _tvChannelRepository = tvChannelRepository;
            _tvChannelAttributeCombinationRepository = tvChannelAttributeCombinationRepository;
            _tvChannelAttributeMappingRepository = tvChannelAttributeMappingRepository;
            _tvChannelCategoryRepository = tvChannelCategoryRepository;
            _tvChannelManufacturerRepository = tvChannelManufacturerRepository;
            _tvChannelPictureRepository = tvChannelPictureRepository;
            _tvChannelTagMappingRepository = tvChannelTagMappingRepository;
            _tvChannelReviewRepository = tvChannelReviewRepository;
            _tvChannelReviewHelpfulnessRepository = tvChannelReviewHelpfulnessRepository;
            _tvChannelSpecificationAttributeRepository = tvChannelSpecificationAttributeRepository;
            _tvChannelTagRepository = tvChannelTagRepository;
            _tvChannelVideoRepository = tvChannelVideoRepository;
            _tvChannelWarehouseInventoryRepository = tvChannelWarehouseInventoryRepository;
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
        /// Applies the low stock activity to specified tvChannel by the total stock quantity
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="totalStock">Total stock</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ApplyLowStockActivityAsync(TvChannel tvChannel, int totalStock)
        {
            var isMinimumStockReached = totalStock <= tvChannel.MinStockQuantity;

            if (!isMinimumStockReached && !_catalogSettings.PublishBackTvChannelWhenCancellingOrders)
                return;

            switch (tvChannel.LowStockActivity)
            {
                case LowStockActivity.DisableBuyButton:
                    tvChannel.DisableBuyButton = isMinimumStockReached;
                    tvChannel.DisableWishlistButton = isMinimumStockReached;
                    await UpdateTvChannelAsync(tvChannel);
                    break;

                case LowStockActivity.Unpublish:
                    tvChannel.Published = !isMinimumStockReached;
                    await UpdateTvChannelAsync(tvChannel);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets SKU, Manufacturer part number and GTIN
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU, Manufacturer part number, GTIN
        /// </returns>
        protected virtual async Task<(string sku, string manufacturerPartNumber, string gtin)> GetSkuMpnGtinAsync(TvChannel tvChannel, string attributesXml)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            string sku = null;
            string manufacturerPartNumber = null;
            string gtin = null;

            if (!string.IsNullOrEmpty(attributesXml) &&
                tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                //manage stock by attribute combinations
                //let's find appropriate record
                var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
                if (combination != null)
                {
                    sku = combination.Sku;
                    manufacturerPartNumber = combination.ManufacturerPartNumber;
                    gtin = combination.Gtin;
                }
            }

            if (string.IsNullOrEmpty(sku))
                sku = tvChannel.Sku;
            if (string.IsNullOrEmpty(manufacturerPartNumber))
                manufacturerPartNumber = tvChannel.ManufacturerPartNumber;
            if (string.IsNullOrEmpty(gtin))
                gtin = tvChannel.Gtin;

            return (sku, manufacturerPartNumber, gtin);
        }

        /// <summary>
        /// Get stock message for a tvChannel with attributes
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageForAttributesAsync(TvChannel tvChannel, string attributesXml)
        {
            if (!tvChannel.DisplayStockAvailability)
                return string.Empty;

            string stockMessage;

            var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
            if (combination != null)
            {
                //combination exists
                var stockQuantity = combination.StockQuantity;
                if (stockQuantity > 0)
                {
                    if (tvChannel.MinStockQuantity >= stockQuantity && tvChannel.LowStockActivity == LowStockActivity.Nothing)
                    {
                        stockMessage = tvChannel.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.LowStock");
                    }
                    else
                    {
                        stockMessage = tvChannel.DisplayStockQuantity
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
                        var tvChannelAvailabilityRange = await
                            _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvChannel.TvChannelAvailabilityRangeId);
                        stockMessage = tvChannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
                    }
                }
            }
            else
            {
                //no combination configured
                if (tvChannel.AllowAddingOnlyExistingAttributeCombinations)
                {
                    var allIds = (await _tvChannelAttributeService.GetTvChannelAttributeMappingsByTvChannelIdAsync(tvChannel.Id)).Where(pa => pa.IsRequired).Select(pa => pa.Id).ToList();
                    var exIds = (await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(attributesXml)).Select(pa => pa.Id).ToList();

                    var selectedIds = allIds.Intersect(exIds).ToList();

                    if (selectedIds.Count() != allIds.Count)
                        if (_catalogSettings.AttributeValueOutOfStockDisplayType == AttributeValueOutOfStockDisplayType.AlwaysDisplay)
                            return await _localizationService.GetResourceAsync("TvChannels.Availability.SelectRequiredAttributes");
                        else
                        {
                            var combinations = await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id);

                            combinations = combinations.Where(p => p.StockQuantity >= 0 || p.AllowOutOfStockOrders).ToList();

                            var attributes = await combinations.SelectAwait(async c => await _tvChannelAttributeParser.ParseTvChannelAttributeMappingsAsync(c.AttributesXml)).ToListAsync();

                            var flag = attributes.SelectMany(a => a).Any(a => selectedIds.Contains(a.Id));

                            if (flag)
                                return await _localizationService.GetResourceAsync("TvChannels.Availability.SelectRequiredAttributes");
                        }

                    var tvChannelAvailabilityRange = await
                        _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvChannel.TvChannelAvailabilityRangeId);
                    stockMessage = tvChannelAvailabilityRange == null
                        ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                        : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                            await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
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
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the message
        /// </returns>
        protected virtual async Task<string> GetStockMessageAsync(TvChannel tvChannel)
        {
            if (!tvChannel.DisplayStockAvailability)
                return string.Empty;

            var stockMessage = string.Empty;
            var stockQuantity = await GetTotalStockQuantityAsync(tvChannel);

            if (stockQuantity > 0)
            {
                if (tvChannel.MinStockQuantity >= stockQuantity && tvChannel.LowStockActivity == LowStockActivity.Nothing)
                {
                    stockMessage = tvChannel.DisplayStockQuantity
                        ?
                        //display "low stock" with stock quantity
                        string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.LowStockWithQuantity"), stockQuantity)
                        :
                        //display "low stock" without stock quantity
                        await _localizationService.GetResourceAsync("TvChannels.Availability.LowStock");
                }
                else
                {
                    stockMessage = tvChannel.DisplayStockQuantity
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
                var tvChannelAvailabilityRange = await _dateRangeService.GetTvChannelAvailabilityRangeByIdAsync(tvChannel.TvChannelAvailabilityRangeId);
                switch (tvChannel.BackorderMode)
                {
                    case BackorderMode.NoBackorders:
                        stockMessage = tvChannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.OutOfStock")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.AvailabilityRange"),
                                await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
                        break;
                    case BackorderMode.AllowQtyBelow0:
                        stockMessage = await _localizationService.GetResourceAsync("TvChannels.Availability.InStock");
                        break;
                    case BackorderMode.AllowQtyBelow0AndNotifyUser:
                        stockMessage = tvChannelAvailabilityRange == null
                            ? await _localizationService.GetResourceAsync("TvChannels.Availability.Backordering")
                            : string.Format(await _localizationService.GetResourceAsync("TvChannels.Availability.BackorderingWithDate"),
                                await _localizationService.GetLocalizedAsync(tvChannelAvailabilityRange, range => range.Name));
                        break;
                }
            }

            return stockMessage;
        }

        /// <summary>
        /// Reserve the given quantity in the warehouses.
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task ReserveInventoryAsync(TvChannel tvChannel, int quantity)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            var qty = -quantity;

            var tvChannelInventory = _tvChannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvChannel.Id)
                .OrderByDescending(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                .ToList();

            if (tvChannelInventory.Count <= 0)
                return;

            // 1st pass: Applying reserved
            foreach (var item in tvChannelInventory)
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
                var pwi = tvChannelInventory[0];
                pwi.ReservedQuantity += qty;
            }

            await UpdateTvChannelWarehouseInventoryAsync(tvChannelInventory);
        }

        /// <summary>
        /// Unblocks the given quantity reserved items in the warehouses
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantity">Quantity, must be positive</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task UnblockReservedInventoryAsync(TvChannel tvChannel, int quantity)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (quantity < 0)
                throw new ArgumentException("Value must be positive.", nameof(quantity));

            var tvChannelInventory = await _tvChannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvChannel.Id)
                .OrderByDescending(pwi => pwi.ReservedQuantity)
                .ThenByDescending(pwi => pwi.StockQuantity)
                .ToListAsync();

            if (!tvChannelInventory.Any())
                return;

            var qty = quantity;

            foreach (var item in tvChannelInventory)
            {
                var selectQty = Math.Min(item.ReservedQuantity, qty);
                item.ReservedQuantity -= selectQty;
                qty -= selectQty;

                if (qty <= 0)
                    break;
            }

            if (qty > 0)
            {
                var pwi = tvChannelInventory[0];
                pwi.StockQuantity += qty;
            }

            await UpdateTvChannelWarehouseInventoryAsync(tvChannelInventory);
        }

        /// <summary>
        /// Gets cross-sell tvChannels by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelIds">The first tvChannel identifiers</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannels
        /// </returns>
        protected virtual async Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelIdsAsync(int[] tvChannelIds, bool showHidden = false)
        {
            if (tvChannelIds == null || tvChannelIds.Length == 0)
                return new List<CrossSellTvChannel>();

            var query = from csp in _crossSellTvChannelRepository.Table
                        join p in _tvChannelRepository.Table on csp.TvChannelId2 equals p.Id
                        where tvChannelIds.Contains(csp.TvChannelId1) &&
                              !p.Deleted &&
                              (showHidden || p.Published)
                        orderby csp.Id
                        select csp;
            var crossSellTvChannels = await query.ToListAsync();

            return crossSellTvChannels;
        }

        /// <summary>
        /// Gets ratio of useful and not useful tvChannel reviews 
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        protected virtual async Task<(int usefulCount, int notUsefulCount)> GetHelpfulnessCountsAsync(TvChannelReview tvChannelReview)
        {
            if (tvChannelReview is null)
                throw new ArgumentNullException(nameof(tvChannelReview));

            var tvChannelReviewHelpfulness = _tvChannelReviewHelpfulnessRepository.Table.Where(prh => prh.TvChannelReviewId == tvChannelReview.Id);

            return (await tvChannelReviewHelpfulness.CountAsync(prh => prh.WasHelpful),
                await tvChannelReviewHelpfulness.CountAsync(prh => !prh.WasHelpful));
        }

        /// <summary>
        /// Inserts a tvChannel review helpfulness record
        /// </summary>
        /// <param name="tvChannelReviewHelpfulness">TvChannel review helpfulness record</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task InsertTvChannelReviewHelpfulnessAsync(TvChannelReviewHelpfulness tvChannelReviewHelpfulness)
        {
            await _tvChannelReviewHelpfulnessRepository.InsertAsync(tvChannelReviewHelpfulness);
        }

        #endregion

        #region Methods

        #region TvChannels

        /// <summary>
        /// Delete a tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelAsync(TvChannel tvChannel)
        {
            await _tvChannelRepository.DeleteAsync(tvChannel);
        }

        /// <summary>
        /// Delete tvChannels
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelsAsync(IList<TvChannel> tvChannels)
        {
            await _tvChannelRepository.DeleteAsync(tvChannels);
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
            var tvChannels = await _tvChannelRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where !p.Deleted
                       select p;
            });

            return tvChannels;
        }

        /// <summary>
        /// Gets all tvChannels displayed on the home page
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetAllTvChannelsDisplayedOnHomepageAsync()
        {
            var tvChannels = await _tvChannelRepository.GetAllAsync(query =>
            {
                return from p in query
                       orderby p.DisplayOrder, p.Id
                       where p.Published &&
                             !p.Deleted &&
                             p.ShowOnHomepage
                       select p;
            }, cache => cache.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TvChannelsHomepageCacheKey));

            return tvChannels;
        }

        /// <summary>
        /// Gets tvChannel
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel
        /// </returns>
        public virtual async Task<TvChannel> GetTvChannelByIdAsync(int tvChannelId)
        {
            return await _tvChannelRepository.GetByIdAsync(tvChannelId, cache => default);
        }

        /// <summary>
        /// Get tvChannels by identifiers
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetTvChannelsByIdsAsync(int[] tvChannelIds)
        {
            return await _tvChannelRepository.GetByIdsAsync(tvChannelIds, cache => default, false);
        }

        /// <summary>
        /// Inserts a tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelAsync(TvChannel tvChannel)
        {
            await _tvChannelRepository.InsertAsync(tvChannel);
        }

        /// <summary>
        /// Обновление телеканала детали
        /// </summary>
        /// <param name="tvChannel">Телеканал детали</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelAsync(TvChannel tvChannel)
        {
            await _tvChannelRepository.UpdateAsync(tvChannel);
        }

        /// <summary>
        /// Обновление телеканалов деталей
        /// </summary>
        /// <param name="tvChannels">Список телеканалов деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelListAsync(IList<TvChannel> tvChannels)
        {
            await _tvChannelRepository.UpdateAsync(tvChannels);
        }

        /// <summary>
        /// Gets featured tvChannels by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvChannels
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
                var query = from p in _tvChannelRepository.Table
                            join pc in _tvChannelCategoryRepository.Table on p.Id equals pc.TvChannelId
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
                featuredTvChannels = await _tvChannelRepository.GetByIdsAsync(featuredTvChannelIds, cache => default, false);

            return featuredTvChannels;
        }

        /// <summary>
        /// Gets featured tvChannels by manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvChannels
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
                var query = from p in _tvChannelRepository.Table
                            join pm in _tvChannelManufacturerRepository.Table on p.Id equals pm.TvChannelId
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
                featuredTvChannels = await _tvChannelRepository.GetByIdsAsync(featuredTvChannelIds, cache => default, false);

            return featuredTvChannels;
        }

        /// <summary>
        /// Gets tvChannels which marked as new
        /// </summary>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of new tvChannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _tvChannelRepository.Table
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
        /// Get number of tvChannel (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        public virtual async Task<int> GetNumberOfTvChannelsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0)
        {
            //validate "categoryIds" parameter
            if (categoryIds != null && categoryIds.Contains(0))
                categoryIds.Remove(0);

            var query = _tvChannelRepository.Table.Where(p => p.Published && !p.Deleted && p.VisibleIndividually);

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
                        join pc in _tvChannelCategoryRepository.Table on p.Id equals pc.TvChannelId
                        where categoryIds.Contains(pc.CategoryId)
                        select p;
            }

            var cacheKey = _staticCacheManager
                .PrepareKeyForDefaultCache(TvProgCatalogDefaults.CategoryTvChannelsNumberCacheKey, userRoleIds, storeId, categoryIds);

            //only distinct tvChannels
            return await _staticCacheManager.GetAsync(cacheKey, () => query.Select(p => p.Id).Count());
        }

        /// <summary>
        /// Search tvChannels
        /// </summary>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="manufacturerIds">Manufacturer identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier; 0 to load all records</param>
        /// <param name="tvChannelType">TvChannel type; 0 to load all records</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only tvChannels marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="excludeFeaturedTvChannels">A value indicating whether loaded tvChannels are marked as featured (relates only to categories and manufacturers); "false" (by default) to load all records; "true" to exclude featured tvChannels from results</param>
        /// <param name="priceMin">Minimum price; null to load all records</param>
        /// <param name="priceMax">Maximum price; null to load all records</param>
        /// <param name="tvChannelTagId">TvChannel tag identifier; 0 to load all records</param>
        /// <param name="keywords">Keywords</param>
        /// <param name="searchDescriptions">A value indicating whether to search by a specified "keyword" in tvChannel descriptions</param>
        /// <param name="searchManufacturerPartNumber">A value indicating whether to search by a specified "keyword" in manufacturer part number</param>
        /// <param name="searchSku">A value indicating whether to search by a specified "keyword" in tvChannel SKU</param>
        /// <param name="searchTvChannelTags">A value indicating whether to search by a specified "keyword" in tvChannel tags</param>
        /// <param name="languageId">Language identifier (search for text searching)</param>
        /// <param name="filteredSpecOptions">Specification options list to filter tvChannels; null to load all records</param>
        /// <param name="orderBy">Order by</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="overridePublished">
        /// null - process "Published" property according to "showHidden" parameter
        /// true - load only "Published" tvChannels
        /// false - load only "Unpublished" tvChannels
        /// </param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> SearchTvChannelsAsync(
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            IList<int> categoryIds = null,
            IList<int> manufacturerIds = null,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            TvChannelType? tvChannelType = null,
            bool visibleIndividuallyOnly = false,
            bool excludeFeaturedTvChannels = false,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int tvChannelTagId = 0,
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

            var tvChannelsQuery = _tvChannelRepository.Table;

            if (!showHidden)
                tvChannelsQuery = tvChannelsQuery.Where(p => p.Published);
            else if (overridePublished.HasValue)
                tvChannelsQuery = tvChannelsQuery.Where(p => p.Published == overridePublished.Value);

            if (!showHidden)
            {
                //apply store mapping constraints
                tvChannelsQuery = await _storeMappingService.ApplyStoreMapping(tvChannelsQuery, storeId);

                //apply ACL constraints
                var user = await _workContext.GetCurrentUserAsync();
                tvChannelsQuery = await _aclService.ApplyAcl(tvChannelsQuery, user);
            }

            tvChannelsQuery =
                from p in tvChannelsQuery
                where !p.Deleted &&
                    (!visibleIndividuallyOnly || p.VisibleIndividually) &&
                    (vendorId == 0 || p.VendorId == vendorId) &&
                    (
                        warehouseId == 0 ||
                        (
                            !p.UseMultipleWarehouses ? p.WarehouseId == warehouseId :
                                _tvChannelWarehouseInventoryRepository.Table.Any(pwi => pwi.WarehouseId == warehouseId && pwi.TvChannelId == p.Id)
                        )
                    ) &&
                    (tvChannelType == null || p.TvChannelTypeId == (int)tvChannelType) &&
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
                IQueryable<int> tvChannelsByKeywords;

                var user = await _workContext.GetCurrentUserAsync();
                var activeSearchProvider = await _searchPluginManager.LoadPrimaryPluginAsync(user, storeId);

                if (activeSearchProvider is not null)
                {
                    tvChannelsByKeywords = (await activeSearchProvider.SearchTvChannelsAsync(keywords, searchLocalizedValue)).AsQueryable();
                }
                else
                {
                    tvChannelsByKeywords =
                        from p in _tvChannelRepository.Table
                        where p.Name.Contains(keywords) ||
                            (searchDescriptions &&
                                (p.ShortDescription.Contains(keywords) || p.FullDescription.Contains(keywords))) ||
                            (searchManufacturerPartNumber && p.ManufacturerPartNumber == keywords) ||
                            (searchSku && p.Sku == keywords)
                        select p.Id;

                    if (searchLocalizedValue)
                    {
                        tvChannelsByKeywords = tvChannelsByKeywords.Union(
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
                    tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pac in _tvChannelAttributeCombinationRepository.Table
                        where pac.Sku == keywords
                        select pac.TvChannelId);
                }

                //search by category name if admin allows
                if (_catalogSettings.AllowUsersToSearchWithCategoryName)
                {
                    tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pc in _tvChannelCategoryRepository.Table
                        join c in _categoryRepository.Table on pc.CategoryId equals c.Id
                        where c.Name.Contains(keywords)
                        select pc.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pc in _tvChannelCategoryRepository.Table
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
                    tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pm in _tvChannelManufacturerRepository.Table
                        join m in _manufacturerRepository.Table on pm.ManufacturerId equals m.Id
                        where m.Name.Contains(keywords)
                        select pm.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pm in _tvChannelManufacturerRepository.Table
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
                    tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pptm in _tvChannelTagMappingRepository.Table
                        join pt in _tvChannelTagRepository.Table on pptm.TvChannelTagId equals pt.Id
                        where pt.Name.Contains(keywords)
                        select pptm.TvChannelId
                    );

                    if (searchLocalizedValue)
                    {
                        tvChannelsByKeywords = tvChannelsByKeywords.Union(
                        from pptm in _tvChannelTagMappingRepository.Table
                        join lp in _localizedPropertyRepository.Table on pptm.TvChannelTagId equals lp.EntityId
                        where lp.LocaleKeyGroup == nameof(TvChannelTag) &&
                              lp.LocaleKey == nameof(TvChannelTag.Name) &&
                              lp.LocaleValue.Contains(keywords) &&
                              lp.LanguageId == languageId
                        select pptm.TvChannelId);
                    }
                }

                tvChannelsQuery =
                    from p in tvChannelsQuery
                    join pbk in tvChannelsByKeywords on p.Id equals pbk
                    select p;
            }

            if (categoryIds is not null)
            {
                if (categoryIds.Contains(0))
                    categoryIds.Remove(0);

                if (categoryIds.Any())
                {
                    var tvChannelCategoryQuery =
                        from pc in _tvChannelCategoryRepository.Table
                        where (!excludeFeaturedTvChannels || !pc.IsFeaturedTvChannel) &&
                            categoryIds.Contains(pc.CategoryId)
                        group pc by pc.TvChannelId into pc
                        select new
                        {
                            TvChannelId = pc.Key,
                            DisplayOrder = pc.First().DisplayOrder
                        };

                    tvChannelsQuery =
                        from p in tvChannelsQuery
                        join pc in tvChannelCategoryQuery on p.Id equals pc.TvChannelId
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
                    var tvChannelManufacturerQuery =
                        from pm in _tvChannelManufacturerRepository.Table
                        where (!excludeFeaturedTvChannels || !pm.IsFeaturedTvChannel) &&
                            manufacturerIds.Contains(pm.ManufacturerId)
                        group pm by pm.TvChannelId into pm
                        select new
                        {
                            TvChannelId = pm.Key,
                            DisplayOrder = pm.First().DisplayOrder
                        };

                    tvChannelsQuery =
                        from p in tvChannelsQuery
                        join pm in tvChannelManufacturerQuery on p.Id equals pm.TvChannelId
                        orderby pm.DisplayOrder, p.Name
                        select p;
                }
            }

            if (tvChannelTagId > 0)
            {
                tvChannelsQuery =
                    from p in tvChannelsQuery
                    join ptm in _tvChannelTagMappingRepository.Table on p.Id equals ptm.TvChannelId
                    where ptm.TvChannelTagId == tvChannelTagId
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

                    var tvChannelSpecificationQuery =
                        from psa in _tvChannelSpecificationAttributeRepository.Table
                        where psa.AllowFiltering && optionIdsBySpecificationAttribute.Contains(psa.SpecificationAttributeOptionId)
                        select psa;

                    tvChannelsQuery =
                        from p in tvChannelsQuery
                        where tvChannelSpecificationQuery.Any(pc => pc.TvChannelId == p.Id)
                        select p;
                }
            }

            return await tvChannelsQuery.OrderBy(_localizedPropertyRepository, await _workContext.GetWorkingLanguageAsync(), orderBy).ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets tvChannels by tvChannel attribute
        /// </summary>
        /// <param name="tvChannelAttributeId">TvChannel attribute identifier</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsByTvChannelAttributeIdAsync(int tvChannelAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var query = from p in _tvChannelRepository.Table
                        join pam in _tvChannelAttributeMappingRepository.Table on p.Id equals pam.TvChannelId
                        where
                            pam.TvChannelAttributeId == tvChannelAttributeId &&
                            !p.Deleted
                        orderby p.Name
                        select p;

            return await query.ToPagedListAsync(pageIndex, pageSize);
        }

        /// <summary>
        /// Gets associated tvChannels
        /// </summary>
        /// <param name="parentGroupedTvChannelId">Parent tvChannel identifier (used with grouped tvChannels)</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IList<TvChannel>> GetAssociatedTvChannelsAsync(int parentGroupedTvChannelId,
            int storeId = 0, int vendorId = 0, bool showHidden = false)
        {
            var query = _tvChannelRepository.Table;
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

            var tvChannels = await query.ToListAsync();

            //ACL mapping
            if (!showHidden)
                tvChannels = await tvChannels.WhereAwait(async x => await _aclService.AuthorizeAsync(x)).ToListAsync();

            //Store mapping
            if (!showHidden && storeId > 0)
                tvChannels = await tvChannels.WhereAwait(async x => await _storeMappingService.AuthorizeAsync(x, storeId)).ToListAsync();

            return tvChannels;
        }

        /// <summary>
        /// Update tvChannel review totals
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelReviewTotalsAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var approvedRatingSum = 0;
            var notApprovedRatingSum = 0;
            var approvedTotalReviews = 0;
            var notApprovedTotalReviews = 0;

            var reviews = await _tvChannelReviewRepository.Table
                .Where(r => r.TvChannelId == tvChannel.Id)
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

            tvChannel.ApprovedRatingSum = approvedRatingSum;
            tvChannel.NotApprovedRatingSum = notApprovedRatingSum;
            tvChannel.ApprovedTotalReviews = approvedTotalReviews;
            tvChannel.NotApprovedTotalReviews = notApprovedTotalReviews;
            await UpdateTvChannelAsync(tvChannel);
        }

        /// <summary>
        /// Get low stock tvChannels
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load published tvChannels only; pass null to load all tvChannels, pass true to load only published tvChannels, pass false to load only unpublished tvChannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetLowStockTvChannelsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _tvChannelRepository.Table;

            //filter by tvChannels with tracking inventory
            query = query.Where(tvChannel => tvChannel.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStock);

            //filter by tvChannels with stock quantity less than the minimum
            query = query.Where(tvChannel =>
                (tvChannel.UseMultipleWarehouses ? _tvChannelWarehouseInventoryRepository.Table.Where(pwi => pwi.TvChannelId == tvChannel.Id).Sum(pwi => pwi.StockQuantity - pwi.ReservedQuantity)
                    : tvChannel.StockQuantity) <= tvChannel.MinStockQuantity);

            //ignore deleted tvChannels
            query = query.Where(tvChannel => !tvChannel.Deleted);

            //ignore grouped tvChannels
            query = query.Where(tvChannel => tvChannel.TvChannelTypeId != (int)TvChannelType.GroupedTvChannel);

            //filter by vendor
            if (vendorId.HasValue && vendorId.Value > 0)
                query = query.Where(tvChannel => tvChannel.VendorId == vendorId.Value);

            //whether to load published tvChannels only
            if (loadPublishedOnly.HasValue)
                query = query.Where(tvChannel => tvChannel.Published == loadPublishedOnly.Value);

            query = query.OrderBy(tvChannel => tvChannel.MinStockQuantity).ThenBy(tvChannel => tvChannel.DisplayOrder).ThenBy(tvChannel => tvChannel.Id);

            return await query.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Get low stock tvChannel combinations
        /// </summary>
        /// <param name="vendorId">Vendor identifier; pass null to load all records</param>
        /// <param name="loadPublishedOnly">Whether to load combinations of published tvChannels only; pass null to load all tvChannels, pass true to load only published tvChannels, pass false to load only unpublished tvChannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">A value in indicating whether you want to load only total number of records. Set to "true" if you don't want to load data from database</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel combinations
        /// </returns>
        public virtual async Task<IPagedList<TvChannelAttributeCombination>> GetLowStockTvChannelCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var combinations = from pac in _tvChannelAttributeCombinationRepository.Table
                               join p in _tvChannelRepository.Table on pac.TvChannelId equals p.Id
                               where
                                   //filter by combinations with stock quantity less than the minimum
                                   pac.StockQuantity <= pac.MinStockQuantity &&
                                   //filter by tvChannels with tracking inventory by attributes
                                   p.ManageInventoryMethodId == (int)ManageInventoryMethod.ManageStockByAttributes &&
                                   //ignore deleted tvChannels
                                   !p.Deleted &&
                                   //ignore grouped tvChannels
                                   p.TvChannelTypeId != (int)TvChannelType.GroupedTvChannel &&
                                   //filter by vendor
                                   ((vendorId ?? 0) == 0 || p.VendorId == vendorId) &&
                                   //whether to load published tvChannels only
                                   (loadPublishedOnly == null || p.Published == loadPublishedOnly)
                               orderby pac.TvChannelId, pac.Id
                               select pac;

            return await combinations.ToPagedListAsync(pageIndex, pageSize, getOnlyTotalCount);
        }

        /// <summary>
        /// Gets a tvChannel by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel
        /// </returns>
        public virtual async Task<TvChannel> GetTvChannelBySkuAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return null;

            sku = sku.Trim();

            var query = from p in _tvChannelRepository.Table
                        orderby p.Id
                        where !p.Deleted &&
                        p.Sku == sku
                        select p;
            var tvChannel = await query.FirstOrDefaultAsync();

            return tvChannel;
        }

        /// <summary>
        /// Gets a tvChannels by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        public async Task<IList<TvChannel>> GetTvChannelsBySkuAsync(string[] skuArray, int vendorId = 0)
        {
            if (skuArray == null)
                throw new ArgumentNullException(nameof(skuArray));

            var query = _tvChannelRepository.Table;
            query = query.Where(p => !p.Deleted && skuArray.Contains(p.Sku));

            if (vendorId != 0)
                query = query.Where(p => p.VendorId == vendorId);

            return await query.ToListAsync();
        }

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateHasTierPricesPropertyAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            tvChannel.HasTierPrices = (await GetTierPricesByTvChannelAsync(tvChannel.Id)).Any();
            await UpdateTvChannelAsync(tvChannel);
        }

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateHasDiscountsAppliedAsync(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            tvChannel.HasDiscountsApplied = _discountTvChannelMappingRepository.Table.Any(dpm => dpm.EntityId == tvChannel.Id);
            await UpdateTvChannelAsync(tvChannel);
        }

        /// <summary>
        /// Gets number of tvChannels by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        public async Task<int> GetNumberOfTvChannelsByVendorIdAsync(int vendorId)
        {
            if (vendorId == 0)
                return 0;

            return await _tvChannelRepository.Table.CountAsync(p => p.VendorId == vendorId && !p.Deleted);
        }

        /// <summary>
        /// Parse "required tvChannel Ids" property
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>A list of required tvChannel IDs</returns>
        public virtual int[] ParseRequiredTvChannelIds(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (string.IsNullOrEmpty(tvChannel.RequiredTvChannelIds))
                return Array.Empty<int>();

            var ids = new List<int>();

            foreach (var idStr in tvChannel.RequiredTvChannelIds
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim()))
                if (int.TryParse(idStr, out var id))
                    ids.Add(id);

            return ids.ToArray();
        }

        /// <summary>
        /// Get a value indicating whether a tvChannel is available now (availability dates)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        public virtual bool TvChannelIsAvailable(TvChannel tvChannel, DateTime? dateTime = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            dateTime ??= DateTime.UtcNow;

            if (tvChannel.AvailableStartDateTimeUtc.HasValue && tvChannel.AvailableStartDateTimeUtc.Value > dateTime)
                return false;

            if (tvChannel.AvailableEndDateTimeUtc.HasValue && tvChannel.AvailableEndDateTimeUtc.Value < dateTime)
                return false;

            return true;
        }

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Result</returns>
        public virtual int[] ParseAllowedQuantities(TvChannel tvChannel)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var result = new List<int>();
            if (!string.IsNullOrWhiteSpace(tvChannel.AllowedQuantities))
            {
                var quantities = tvChannel.AllowedQuantities
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
        /// <param name="tvChannel">TvChannel</param>
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
        public virtual async Task<int> GetTotalStockQuantityAsync(TvChannel tvChannel, bool useReservedQuantity = true, int warehouseId = 0)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (tvChannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock)
                //We can calculate total stock quantity when 'Manage inventory' property is set to 'Track inventory'
                return 0;

            if (!tvChannel.UseMultipleWarehouses)
                return tvChannel.StockQuantity;

            var pwi = _tvChannelWarehouseInventoryRepository.Table.Where(wi => wi.TvChannelId == tvChannel.Id);

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
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        public virtual int GetRentalPeriods(TvChannel tvChannel, DateTime startDate, DateTime endDate)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (!tvChannel.IsRental)
                return 1;

            if (startDate.CompareTo(endDate) >= 0)
                return 1;

            int totalPeriods;
            switch (tvChannel.RentalPricePeriod)
            {
                case RentalPricePeriod.Days:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = tvChannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling(totalDaysToRent / configuredPeriodDays));
                    }

                    break;
                case RentalPricePeriod.Weeks:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 7 * tvChannel.RentalPriceLength;
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

                        var configuredPeriodMonths = tvChannel.RentalPriceLength;
                        totalPeriods = Convert.ToInt32(Math.Ceiling((double)totalMonthsToRent / configuredPeriodMonths));
                    }

                    break;
                case RentalPricePeriod.Years:
                    {
                        var totalDaysToRent = Math.Max((endDate - startDate).TotalDays, 1);
                        var configuredPeriodDays = 365 * tvChannel.RentalPriceLength;
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
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Selected tvChannel attributes in XML format (if specified)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock message
        /// </returns>
        public virtual async Task<string> FormatStockMessageAsync(TvChannel tvChannel, string attributesXml)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var stockMessage = string.Empty;

            switch (tvChannel.ManageInventoryMethod)
            {
                case ManageInventoryMethod.ManageStock:
                    stockMessage = await GetStockMessageAsync(tvChannel);
                    break;
                case ManageInventoryMethod.ManageStockByAttributes:
                    stockMessage = await GetStockMessageForAttributesAsync(tvChannel, attributesXml);
                    break;
            }

            return stockMessage;
        }

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU
        /// </returns>
        public virtual async Task<string> FormatSkuAsync(TvChannel tvChannel, string attributesXml = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var (sku, _, _) = await GetSkuMpnGtinAsync(tvChannel, attributesXml);

            return sku;
        }

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer part number
        /// </returns>
        public virtual async Task<string> FormatMpnAsync(TvChannel tvChannel, string attributesXml = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var (_, manufacturerPartNumber, _) = await GetSkuMpnGtinAsync(tvChannel, attributesXml);

            return manufacturerPartNumber;
        }

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gTIN
        /// </returns>
        public virtual async Task<string> FormatGtinAsync(TvChannel tvChannel, string attributesXml = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var (_, _, gtin) = await GetSkuMpnGtinAsync(tvChannel, attributesXml);

            return gtin;
        }

        /// <summary>
        /// Formats start/end date for rental tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        public virtual string FormatRentalDate(TvChannel tvChannel, DateTime date)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (!tvChannel.IsRental)
                return null;

            return date.ToShortDateString();
        }

        /// <summary>
        /// Update tvChannel store mappings
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelStoreMappingsAsync(TvChannel tvChannel, IList<int> limitedToStoresIds)
        {
            tvChannel.LimitedToStores = limitedToStoresIds.Any();

            var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(tvChannel);
            var allStores = await _storeService.GetAllStoresAsync();
            foreach (var store in allStores)
            {
                if (limitedToStoresIds.Contains(store.Id))
                {
                    //new store
                    if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                        await _storeMappingService.InsertStoreMappingAsync(tvChannel, store.Id);
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
        /// Gets the value whether the sequence contains downloadable tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyDownloadableTvChannelAsync(int[] tvChannelIds)
        {
            return await _tvChannelRepository.Table
                .AnyAsync(p => tvChannelIds.Contains(p.Id) && p.IsDownload);
        }

        /// <summary>
        /// Gets the value whether the sequence contains gift card tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyGiftCardTvChannelAsync(int[] tvChannelIds)
        {
            return await _tvChannelRepository.Table
                .AnyAsync(p => tvChannelIds.Contains(p.Id) && p.IsGiftCard);
        }

        /// <summary>
        /// Gets the value whether the sequence contains recurring tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<bool> HasAnyRecurringTvChannelAsync(int[] tvChannelIds)
        {
            return await _tvChannelRepository.Table
                .AnyAsync(p => tvChannelIds.Contains(p.Id) && p.IsRecurring);
        }

        /// <summary>
        /// Returns a list of sku of not existing tvChannels
        /// </summary>
        /// <param name="tvChannelSku">The sku of the tvChannels to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of sku not existing tvChannels
        /// </returns>
        public virtual async Task<string[]> GetNotExistingTvChannelsAsync(string[] tvChannelSku)
        {
            if (tvChannelSku == null)
                throw new ArgumentNullException(nameof(tvChannelSku));

            var query = _tvChannelRepository.Table;
            var queryFilter = tvChannelSku.Distinct().ToArray();
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
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantityToChange">Quantity to increase or decrease</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AdjustInventoryAsync(TvChannel tvChannel, int quantityToChange, string attributesXml = "", string message = "")
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (quantityToChange == 0)
                return;

            if (tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
            {
                //update stock quantity
                if (tvChannel.UseMultipleWarehouses)
                {
                    //use multiple warehouses
                    if (quantityToChange < 0)
                        await ReserveInventoryAsync(tvChannel, quantityToChange);
                    else
                        await UnblockReservedInventoryAsync(tvChannel, quantityToChange);
                }
                else
                {
                    //do not use multiple warehouses
                    //simple inventory management
                    tvChannel.StockQuantity += quantityToChange;
                    await UpdateTvChannelAsync(tvChannel);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(tvChannel, quantityToChange, tvChannel.StockQuantity, tvChannel.WarehouseId, message);
                }

                var totalStock = await GetTotalStockQuantityAsync(tvChannel);

                await ApplyLowStockActivityAsync(tvChannel, totalStock);

                //send email notification
                if (quantityToChange < 0 && totalStock < tvChannel.NotifyAdminForQuantityBelow)
                {
                    //do not inject IWorkflowMessageService via constructor because it'll cause circular references
                    var workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
                    await workflowMessageService.SendQuantityBelowStoreOwnerNotificationAsync(tvChannel, _localizationSettings.DefaultAdminLanguageId);
                }
            }

            if (tvChannel.ManageInventoryMethod == ManageInventoryMethod.ManageStockByAttributes)
            {
                var combination = await _tvChannelAttributeParser.FindTvChannelAttributeCombinationAsync(tvChannel, attributesXml);
                if (combination != null)
                {
                    combination.StockQuantity += quantityToChange;
                    await _tvChannelAttributeService.UpdateTvChannelAttributeCombinationAsync(combination);

                    //quantity change history
                    await AddStockQuantityHistoryEntryAsync(tvChannel, quantityToChange, combination.StockQuantity, message: message, combinationId: combination.Id);

                    if (tvChannel.AllowAddingOnlyExistingAttributeCombinations)
                    {
                        var totalStockByAllCombinations = await (await _tvChannelAttributeService.GetAllTvChannelAttributeCombinationsAsync(tvChannel.Id))
                            .ToAsyncEnumerable()
                            .SumAsync(c => c.StockQuantity);

                        await ApplyLowStockActivityAsync(tvChannel, totalStockByAllCombinations);
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

            //bundled tvChannels
            var attributeValues = await _tvChannelAttributeParser.ParseTvChannelAttributeValuesAsync(attributesXml);
            foreach (var attributeValue in attributeValues)
            {
                if (attributeValue.AttributeValueType != AttributeValueType.AssociatedToTvChannel)
                    continue;

                //associated tvChannel (bundle)
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
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task BookReservedInventoryAsync(TvChannel tvChannel, int warehouseId, int quantity, string message = "")
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (quantity >= 0)
                throw new ArgumentException("Value must be negative.", nameof(quantity));

            //only tvChannels with "use multiple warehouses" are handled this way
            if (tvChannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !tvChannel.UseMultipleWarehouses)
                return;

            var pwi = await _tvChannelWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.TvChannelId == tvChannel.Id && wi.WarehouseId == warehouseId);
            if (pwi == null)
                return;

            pwi.ReservedQuantity = Math.Max(pwi.ReservedQuantity + quantity, 0);
            pwi.StockQuantity += quantity;

            await UpdateTvChannelWarehouseInventoryAsync(pwi);

            //quantity change history
            await AddStockQuantityHistoryEntryAsync(tvChannel, quantity, pwi.StockQuantity, warehouseId, message);
        }

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="tvChannel">tvChannel</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the quantity reversed
        /// </returns>
        public virtual async Task<int> ReverseBookedInventoryAsync(TvChannel tvChannel, ShipmentItem shipmentItem, string message = "")
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (shipmentItem == null)
                throw new ArgumentNullException(nameof(shipmentItem));

            //only tvChannels with "use multiple warehouses" are handled this way
            if (tvChannel.ManageInventoryMethod != ManageInventoryMethod.ManageStock || !tvChannel.UseMultipleWarehouses)
                return 0;

            var pwi = await _tvChannelWarehouseInventoryRepository.Table

                .FirstOrDefaultAsync(wi => wi.TvChannelId == tvChannel.Id && wi.WarehouseId == shipmentItem.WarehouseId);
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
            await AddStockQuantityHistoryEntryAsync(tvChannel, qty, pwi.StockQuantity, shipmentItem.WarehouseId, message);

            return qty;
        }

        #endregion

        #region Related tvChannels

        /// <summary>
        /// Deletes a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.DeleteAsync(relatedTvChannel);
        }

        /// <summary>
        /// Gets related tvChannels by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The first tvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannels
        /// </returns>
        public virtual async Task<IList<RelatedTvChannel>> GetRelatedTvChannelsByTvChannelId1Async(int tvChannelId, bool showHidden = false)
        {
            var query = from rp in _relatedTvChannelRepository.Table
                        join p in _tvChannelRepository.Table on rp.TvChannelId2 equals p.Id
                        where rp.TvChannelId1 == tvChannelId &&
                        !p.Deleted &&
                        (showHidden || p.Published)
                        orderby rp.DisplayOrder, rp.Id
                        select rp;

            var relatedTvChannels = await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.RelatedTvChannelsCacheKey, tvChannelId, showHidden), async () => await query.ToListAsync());

            return relatedTvChannels;
        }

        /// <summary>
        /// Gets a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannelId">Related tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel
        /// </returns>
        public virtual async Task<RelatedTvChannel> GetRelatedTvChannelByIdAsync(int relatedTvChannelId)
        {
            return await _relatedTvChannelRepository.GetByIdAsync(relatedTvChannelId, cache => default);
        }

        /// <summary>
        /// Inserts a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.InsertAsync(relatedTvChannel);
        }

        /// <summary>
        /// Updates a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel)
        {
            await _relatedTvChannelRepository.UpdateAsync(relatedTvChannel);
        }

        /// <summary>
        /// Finds a related tvChannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="tvChannelId2">The second tvChannel identifier</param>
        /// <returns>Related tvChannel</returns>
        public virtual RelatedTvChannel FindRelatedTvChannel(IList<RelatedTvChannel> source, int tvChannelId1, int tvChannelId2)
        {
            foreach (var relatedTvChannel in source)
                if (relatedTvChannel.TvChannelId1 == tvChannelId1 && relatedTvChannel.TvChannelId2 == tvChannelId2)
                    return relatedTvChannel;
            return null;
        }

        #endregion

        #region Cross-sell tvChannels

        /// <summary>
        /// Deletes a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel)
        {
            await _crossSellTvChannelRepository.DeleteAsync(crossSellTvChannel);
        }

        /// <summary>
        /// Gets cross-sell tvChannels by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannels
        /// </returns>
        public virtual async Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelId1Async(int tvChannelId1, bool showHidden = false)
        {
            return await GetCrossSellTvChannelsByTvChannelIdsAsync(new[] { tvChannelId1 }, showHidden);
        }

        /// <summary>
        /// Gets a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannelId">Cross-sell tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel
        /// </returns>
        public virtual async Task<CrossSellTvChannel> GetCrossSellTvChannelByIdAsync(int crossSellTvChannelId)
        {
            return await _crossSellTvChannelRepository.GetByIdAsync(crossSellTvChannelId, cache => default);
        }

        /// <summary>
        /// Inserts a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel)
        {
            await _crossSellTvChannelRepository.InsertAsync(crossSellTvChannel);
        }

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfTvChannels">Number of tvChannels to return</param>
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

            var tvChannelIds = cart.Select(sci => sci.TvChannelId).ToArray();
            var crossSells = await GetCrossSellTvChannelsByTvChannelIdsAsync(tvChannelIds);
            foreach (var crossSell in crossSells)
            {
                //validate that this tvChannel is not added to result yet
                //validate that this tvChannel is not in the cart
                if (result.Find(p => p.Id == crossSell.TvChannelId2) != null || cartTvChannelIds.Contains(crossSell.TvChannelId2))
                    continue;

                var tvChannelToAdd = await GetTvChannelByIdAsync(crossSell.TvChannelId2);
                //validate tvChannel
                if (tvChannelToAdd == null || tvChannelToAdd.Deleted || !tvChannelToAdd.Published)
                    continue;

                //add a tvChannel to result
                result.Add(tvChannelToAdd);
                if (result.Count >= numberOfTvChannels)
                    return result;
            }

            return result;
        }

        /// <summary>
        /// Finds a cross-sell tvChannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="tvChannelId2">The second tvChannel identifier</param>
        /// <returns>Cross-sell tvChannel</returns>
        public virtual CrossSellTvChannel FindCrossSellTvChannel(IList<CrossSellTvChannel> source, int tvChannelId1, int tvChannelId2)
        {
            foreach (var crossSellTvChannel in source)
                if (crossSellTvChannel.TvChannelId1 == tvChannelId1 && crossSellTvChannel.TvChannelId2 == tvChannelId2)
                    return crossSellTvChannel;
            return null;
        }

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tvChannel tier prices for user
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesAsync(TvChannel tvChannel, User user, Store store)
        {
            if (tvChannel is null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (!tvChannel.HasTierPrices)
                return null;

            //get actual tier prices
            return (await GetTierPricesByTvChannelAsync(tvChannel.Id))
                .OrderBy(price => price.Quantity)
                .FilterByStore(store)
                .FilterByUserRole(await _userService.GetUserRoleIdsAsync(user))
                .FilterByDate()
                .RemoveDuplicatedQuantities()
                .ToList();
        }

        /// <summary>
        /// Gets a tier prices by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TierPrice>> GetTierPricesByTvChannelAsync(int tvChannelId)
        {
            var query = _tierPriceRepository.Table.Where(tp => tp.TvChannelId == tvChannelId);

            return await _staticCacheManager.GetAsync(_staticCacheManager.PrepareKeyForDefaultCache(TvProgCatalogDefaults.TierPricesByTvChannelCacheKey, tvChannelId), async () => await query.ToListAsync());
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
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tier price
        /// </returns>
        public virtual async Task<TierPrice> GetPreferredTierPriceAsync(TvChannel tvChannel, User user, Store store, int quantity)
        {
            if (tvChannel is null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (!tvChannel.HasTierPrices)
                return null;

            //get the most suitable tier price based on the passed quantity
            return (await GetTierPricesAsync(tvChannel, user, store))?.LastOrDefault(price => quantity >= price.Quantity);
        }

        #endregion

        #region TvChannel pictures

        /// <summary>
        /// Deletes a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelPictureAsync(TvChannelPicture tvChannelPicture)
        {
            await _tvChannelPictureRepository.DeleteAsync(tvChannelPicture);
        }

        /// <summary>
        /// Gets a tvChannel pictures by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel pictures
        /// </returns>
        public virtual async Task<IList<TvChannelPicture>> GetTvChannelPicturesByTvChannelIdAsync(int tvChannelId)
        {
            var query = from pp in _tvChannelPictureRepository.Table
                        where pp.TvChannelId == tvChannelId
                        orderby pp.DisplayOrder, pp.Id
                        select pp;

            var tvChannelPictures = await query.ToListAsync();

            return tvChannelPictures;
        }

        /// <summary>
        /// Gets a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPictureId">TvChannel picture identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel picture
        /// </returns>
        public virtual async Task<TvChannelPicture> GetTvChannelPictureByIdAsync(int tvChannelPictureId)
        {
            return await _tvChannelPictureRepository.GetByIdAsync(tvChannelPictureId, cache => default);
        }

        /// <summary>
        /// Inserts a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelPictureAsync(TvChannelPicture tvChannelPicture)
        {
            await _tvChannelPictureRepository.InsertAsync(tvChannelPicture);
        }

        /// <summary>
        /// Updates a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelPictureAsync(TvChannelPicture tvChannelPicture)
        {
            await _tvChannelPictureRepository.UpdateAsync(tvChannelPicture);
        }

        /// <summary>
        /// Get the IDs of all tvChannel images 
        /// </summary>
        /// <param name="tvChannelsIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the all picture identifiers grouped by tvChannel ID
        /// </returns>
        public async Task<IDictionary<int, int[]>> GetTvChannelsImagesIdsAsync(int[] tvChannelsIds)
        {
            var tvChannelPictures = await _tvChannelPictureRepository.Table
                .Where(p => tvChannelsIds.Contains(p.TvChannelId))
                .ToListAsync();

            return tvChannelPictures.GroupBy(p => p.TvChannelId).ToDictionary(p => p.Key, p => p.Select(p1 => p1.PictureId).ToArray());
        }

        /// <summary>
        /// Get tvChannels for which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted tvChannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannels
        /// </returns>
        public virtual async Task<IPagedList<TvChannel>> GetTvChannelsWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var tvChannels = _tvChannelRepository.Table.Where(tvChannel => tvChannel.HasDiscountsApplied);

            if (discountId.HasValue)
                tvChannels = from tvChannel in tvChannels
                           join dpm in _discountTvChannelMappingRepository.Table on tvChannel.Id equals dpm.EntityId
                           where dpm.DiscountId == discountId.Value
                           select tvChannel;

            if (!showHidden)
                tvChannels = tvChannels.Where(tvChannel => !tvChannel.Deleted);

            tvChannels = tvChannels.OrderBy(tvChannel => tvChannel.DisplayOrder).ThenBy(tvChannel => tvChannel.Id);

            return await tvChannels.ToPagedListAsync(pageIndex, pageSize);
        }

        #endregion

        #region TvChannel videos

        /// <summary>
        /// Deletes a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelVideoAsync(TvChannelVideo tvChannelVideo)
        {
            await _tvChannelVideoRepository.DeleteAsync(tvChannelVideo);
        }

        /// <summary>
        /// Gets a tvChannel videos by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel videos
        /// </returns>
        public virtual async Task<IList<TvChannelVideo>> GetTvChannelVideosByTvChannelIdAsync(int tvChannelId)
        {
            var query = from pvm in _tvChannelVideoRepository.Table
                        where pvm.TvChannelId == tvChannelId
                        orderby pvm.DisplayOrder, pvm.Id
                        select pvm;

            var tvChannelVideos = await query.ToListAsync();

            return tvChannelVideos;
        }

        /// <summary>
        /// Gets a tvChannel video
        /// </summary>
        /// <param name="tvChannelPictureId">TvChannel video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel video
        /// </returns>
        public virtual async Task<TvChannelVideo> GetTvChannelVideoByIdAsync(int tvChannelVideoId)
        {
            return await _tvChannelVideoRepository.GetByIdAsync(tvChannelVideoId, cache => default);
        }

        /// <summary>
        /// Inserts a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelVideoAsync(TvChannelVideo tvChannelVideo)
        {
            await _tvChannelVideoRepository.InsertAsync(tvChannelVideo);
        }

        /// <summary>
        /// Updates a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelVideoAsync(TvChannelVideo tvChannelVideo)
        {
            await _tvChannelVideoRepository.UpdateAsync(tvChannelVideo);
        }

        #endregion

        #region TvChannel reviews

        /// <summary>
        /// Gets all tvChannel reviews
        /// </summary>
        /// <param name="userId">User identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier, where a review has been created; pass 0 to load all records</param>
        /// <param name="tvChannelId">The tvChannel identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to tvChannels of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reviews
        /// </returns>
        public virtual async Task<IPagedList<TvChannelReview>> GetAllTvChannelReviewsAsync(int userId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int tvChannelId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var tvChannelReviews = await _tvChannelReviewRepository.GetAllPagedAsync(async query =>
            {
                if (!showHidden)
                {
                    var tvChannelsQuery = _tvChannelRepository.Table.Where(p => p.Published);

                    //apply store mapping constraints
                    tvChannelsQuery = await _storeMappingService.ApplyStoreMapping(tvChannelsQuery, storeId);

                    //apply ACL constraints
                    var user = await _workContext.GetCurrentUserAsync();
                    tvChannelsQuery = await _aclService.ApplyAcl(tvChannelsQuery, user);

                    query = query.Where(review => tvChannelsQuery.Any(tvChannel => tvChannel.Id == review.TvChannelId));
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
                if (tvChannelId > 0)
                    query = query.Where(pr => pr.TvChannelId == tvChannelId);

                query = from tvChannelReview in query
                        join tvChannel in _tvChannelRepository.Table on tvChannelReview.TvChannelId equals tvChannel.Id
                        where
                            (vendorId == 0 || tvChannel.VendorId == vendorId) &&
                            //ignore deleted tvChannels
                            !tvChannel.Deleted
                        select tvChannelReview;

                query = _catalogSettings.TvChannelReviewsSortByCreatedDateAscending
                    ? query.OrderBy(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id)
                    : query.OrderByDescending(pr => pr.CreatedOnUtc).ThenBy(pr => pr.Id);

                return query;
            }, pageIndex, pageSize);

            return tvChannelReviews;
        }

        /// <summary>
        /// Gets tvChannel review
        /// </summary>
        /// <param name="tvChannelReviewId">TvChannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review
        /// </returns>
        public virtual async Task<TvChannelReview> GetTvChannelReviewByIdAsync(int tvChannelReviewId)
        {
            return await _tvChannelReviewRepository.GetByIdAsync(tvChannelReviewId, cache => default);
        }

        /// <summary>
        /// Get tvChannel reviews by identifiers
        /// </summary>
        /// <param name="tvChannelReviewIds">TvChannel review identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel reviews
        /// </returns>
        public virtual async Task<IList<TvChannelReview>> GetTvChannelReviewsByIdsAsync(int[] tvChannelReviewIds)
        {
            return await _tvChannelReviewRepository.GetByIdsAsync(tvChannelReviewIds);
        }

        /// <summary>
        /// Inserts a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelReviewAsync(TvChannelReview tvChannelReview)
        {
            await _tvChannelReviewRepository.InsertAsync(tvChannelReview);
        }

        /// <summary>
        /// Deletes a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelReviewAsync(TvChannelReview tvChannelReview)
        {
            await _tvChannelReviewRepository.DeleteAsync(tvChannelReview);
        }

        /// <summary>
        /// Deletes tvChannel reviews
        /// </summary>
        /// <param name="tvChannelReviews">TvChannel reviews</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelReviewsAsync(IList<TvChannelReview> tvChannelReviews)
        {
            await _tvChannelReviewRepository.DeleteAsync(tvChannelReviews);
        }

        /// <summary>
        /// Sets or create a tvChannel review helpfulness record
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task SetTvChannelReviewHelpfulnessAsync(TvChannelReview tvChannelReview, bool helpfulness)
        {
            if (tvChannelReview is null)
                throw new ArgumentNullException(nameof(tvChannelReview));

            var user = await _workContext.GetCurrentUserAsync();
            var prh = _tvChannelReviewHelpfulnessRepository.Table
                .SingleOrDefault(h => h.TvChannelReviewId == tvChannelReview.Id && h.UserId == user.Id);

            if (prh is null)
            {
                //insert new helpfulness
                prh = new TvChannelReviewHelpfulness
                {
                    TvChannelReviewId = tvChannelReview.Id,
                    UserId = user.Id,
                    WasHelpful = helpfulness,
                };

                await InsertTvChannelReviewHelpfulnessAsync(prh);
            }
            else
            {
                //existing one
                prh.WasHelpful = helpfulness;

                await _tvChannelReviewHelpfulnessRepository.UpdateAsync(prh);
            }
        }

        /// <summary>
        /// Updates a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelReviewAsync(TvChannelReview tvChannelReview)
        {
            await _tvChannelReviewRepository.UpdateAsync(tvChannelReview);
        }

        /// <summary>
        /// Updates a totals helpfulness count for tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task UpdateTvChannelReviewHelpfulnessTotalsAsync(TvChannelReview tvChannelReview)
        {
            if (tvChannelReview is null)
                throw new ArgumentNullException(nameof(tvChannelReview));

            (tvChannelReview.HelpfulYesTotal, tvChannelReview.HelpfulNoTotal) = await GetHelpfulnessCountsAsync(tvChannelReview);

            await _tvChannelReviewRepository.UpdateAsync(tvChannelReview);
        }

        /// <summary>
        /// Check possibility added review for current user
        /// </summary>
        /// <param name="tvChannelId">Current tvChannel</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        public virtual async Task<bool> CanAddReviewAsync(int tvChannelId, int storeId = 0)
        {
            var user = await _workContext.GetCurrentUserAsync();

            if (_catalogSettings.OneReviewPerTvChannelFromUser)
                return (await GetAllTvChannelReviewsAsync(userId: user.Id, tvChannelId: tvChannelId, storeId: storeId)).TotalCount == 0;

            return true;
        }

        #endregion

        #region TvChannel warehouses

        /// <summary>
        /// Get a tvChannel warehouse-inventory records by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<TvChannelWarehouseInventory>> GetAllTvChannelWarehouseInventoryRecordsAsync(int tvChannelId)
        {
            return await _tvChannelWarehouseInventoryRepository.GetAllAsync(query => query.Where(pwi => pwi.TvChannelId == tvChannelId));
        }

        /// <summary>
        /// Deletes a record to manage tvChannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvChannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvChannelWarehouseInventoryRepository.DeleteAsync(pwi);
        }

        /// <summary>
        /// Inserts a record to manage tvChannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvChannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvChannelWarehouseInventoryRepository.InsertAsync(pwi);
        }

        /// <summary>
        /// Updates a record to manage tvChannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvChannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi)
        {
            await _tvChannelWarehouseInventoryRepository.UpdateAsync(pwi);
        }

        /// <summary>
        /// Updates a records to manage tvChannel inventory per warehouse
        /// </summary>
        /// <param name="pwis">Records to manage tvChannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task UpdateTvChannelWarehouseInventoryAsync(IList<TvChannelWarehouseInventory> pwis)
        {
            await _tvChannelWarehouseInventoryRepository.UpdateAsync(pwis);
        }

        #endregion

        #region Stock quantity history

        /// <summary>
        /// Add stock quantity change entry
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="quantityAdjustment">Quantity adjustment</param>
        /// <param name="stockQuantity">Current stock quantity</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="message">Message</param>
        /// <param name="combinationId">TvChannel attribute combination identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task AddStockQuantityHistoryEntryAsync(TvChannel tvChannel, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            if (quantityAdjustment == 0)
                return;

            var historyEntry = new StockQuantityHistory
            {
                TvChannelId = tvChannel.Id,
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
        /// Get the history of the tvChannel stock quantity changes
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to load all entries</param>
        /// <param name="combinationId">TvChannel attribute combination identifier; pass 0 to load all entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of stock quantity change entries
        /// </returns>
        public virtual async Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(TvChannel tvChannel, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue)
        {
            if (tvChannel == null)
                throw new ArgumentNullException(nameof(tvChannel));

            var query = _stockQuantityHistoryRepository.Table.Where(historyEntry => historyEntry.TvChannelId == tvChannel.Id);

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
        /// Clean up tvChannel references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task ClearDiscountTvChannelMappingAsync(Discount discount)
        {
            if (discount is null)
                throw new ArgumentNullException(nameof(discount));

            var mappingsWithTvChannels =
                from dcm in _discountTvChannelMappingRepository.Table
                join p in _tvChannelRepository.Table on dcm.EntityId equals p.Id
                where dcm.DiscountId == discount.Id
                select new { tvChannel = p, dcm };

            foreach (var pdcm in await mappingsWithTvChannels.ToListAsync())
            {
                await _discountTvChannelMappingRepository.DeleteAsync(pdcm.dcm);

                //update "HasDiscountsApplied" property
                await UpdateHasDiscountsAppliedAsync(pdcm.tvChannel);
            }
        }

        /// <summary>
        /// Get a discount-tvChannel mapping records by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task<IList<DiscountTvChannelMapping>> GetAllDiscountsAppliedToTvChannelAsync(int tvChannelId)
        {
            return await _discountTvChannelMappingRepository.GetAllAsync(query => query.Where(dcm => dcm.EntityId == tvChannelId));
        }

        /// <summary>
        /// Get a discount-tvChannel mapping record
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        public virtual async Task<DiscountTvChannelMapping> GetDiscountAppliedToTvChannelAsync(int tvChannelId, int discountId)
        {
            return await _discountTvChannelMappingRepository.Table
                .FirstOrDefaultAsync(dcm => dcm.EntityId == tvChannelId && dcm.DiscountId == discountId);
        }

        /// <summary>
        /// Inserts a discount-tvChannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvChannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task InsertDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping)
        {
            await _discountTvChannelMappingRepository.InsertAsync(discountTvChannelMapping);
        }

        /// <summary>
        /// Deletes a discount-tvChannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvChannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task DeleteDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping)
        {
            await _discountTvChannelMappingRepository.DeleteAsync(discountTvChannelMapping);
        }

        #endregion

        #endregion
    }
}
