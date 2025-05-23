﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Shipping.Tracking;

namespace TvProgViewer.Services.Shipping
{
    /// <summary>
    /// Shipment service interface
    /// </summary>
    public partial interface IShipmentService
    {
        /// <summary>
        /// Deletes a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteShipmentAsync(Shipment shipment);

        /// <summary>
        /// Search shipments
        /// </summary>
        /// <param name="vendorId">Vendor identifier; 0 to load all records</param>
        /// <param name="warehouseId">Warehouse identifier, only shipments with tvChannels from a specified warehouse will be loaded; 0 to load all orders</param>
        /// <param name="shippingCountryId">Shipping country identifier; 0 to load all records</param>
        /// <param name="shippingStateId">Shipping state identifier; 0 to load all records</param>
        /// <param name="shippingCounty">Shipping county; null to load all records</param>
        /// <param name="shippingCity">Shipping city; null to load all records</param>
        /// <param name="trackingNumber">Search by tracking number</param>
        /// <param name="loadNotShipped">A value indicating whether we should load only not shipped shipments</param>
        /// <param name="loadNotReadyForPickup">A value indicating whether we should load only not ready for pickup shipments</param>
        /// <param name="loadNotDelivered">A value indicating whether we should load only not delivered shipments</param>
        /// <param name="orderId">Order identifier; 0 to load all records</param>
        /// <param name="createdFromUtc">Created date from (UTC); null to load all records</param>
        /// <param name="createdToUtc">Created date to (UTC); null to load all records</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipments
        /// </returns>
        Task<IPagedList<Shipment>> GetAllShipmentsAsync(int vendorId = 0, int warehouseId = 0,
            int shippingCountryId = 0,
            int shippingStateId = 0,
            string shippingCounty = null,
            string shippingCity = null,
            string trackingNumber = null,
            bool loadNotShipped = false,
            bool loadNotReadyForPickup = false,
            bool loadNotDelivered = false,
            int orderId = 0,
            DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int pageIndex = 0, int pageSize = int.MaxValue);

        /// <summary>
        /// Get shipment by identifiers
        /// </summary>
        /// <param name="shipmentIds">Shipment identifiers</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipments
        /// </returns>
        Task<IList<Shipment>> GetShipmentsByIdsAsync(int[] shipmentIds);

        /// <summary>
        /// Gets a shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment
        /// </returns>
        Task<Shipment> GetShipmentByIdAsync(int shipmentId);

        /// <summary>
        /// Gets a list of order shipments
        /// </summary>
        /// <param name="orderId">Order identifier</param>
        /// <param name="shipped">A value indicating whether to count only shipped or not shipped shipments; pass null to ignore</param>
        /// <param name="readyForPickup">A value indicating whether to load only ready for pickup shipments; pass null to ignore</param>
        /// <param name="vendorId">Vendor identifier; pass 0 to ignore</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<IList<Shipment>> GetShipmentsByOrderIdAsync(int orderId, bool? shipped = null, bool? readyForPickup = null, int vendorId = 0);

        /// <summary>
        /// Inserts a shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertShipmentAsync(Shipment shipment);

        /// <summary>
        /// Updates the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateShipmentAsync(Shipment shipment);

        /// <summary>
        /// Gets a shipment items of shipment
        /// </summary>
        /// <param name="shipmentId">Shipment identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment items
        /// </returns>
        Task<IList<ShipmentItem>> GetShipmentItemsByShipmentIdAsync(int shipmentId);

        /// <summary>
        /// Inserts a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertShipmentItemAsync(ShipmentItem shipmentItem);

        /// <summary>
        /// Deletes a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment Item</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteShipmentItemAsync(ShipmentItem shipmentItem);

        /// <summary>
        /// Updates a shipment item
        /// </summary>
        /// <param name="shipmentItem">Shipment item</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateShipmentItemAsync(ShipmentItem shipmentItem);

        /// <summary>
        /// Gets a shipment item
        /// </summary>
        /// <param name="shipmentItemId">Shipment item identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment item
        /// </returns>
        Task<ShipmentItem> GetShipmentItemByIdAsync(int shipmentItemId);

        /// <summary>
        /// Get quantity in shipments. For example, get planned quantity to be shipped
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="warehouseId">Warehouse identifier</param>
        /// <param name="ignoreShipped">Ignore already shipped shipments</param>
        /// <param name="ignoreDelivered">Ignore already delivered shipments</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the quantity
        /// </returns>
        Task<int> GetQuantityInShipmentsAsync(TvChannel tvChannel, int warehouseId,
            bool ignoreShipped, bool ignoreDelivered);

        /// <summary>
        /// Get the tracker of the shipment
        /// </summary>
        /// <param name="shipment">Shipment</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment tracker
        /// </returns>
        Task<IShipmentTracker> GetShipmentTrackerAsync(Shipment shipment);
    }
}