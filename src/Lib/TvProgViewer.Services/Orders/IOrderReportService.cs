﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Payments;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Services.Orders
{
    /// <summary>
    /// Order report service interface
    /// </summary>
    public partial interface IOrderReportService
    {
        /// <summary>
        /// Get "order by country" report
        /// </summary>
        /// <param name="storeId">Store identifier; 0 to load all records</param>
        /// <param name="os">Order status</param>
        /// <param name="ps">Payment status</param>
        /// <param name="ss">Shipping status</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<IList<OrderByCountryReportLine>> GetCountryReportAsync(int storeId = 0, OrderStatus? os = null,
            PaymentStatus? ps = null, ShippingStatus? ss = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null);

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="tvChannelId">TvChannel identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="osIds">Order status identifiers</param>
        /// <param name="psIds">Payment status identifiers</param>
        /// <param name="ssIds">Shipping status identifiers</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="billingSmartPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<OrderAverageReportLine> GetOrderAverageReportLineAsync(int storeId = 0, int vendorId = 0, int tvChannelId = 0,
            int warehouseId = 0, int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingSmartPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null);

        /// <summary>
        /// Get order average report
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="os">Order status</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<OrderAverageReportLineSummary> OrderAverageReportAsync(int storeId, OrderStatus os);

        /// <summary>
        /// Get sales summary report
        /// </summary>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="tvChannelId">TvChannel identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="groupBy">0 - group by day, 1 - group by week, 2 - group by total month</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<IPagedList<SalesSummaryReportLine>> SalesSummaryReportAsync(
            int categoryId = 0,
            int tvChannelId = 0,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            OrderStatus? os = null,
            PaymentStatus? ps = null,
            int billingCountryId = 0,
            GroupByOptions groupBy = GroupByOptions.Day,
            int pageIndex = 0,
            int pageSize = int.MaxValue);

        /// <summary>
        /// Get best sellers report
        /// </summary>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="orderBy">1 - order by quantity, 2 - order by total amount</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<IPagedList<BestsellersReportLine>> BestSellersReportAsync(
            int categoryId = 0,
            int manufacturerId = 0, 
            int storeId = 0,
            int vendorId = 0,
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            OrderStatus? os = null,
            PaymentStatus? ps = null,
            ShippingStatus? ss = null,
            int billingCountryId = 0,
            OrderByEnum orderBy = OrderByEnum.OrderByQuantity,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool showHidden = false);
        
        /// <summary>
        /// Get a total amount of best sellers
        /// </summary>
        /// <param name="storeId">Store identifier (orders placed in a specific store); 0 to load all records</param>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="os">Order status; null to load all records</param>
        /// <param name="ps">Order payment status; null to load all records</param>
        /// <param name="ss">Shipping status; null to load all records</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all records</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<decimal> BestSellersReportTotalAmountAsync(
            int categoryId = 0,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            DateTime? createdFromUtc = null,
            DateTime? createdToUtc = null,
            OrderStatus? os = null,
            PaymentStatus? ps = null,
            ShippingStatus? ss = null,
            int billingCountryId = 0,
            bool showHidden = false);
            
        /// <summary>
        /// Gets a list of tvChannels (identifiers) purchased by other users who purchased a specified tvChannel
        /// </summary>
        /// <param name="storeId">Store identifier</param>
        /// <param name="tvChannelId">TvChannel identifier</param>
        /// <param name="recordsToReturn">Records to return</param>
        /// <param name="visibleIndividuallyOnly">A values indicating whether to load only tvChannels marked as "visible individually"; "false" to load all records; "true" to load "visible individually" only</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<int[]> GetAlsoPurchasedTvChannelsIdsAsync(int storeId, int tvChannelId,
            int recordsToReturn = 5, bool visibleIndividuallyOnly = true, bool showHidden = false);

        /// <summary>
        /// Gets a list of tvChannels that were never sold
        /// </summary>
        /// <param name="vendorId">Vendor identifier (filter tvChannels by a specific vendor); 0 to load all records</param>
        /// <param name="storeId">Store identifier (filter tvChannels by a specific store); 0 to load all records</param>
        /// <param name="categoryId">Category identifier; 0 to load all records</param>
        /// <param name="manufacturerId">Manufacturer identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Order created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Order created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the tvChannels
        /// </returns>
        Task<IPagedList<TvChannel>> TvChannelsNeverSoldAsync(int vendorId = 0, int storeId = 0,
            int categoryId = 0, int manufacturerId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false);

        /// <summary>
        /// Get profit report
        /// </summary>
        /// <param name="storeId">Store identifier; pass 0 to ignore this parameter</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore this parameter</param>
        /// <param name="tvChannelId">TvChannel identifier which was purchased in an order; 0 to load all orders</param>
        /// <param name="warehouseId">Warehouse identifier; pass 0 to ignore this parameter</param>
        /// <param name="orderId">Order identifier; pass 0 to ignore this parameter</param>
        /// <param name="billingCountryId">Billing country identifier; 0 to load all orders</param>
        /// <param name="paymentMethodSystemName">Payment method system name; null to load all records</param>
        /// <param name="startTimeUtc">Start date</param>
        /// <param name="endTimeUtc">End date</param>
        /// <param name="osIds">Order status identifiers; null to load all records</param>
        /// <param name="psIds">Payment status identifiers; null to load all records</param>
        /// <param name="ssIds">Shipping status identifiers; null to load all records</param>
        /// <param name="billingSmartPhone">Billing phone. Leave empty to load all records.</param>
        /// <param name="billingEmail">Billing email. Leave empty to load all records.</param>
        /// <param name="billingLastName">Billing last name. Leave empty to load all records.</param>
        /// <param name="orderNotes">Search in order notes. Leave empty to load all records.</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<decimal> ProfitReportAsync(int storeId = 0, int vendorId = 0, int tvChannelId = 0,
            int warehouseId = 0, int billingCountryId = 0, int orderId = 0, string paymentMethodSystemName = null,
            List<int> osIds = null, List<int> psIds = null, List<int> ssIds = null,
            DateTime? startTimeUtc = null, DateTime? endTimeUtc = null,
            string billingSmartPhone = null, string billingEmail = null, string billingLastName = "", string orderNotes = null);
    }
}
