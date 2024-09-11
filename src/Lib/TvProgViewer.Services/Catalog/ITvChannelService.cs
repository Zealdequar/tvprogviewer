using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Discounts;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Stores;

namespace TvProgViewer.Services.Catalog
{
    /// <summary>
    /// TvChannel service
    /// </summary>
    public partial interface ITvChannelService
    {
        #region TvChannels

        /// <summary>
        /// Delete a tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAsync(TvChannel tvChannel);

        /// <summary>
        /// Delete tvChannels
        /// </summary>
        /// <param name="tvChannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelsAsync(IList<TvChannel> tvChannels);

        /// <summary>
        /// Получение всех телеканалов деталей
        /// </summary>
        /// <returns>Список телеканалов</returns>
        Task<IList<TvChannel>> GetAllTvChannelsAsync();

        /// <summary>
        /// Gets all tvChannels displayed on the home page
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<IList<TvChannel>> GetAllTvChannelsDisplayedOnHomepageAsync();

        /// <summary>
        /// Gets featured tvChannels by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvChannels
        /// </returns>
        Task<IList<TvChannel>> GetCategoryFeaturedTvChannelsAsync(int categoryId, int storeId = 0);

        /// <summary>
        /// Gets featured tvChannels by a manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvChannels
        /// </returns>
        Task<IList<TvChannel>> GetManufacturerFeaturedTvChannelsAsync(int manufacturerId, int storeId = 0);

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
        Task<IPagedList<TvChannel>> GetTvChannelsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets tvChannel
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel
        /// </returns>
        Task<TvChannel> GetTvChannelByIdAsync(int tvChannelId);

        /// <summary>
        /// Gets tvChannels by identifier
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<IList<TvChannel>> GetTvChannelsByIdsAsync(int[] tvChannelIds);

        /// <summary>
        /// Inserts a tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAsync(TvChannel tvChannel);

        /// <summary>
        /// Обновление телеканала деталей
        /// </summary>
        /// <param name="tvChannel">Телеканал деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAsync(TvChannel tvChannel);

        /// <summary>
        /// Обновление телеканалов деталей
        /// </summary>
        /// <param name="tvChannels">Список телеканалов деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelListAsync(IList<TvChannel> tvChannels);

        /// <summary>
        /// Get number of tvChannel (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        Task<int> GetNumberOfTvChannelsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0);

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
        Task<IPagedList<TvChannel>> SearchTvChannelsAsync(
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
            bool? overridePublished = null);

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
        Task<IPagedList<TvChannel>> GetTvChannelsByTvChannelAttributeIdAsync(int tvChannelAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue);

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
        Task<IList<TvChannel>> GetAssociatedTvChannelsAsync(int parentGroupedTvChannelId,
            int storeId = 0, int vendorId = 0, bool showHidden = false);

        /// <summary>
        /// Update tvChannel review totals
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelReviewTotalsAsync(TvChannel tvChannel);

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
        Task<IPagedList<TvChannel>> GetLowStockTvChannelsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

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
        Task<IPagedList<TvChannelAttributeCombination>> GetLowStockTvChannelCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets a tvChannel by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel
        /// </returns>
        Task<TvChannel> GetTvChannelBySkuAsync(string sku);

        /// <summary>
        /// Gets a tvChannels by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<IList<TvChannel>> GetTvChannelsBySkuAsync(string[] skuArray, int vendorId = 0);

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateHasTierPricesPropertyAsync(TvChannel tvChannel);

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateHasDiscountsAppliedAsync(TvChannel tvChannel);

        /// <summary>
        /// Gets number of tvChannels by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvChannels
        /// </returns>
        Task<int> GetNumberOfTvChannelsByVendorIdAsync(int vendorId);

        /// <summary>
        /// Parse "required tvChannel Ids" property
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>A list of required tvChannel IDs</returns>
        int[] ParseRequiredTvChannelIds(TvChannel tvChannel);

        /// <summary>
        /// Get a value indicating whether a tvChannel is available now (availability dates)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        bool TvChannelIsAvailable(TvChannel tvChannel, DateTime? dateTime = null);

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <returns>Result</returns>
        int[] ParseAllowedQuantities(TvChannel tvChannel);

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
        Task<int> GetTotalStockQuantityAsync(TvChannel tvChannel, bool useReservedQuantity = true, int warehouseId = 0);

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        int GetRentalPeriods(TvChannel tvChannel, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Selected tvChannel attributes in XML format (if specified)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock message
        /// </returns>
        Task<string> FormatStockMessageAsync(TvChannel tvChannel, string attributesXml);

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU
        /// </returns>
        Task<string> FormatSkuAsync(TvChannel tvChannel, string attributesXml = null);

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer part number
        /// </returns>
        Task<string> FormatMpnAsync(TvChannel tvChannel, string attributesXml = null);

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gTIN
        /// </returns>
        Task<string> FormatGtinAsync(TvChannel tvChannel, string attributesXml = null);

        /// <summary>
        /// Formats start/end date for rental tvChannel
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        string FormatRentalDate(TvChannel tvChannel, DateTime date);

        /// <summary>
        /// Update tvChannel store mappings
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelStoreMappingsAsync(TvChannel tvChannel, IList<int> limitedToStoresIds);

        /// <summary>
        /// Gets the value whether the sequence contains downloadable tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyDownloadableTvChannelAsync(int[] tvChannelIds);

        /// <summary>
        /// Gets the value whether the sequence contains gift card tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyGiftCardTvChannelAsync(int[] tvChannelIds);

        /// <summary>
        /// Gets the value whether the sequence contains recurring tvChannels
        /// </summary>
        /// <param name="tvChannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyRecurringTvChannelAsync(int[] tvChannelIds);

        /// <summary>
        /// Returns a list of sku of not existing tvChannels
        /// </summary>
        /// <param name="tvChannelSku">The sku of the tvChannels to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of sku not existing tvChannels
        /// </returns>
        Task<string[]> GetNotExistingTvChannelsAsync(string[] tvChannelSku);

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
        Task AdjustInventoryAsync(TvChannel tvChannel, int quantityToChange, string attributesXml = "", string message = "");

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task BookReservedInventoryAsync(TvChannel tvChannel, int warehouseId, int quantity, string message = "");

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="tvChannel">tvChannel</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Quantity reversed</returns>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<int> ReverseBookedInventoryAsync(TvChannel tvChannel, ShipmentItem shipmentItem, string message = "");

        #endregion

        #region Related tvChannels

        /// <summary>
        /// Deletes a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Gets related tvChannels by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannels
        /// </returns>
        Task<IList<RelatedTvChannel>> GetRelatedTvChannelsByTvChannelId1Async(int tvChannelId1, bool showHidden = false);

        /// <summary>
        /// Gets a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannelId">Related tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvChannel
        /// </returns>
        Task<RelatedTvChannel> GetRelatedTvChannelByIdAsync(int relatedTvChannelId);

        /// <summary>
        /// Inserts a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Updates a related tvChannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Finds a related tvChannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="tvChannelId2">The second tvChannel identifier</param>
        /// <returns>Related tvChannel</returns>
        RelatedTvChannel FindRelatedTvChannel(IList<RelatedTvChannel> source, int tvChannelId1, int tvChannelId2);

        #endregion

        #region Cross-sell tvChannels

        /// <summary>
        /// Deletes a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel);

        /// <summary>
        /// Gets cross-sell tvChannels by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannels
        /// </returns>
        Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelId1Async(int tvChannelId1, bool showHidden = false);

        /// <summary>
        /// Gets a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannelId">Cross-sell tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvChannel
        /// </returns>
        Task<CrossSellTvChannel> GetCrossSellTvChannelByIdAsync(int crossSellTvChannelId);

        /// <summary>
        /// Inserts a cross-sell tvChannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell tvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel);

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfTvChannels">Number of tvChannels to return</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sells
        /// </returns>
        Task<IList<TvChannel>> GetCrossSellTvChannelsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfTvChannels);

        /// <summary>
        /// Finds a cross-sell tvChannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvChannelId1">The first tvChannel identifier</param>
        /// <param name="tvChannelId2">The second tvChannel identifier</param>
        /// <returns>Cross-sell tvChannel</returns>
        CrossSellTvChannel FindCrossSellTvChannel(IList<CrossSellTvChannel> source, int tvChannelId1, int tvChannelId2);

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tvChannel tier prices for user
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TierPrice>> GetTierPricesAsync(TvChannel tvChannel, User user, Store store);

        /// <summary>
        /// Gets a tier prices by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TierPrice>> GetTierPricesByTvChannelAsync(int tvChannelId);

        /// <summary>
        /// Deletes a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Gets a tier price
        /// </summary>
        /// <param name="tierPriceId">Tier price identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price
        /// </returns>
        Task<TierPrice> GetTierPriceByIdAsync(int tierPriceId);

        /// <summary>
        /// Inserts a tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Updates the tier price
        /// </summary>
        /// <param name="tierPrice">Tier price</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTierPriceAsync(TierPrice tierPrice);

        /// <summary>
        /// Gets a preferred tier price
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price
        /// </returns>
        Task<TierPrice> GetPreferredTierPriceAsync(TvChannel tvChannel, User user, Store store, int quantity);

        #endregion

        #region TvChannel pictures

        /// <summary>
        /// Deletes a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelPictureAsync(TvChannelPicture tvChannelPicture);

        /// <summary>
        /// Gets a tvChannel pictures by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel pictures
        /// </returns>
        Task<IList<TvChannelPicture>> GetTvChannelPicturesByTvChannelIdAsync(int tvChannelId);

        /// <summary>
        /// Gets a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPictureId">TvChannel picture identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel picture
        /// </returns>
        Task<TvChannelPicture> GetTvChannelPictureByIdAsync(int tvChannelPictureId);

        /// <summary>
        /// Inserts a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelPictureAsync(TvChannelPicture tvChannelPicture);

        /// <summary>
        /// Updates a tvChannel picture
        /// </summary>
        /// <param name="tvChannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelPictureAsync(TvChannelPicture tvChannelPicture);

        /// <summary>
        /// Get the IDs of all tvChannel images 
        /// </summary>
        /// <param name="tvChannelsIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the all picture identifiers grouped by tvChannel ID
        /// </returns>
        Task<IDictionary<int, int[]>> GetTvChannelsImagesIdsAsync(int[] tvChannelsIds);

        /// <summary>
        /// Get tvChannels to which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted tvChannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvChannels
        /// </returns>
        Task<IPagedList<TvChannel>> GetTvChannelsWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region TvChannel videos

        /// <summary>
        /// Deletes a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelVideoAsync(TvChannelVideo tvChannelVideo);

        /// <summary>
        /// Gets a tvChannel videos by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">The tvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel videos
        /// </returns>
        Task<IList<TvChannelVideo>> GetTvChannelVideosByTvChannelIdAsync(int tvChannelId);

        /// <summary>
        /// Gets a tvChannel video
        /// </summary>
        /// <param name="tvChannelPictureId">TvChannel video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel video
        /// </returns>
        Task<TvChannelVideo> GetTvChannelVideoByIdAsync(int tvChannelVideoId);

        /// <summary>
        /// Inserts a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelVideoAsync(TvChannelVideo tvChannelVideo);

        /// <summary>
        /// Updates a tvChannel video
        /// </summary>
        /// <param name="tvChannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelVideoAsync(TvChannelVideo tvChannelVideo);

        #endregion

        #region TvChannel reviews

        /// <summary>
        /// Gets all tvChannel reviews
        /// </summary>
        /// <param name="userId">User identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="tvChannelId">The tvChannel identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to tvChannels of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reviews
        /// </returns>
        Task<IPagedList<TvChannelReview>> GetAllTvChannelReviewsAsync(int userId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int tvChannelId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets tvChannel review
        /// </summary>
        /// <param name="tvChannelReviewId">TvChannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel review
        /// </returns>
        Task<TvChannelReview> GetTvChannelReviewByIdAsync(int tvChannelReviewId);

        /// <summary>
        /// Get tvChannel reviews by identifiers
        /// </summary>
        /// <param name="tvChannelReviewIds">TvChannel review identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannel reviews
        /// </returns>
        Task<IList<TvChannelReview>> GetTvChannelReviewsByIdsAsync(int[] tvChannelReviewIds);

        /// <summary>
        /// Inserts a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelReviewAsync(TvChannelReview tvChannelReview);

        /// <summary>
        /// Deletes a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelReviewAsync(TvChannelReview tvChannelReview);

        /// <summary>
        /// Deletes tvChannel reviews
        /// </summary>
        /// <param name="tvChannelReviews">TvChannel reviews</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelReviewsAsync(IList<TvChannelReview> tvChannelReviews);

        /// <summary>
        /// Sets or create a tvChannel review helpfulness record
        /// </summary>
        /// <param name="tvChannelReview">TvChannel reviews</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetTvChannelReviewHelpfulnessAsync(TvChannelReview tvChannelReview, bool helpfulness);

        /// <summary>
        /// Updates a totals helpfulness count for tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task UpdateTvChannelReviewHelpfulnessTotalsAsync(TvChannelReview tvChannelReview);

        /// <summary>
        /// Updates a tvChannel review
        /// </summary>
        /// <param name="tvChannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelReviewAsync(TvChannelReview tvChannelReview);

        /// <summary>
        /// Check possibility added review for current user
        /// </summary>
        /// <param name="tvChannelId">Current tvChannel</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        Task<bool> CanAddReviewAsync(int tvChannelId, int storeId = 0);

        #endregion

        #region TvChannel warehouses

        /// <summary>
        /// Get a tvChannel warehouse-inventory records by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TvChannelWarehouseInventory>> GetAllTvChannelWarehouseInventoryRecordsAsync(int tvChannelId);

        /// <summary>
        /// Deletes a TvChannelWarehouseInventory
        /// </summary>
        /// <param name="pwi">TvChannelWarehouseInventory</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi);

        /// <summary>
        /// Inserts a TvChannelWarehouseInventory
        /// </summary>
        /// <param name="pwi">TvChannelWarehouseInventory</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi);

        /// <summary>
        /// Updates a record to manage tvChannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvChannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi);

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
        Task AddStockQuantityHistoryEntryAsync(TvChannel tvChannel, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null);

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
        Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(TvChannel tvChannel, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region TvChannel discounts

        /// <summary>
        /// Clean up tvChannel references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ClearDiscountTvChannelMappingAsync(Discount discount);

        /// <summary>
        /// Get a discount-tvChannel mapping records by tvChannel identifier
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<DiscountTvChannelMapping>> GetAllDiscountsAppliedToTvChannelAsync(int tvChannelId);

        /// <summary>
        /// Get a discount-tvChannel mapping record
        /// </summary>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<DiscountTvChannelMapping> GetDiscountAppliedToTvChannelAsync(int tvChannelId, int discountId);

        /// <summary>
        /// Inserts a discount-tvChannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvChannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping);

        /// <summary>
        /// Deletes a discount-tvChannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvChannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping);

        #endregion
    }
}