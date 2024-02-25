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
        /// Delete a tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelAsync(TvChannel tvchannel);

        /// <summary>
        /// Delete tvchannels
        /// </summary>
        /// <param name="tvchannels">TvChannels</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelsAsync(IList<TvChannel> tvchannels);

        /// <summary>
        /// Получение всех телеканалов деталей
        /// </summary>
        /// <returns>Список телеканалов</returns>
        Task<IList<TvChannel>> GetAllTvChannelsAsync();

        /// <summary>
        /// Gets all tvchannels displayed on the home page
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        Task<IList<TvChannel>> GetAllTvChannelsDisplayedOnHomepageAsync();

        /// <summary>
        /// Gets featured tvchannels by a category identifier
        /// </summary>
        /// <param name="categoryId">Category identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvchannels
        /// </returns>
        Task<IList<TvChannel>> GetCategoryFeaturedTvChannelsAsync(int categoryId, int storeId = 0);

        /// <summary>
        /// Gets featured tvchannels by a manufacturer identifier
        /// </summary>
        /// <param name="manufacturerId">Manufacturer identifier</param>
        /// <param name="storeId">Store identifier; 0 if you want to get all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of featured tvchannels
        /// </returns>
        Task<IList<TvChannel>> GetManufacturerFeaturedTvChannelsAsync(int manufacturerId, int storeId = 0);

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
        Task<IPagedList<TvChannel>> GetTvChannelsMarkedAsNewAsync(int storeId = 0, int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets tvchannel
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel
        /// </returns>
        Task<TvChannel> GetTvChannelByIdAsync(int tvchannelId);

        /// <summary>
        /// Gets tvchannels by identifier
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        Task<IList<TvChannel>> GetTvChannelsByIdsAsync(int[] tvchannelIds);

        /// <summary>
        /// Inserts a tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelAsync(TvChannel tvchannel);

        /// <summary>
        /// Обновление телеканала деталей
        /// </summary>
        /// <param name="tvchannel">Телеканал деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelAsync(TvChannel tvchannel);

        /// <summary>
        /// Обновление телеканалов деталей
        /// </summary>
        /// <param name="tvChannels">Список телеканалов деталей</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelListAsync(IList<TvChannel> tvChannels);

        /// <summary>
        /// Get number of tvchannel (published and visible) in certain category
        /// </summary>
        /// <param name="categoryIds">Category identifiers</param>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvchannels
        /// </returns>
        Task<int> GetNumberOfTvChannelsInCategoryAsync(IList<int> categoryIds = null, int storeId = 0);

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
        Task<IPagedList<TvChannel>> SearchTvChannelsAsync(
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
            bool? overridePublished = null);

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
        Task<IPagedList<TvChannel>> GetTvChannelsByTvChannelAttributeIdAsync(int tvchannelAttributeId,
            int pageIndex = 0, int pageSize = int.MaxValue);

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
        Task<IList<TvChannel>> GetAssociatedTvChannelsAsync(int parentGroupedTvChannelId,
            int storeId = 0, int vendorId = 0, bool showHidden = false);

        /// <summary>
        /// Update tvchannel review totals
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelReviewTotalsAsync(TvChannel tvchannel);

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
        Task<IPagedList<TvChannel>> GetLowStockTvChannelsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

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
        Task<IPagedList<TvChannelAttributeCombination>> GetLowStockTvChannelCombinationsAsync(int? vendorId = null, bool? loadPublishedOnly = true,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Gets a tvchannel by SKU
        /// </summary>
        /// <param name="sku">SKU</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel
        /// </returns>
        Task<TvChannel> GetTvChannelBySkuAsync(string sku);

        /// <summary>
        /// Gets a tvchannels by SKU array
        /// </summary>
        /// <param name="skuArray">SKU array</param>
        /// <param name="vendorId">Vendor ID; 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannels
        /// </returns>
        Task<IList<TvChannel>> GetTvChannelsBySkuAsync(string[] skuArray, int vendorId = 0);

        /// <summary>
        /// Update HasTierPrices property (used for performance optimization)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateHasTierPricesPropertyAsync(TvChannel tvchannel);

        /// <summary>
        /// Update HasDiscountsApplied property (used for performance optimization)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateHasDiscountsAppliedAsync(TvChannel tvchannel);

        /// <summary>
        /// Gets number of tvchannels by vendor identifier
        /// </summary>
        /// <param name="vendorId">Vendor identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the number of tvchannels
        /// </returns>
        Task<int> GetNumberOfTvChannelsByVendorIdAsync(int vendorId);

        /// <summary>
        /// Parse "required tvchannel Ids" property
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>A list of required tvchannel IDs</returns>
        int[] ParseRequiredTvChannelIds(TvChannel tvchannel);

        /// <summary>
        /// Get a value indicating whether a tvchannel is available now (availability dates)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="dateTime">Datetime to check; pass null to use current date</param>
        /// <returns>Result</returns>
        bool TvChannelIsAvailable(TvChannel tvchannel, DateTime? dateTime = null);

        /// <summary>
        /// Get a list of allowed quantities (parse 'AllowedQuantities' property)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <returns>Result</returns>
        int[] ParseAllowedQuantities(TvChannel tvchannel);

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
        Task<int> GetTotalStockQuantityAsync(TvChannel tvchannel, bool useReservedQuantity = true, int warehouseId = 0);

        /// <summary>
        /// Get number of rental periods (price ratio)
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <returns>Number of rental periods</returns>
        int GetRentalPeriods(TvChannel tvchannel, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Formats the stock availability/quantity message
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Selected tvchannel attributes in XML format (if specified)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the stock message
        /// </returns>
        Task<string> FormatStockMessageAsync(TvChannel tvchannel, string attributesXml);

        /// <summary>
        /// Formats SKU
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the sKU
        /// </returns>
        Task<string> FormatSkuAsync(TvChannel tvchannel, string attributesXml = null);

        /// <summary>
        /// Formats manufacturer part number
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the manufacturer part number
        /// </returns>
        Task<string> FormatMpnAsync(TvChannel tvchannel, string attributesXml = null);

        /// <summary>
        /// Formats GTIN
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the gTIN
        /// </returns>
        Task<string> FormatGtinAsync(TvChannel tvchannel, string attributesXml = null);

        /// <summary>
        /// Formats start/end date for rental tvchannel
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="date">Date</param>
        /// <returns>Formatted date</returns>
        string FormatRentalDate(TvChannel tvchannel, DateTime date);

        /// <summary>
        /// Update tvchannel store mappings
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="limitedToStoresIds">A list of store ids for mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelStoreMappingsAsync(TvChannel tvchannel, IList<int> limitedToStoresIds);

        /// <summary>
        /// Gets the value whether the sequence contains downloadable tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyDownloadableTvChannelAsync(int[] tvchannelIds);

        /// <summary>
        /// Gets the value whether the sequence contains gift card tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyGiftCardTvChannelAsync(int[] tvchannelIds);

        /// <summary>
        /// Gets the value whether the sequence contains recurring tvchannels
        /// </summary>
        /// <param name="tvchannelIds">TvChannel identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> HasAnyRecurringTvChannelAsync(int[] tvchannelIds);

        /// <summary>
        /// Returns a list of sku of not existing tvchannels
        /// </summary>
        /// <param name="tvchannelSku">The sku of the tvchannels to check</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of sku not existing tvchannels
        /// </returns>
        Task<string[]> GetNotExistingTvChannelsAsync(string[] tvchannelSku);

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
        Task AdjustInventoryAsync(TvChannel tvchannel, int quantityToChange, string attributesXml = "", string message = "");

        /// <summary>
        /// Book the reserved quantity
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="quantity">Quantity, must be negative</param>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task BookReservedInventoryAsync(TvChannel tvchannel, int warehouseId, int quantity, string message = "");

        /// <summary>
        /// Reverse booked inventory (if acceptable)
        /// </summary>
        /// <param name="tvchannel">tvchannel</param>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Quantity reversed</returns>
        /// <param name="message">Message for the stock quantity history</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<int> ReverseBookedInventoryAsync(TvChannel tvchannel, ShipmentItem shipmentItem, string message = "");

        #endregion

        #region Related tvchannels

        /// <summary>
        /// Deletes a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Gets related tvchannels by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannels
        /// </returns>
        Task<IList<RelatedTvChannel>> GetRelatedTvChannelsByTvChannelId1Async(int tvchannelId1, bool showHidden = false);

        /// <summary>
        /// Gets a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannelId">Related tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the related tvchannel
        /// </returns>
        Task<RelatedTvChannel> GetRelatedTvChannelByIdAsync(int relatedTvChannelId);

        /// <summary>
        /// Inserts a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Updates a related tvchannel
        /// </summary>
        /// <param name="relatedTvChannel">Related tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateRelatedTvChannelAsync(RelatedTvChannel relatedTvChannel);

        /// <summary>
        /// Finds a related tvchannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="tvchannelId2">The second tvchannel identifier</param>
        /// <returns>Related tvchannel</returns>
        RelatedTvChannel FindRelatedTvChannel(IList<RelatedTvChannel> source, int tvchannelId1, int tvchannelId2);

        #endregion

        #region Cross-sell tvchannels

        /// <summary>
        /// Deletes a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel);

        /// <summary>
        /// Gets cross-sell tvchannels by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannels
        /// </returns>
        Task<IList<CrossSellTvChannel>> GetCrossSellTvChannelsByTvChannelId1Async(int tvchannelId1, bool showHidden = false);

        /// <summary>
        /// Gets a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannelId">Cross-sell tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sell tvchannel
        /// </returns>
        Task<CrossSellTvChannel> GetCrossSellTvChannelByIdAsync(int crossSellTvChannelId);

        /// <summary>
        /// Inserts a cross-sell tvchannel
        /// </summary>
        /// <param name="crossSellTvChannel">Cross-sell tvchannel</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertCrossSellTvChannelAsync(CrossSellTvChannel crossSellTvChannel);

        /// <summary>
        /// Gets a cross-sells
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="numberOfTvChannels">Number of tvchannels to return</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the cross-sells
        /// </returns>
        Task<IList<TvChannel>> GetCrossSellTvChannelsByShoppingCartAsync(IList<ShoppingCartItem> cart, int numberOfTvChannels);

        /// <summary>
        /// Finds a cross-sell tvchannel item by specified identifiers
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="tvchannelId1">The first tvchannel identifier</param>
        /// <param name="tvchannelId2">The second tvchannel identifier</param>
        /// <returns>Cross-sell tvchannel</returns>
        CrossSellTvChannel FindCrossSellTvChannel(IList<CrossSellTvChannel> source, int tvchannelId1, int tvchannelId2);

        #endregion

        #region Tier prices

        /// <summary>
        /// Gets a tvchannel tier prices for user
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TierPrice>> GetTierPricesAsync(TvChannel tvchannel, User user, Store store);

        /// <summary>
        /// Gets a tier prices by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TierPrice>> GetTierPricesByTvChannelAsync(int tvchannelId);

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
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <param name="store">Store</param>
        /// <param name="quantity">Quantity</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the ier price
        /// </returns>
        Task<TierPrice> GetPreferredTierPriceAsync(TvChannel tvchannel, User user, Store store, int quantity);

        #endregion

        #region TvChannel pictures

        /// <summary>
        /// Deletes a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelPictureAsync(TvChannelPicture tvchannelPicture);

        /// <summary>
        /// Gets a tvchannel pictures by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel pictures
        /// </returns>
        Task<IList<TvChannelPicture>> GetTvChannelPicturesByTvChannelIdAsync(int tvchannelId);

        /// <summary>
        /// Gets a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPictureId">TvChannel picture identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel picture
        /// </returns>
        Task<TvChannelPicture> GetTvChannelPictureByIdAsync(int tvchannelPictureId);

        /// <summary>
        /// Inserts a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelPictureAsync(TvChannelPicture tvchannelPicture);

        /// <summary>
        /// Updates a tvchannel picture
        /// </summary>
        /// <param name="tvchannelPicture">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelPictureAsync(TvChannelPicture tvchannelPicture);

        /// <summary>
        /// Get the IDs of all tvchannel images 
        /// </summary>
        /// <param name="tvchannelsIds">TvChannels IDs</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the all picture identifiers grouped by tvchannel ID
        /// </returns>
        Task<IDictionary<int, int[]>> GetTvChannelsImagesIdsAsync(int[] tvchannelsIds);

        /// <summary>
        /// Get tvchannels to which a discount is applied
        /// </summary>
        /// <param name="discountId">Discount identifier; pass null to load all records</param>
        /// <param name="showHidden">A value indicating whether to load deleted tvchannels</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of tvchannels
        /// </returns>
        Task<IPagedList<TvChannel>> GetTvChannelsWithAppliedDiscountAsync(int? discountId = null,
            bool showHidden = false, int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region TvChannel videos

        /// <summary>
        /// Deletes a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelVideoAsync(TvChannelVideo tvchannelVideo);

        /// <summary>
        /// Gets a tvchannel videos by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">The tvchannel identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel videos
        /// </returns>
        Task<IList<TvChannelVideo>> GetTvChannelVideosByTvChannelIdAsync(int tvchannelId);

        /// <summary>
        /// Gets a tvchannel video
        /// </summary>
        /// <param name="tvchannelPictureId">TvChannel video identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel video
        /// </returns>
        Task<TvChannelVideo> GetTvChannelVideoByIdAsync(int tvchannelVideoId);

        /// <summary>
        /// Inserts a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel picture</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelVideoAsync(TvChannelVideo tvchannelVideo);

        /// <summary>
        /// Updates a tvchannel video
        /// </summary>
        /// <param name="tvchannelVideo">TvChannel video</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelVideoAsync(TvChannelVideo tvchannelVideo);

        #endregion

        #region TvChannel reviews

        /// <summary>
        /// Gets all tvchannel reviews
        /// </summary>
        /// <param name="userId">User identifier (who wrote a review); 0 to load all records</param>
        /// <param name="approved">A value indicating whether to content is approved; null to load all records</param> 
        /// <param name="fromUtc">Item creation from; null to load all records</param>
        /// <param name="toUtc">Item creation to; null to load all records</param>
        /// <param name="message">Search title or review text; null to load all records</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <param name="tvchannelId">The tvchannel identifier; pass 0 to load all records</param>
        /// <param name="vendorId">The vendor identifier (limit to tvchannels of this vendor); pass 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the reviews
        /// </returns>
        Task<IPagedList<TvChannelReview>> GetAllTvChannelReviewsAsync(int userId = 0, bool? approved = null,
            DateTime? fromUtc = null, DateTime? toUtc = null,
            string message = null, int storeId = 0, int tvchannelId = 0, int vendorId = 0, bool showHidden = false,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Gets tvchannel review
        /// </summary>
        /// <param name="tvchannelReviewId">TvChannel review identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel review
        /// </returns>
        Task<TvChannelReview> GetTvChannelReviewByIdAsync(int tvchannelReviewId);

        /// <summary>
        /// Get tvchannel reviews by identifiers
        /// </summary>
        /// <param name="tvchannelReviewIds">TvChannel review identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvchannel reviews
        /// </returns>
        Task<IList<TvChannelReview>> GetTvChannelReviewsByIdsAsync(int[] tvchannelReviewIds);

        /// <summary>
        /// Inserts a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertTvChannelReviewAsync(TvChannelReview tvchannelReview);

        /// <summary>
        /// Deletes a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelReviewAsync(TvChannelReview tvchannelReview);

        /// <summary>
        /// Deletes tvchannel reviews
        /// </summary>
        /// <param name="tvchannelReviews">TvChannel reviews</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteTvChannelReviewsAsync(IList<TvChannelReview> tvchannelReviews);

        /// <summary>
        /// Sets or create a tvchannel review helpfulness record
        /// </summary>
        /// <param name="tvchannelReview">TvChannel reviews</param>
        /// <param name="helpfulness">Value indicating whether a review a helpful</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SetTvChannelReviewHelpfulnessAsync(TvChannelReview tvchannelReview, bool helpfulness);

        /// <summary>
        /// Updates a totals helpfulness count for tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task UpdateTvChannelReviewHelpfulnessTotalsAsync(TvChannelReview tvchannelReview);

        /// <summary>
        /// Updates a tvchannel review
        /// </summary>
        /// <param name="tvchannelReview">TvChannel review</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelReviewAsync(TvChannelReview tvchannelReview);

        /// <summary>
        /// Check possibility added review for current user
        /// </summary>
        /// <param name="tvchannelId">Current tvchannel</param>
        /// <param name="storeId">The store identifier; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        Task<bool> CanAddReviewAsync(int tvchannelId, int storeId = 0);

        #endregion

        #region TvChannel warehouses

        /// <summary>
        /// Get a tvchannel warehouse-inventory records by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<TvChannelWarehouseInventory>> GetAllTvChannelWarehouseInventoryRecordsAsync(int tvchannelId);

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
        /// Updates a record to manage tvchannel inventory per warehouse
        /// </summary>
        /// <param name="pwi">Record to manage tvchannel inventory per warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateTvChannelWarehouseInventoryAsync(TvChannelWarehouseInventory pwi);

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
        Task AddStockQuantityHistoryEntryAsync(TvChannel tvchannel, int quantityAdjustment, int stockQuantity,
            int warehouseId = 0, string message = "", int? combinationId = null);

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
        Task<IPagedList<StockQuantityHistory>> GetStockQuantityHistoryAsync(TvChannel tvchannel, int warehouseId = 0, int combinationId = 0,
            int pageIndex = 0, int pageSize = int.MaxValue);

        #endregion

        #region TvChannel discounts

        /// <summary>
        /// Clean up tvchannel references for a specified discount
        /// </summary>
        /// <param name="discount">Discount</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task ClearDiscountTvChannelMappingAsync(Discount discount);

        /// <summary>
        /// Get a discount-tvchannel mapping records by tvchannel identifier
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task<IList<DiscountTvChannelMapping>> GetAllDiscountsAppliedToTvChannelAsync(int tvchannelId);

        /// <summary>
        /// Get a discount-tvchannel mapping record
        /// </summary>
        /// <param name="tvchannelId">TvChannel identifier</param>
        /// <param name="discountId">Discount identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<DiscountTvChannelMapping> GetDiscountAppliedToTvChannelAsync(int tvchannelId, int discountId);

        /// <summary>
        /// Inserts a discount-tvchannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvchannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping);

        /// <summary>
        /// Deletes a discount-tvchannel mapping record
        /// </summary>
        /// <param name="discountTvChannelMapping">Discount-tvchannel mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteDiscountTvChannelMappingAsync(DiscountTvChannelMapping discountTvChannelMapping);

        #endregion
    }
}