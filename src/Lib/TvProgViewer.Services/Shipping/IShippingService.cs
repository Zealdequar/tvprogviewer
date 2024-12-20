﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Services.Shipping.Pickup;

namespace TvProgViewer.Services.Shipping
{
    /// <summary>
    /// Shipping service interface
    /// </summary>
    public partial interface IShippingService
    {
        #region Shipping methods

        /// <summary>
        /// Deletes a shipping method
        /// </summary>
        /// <param name="shippingMethod">The shipping method</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Gets a shipping method
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method
        /// </returns>
        Task<ShippingMethod> GetShippingMethodByIdAsync(int shippingMethodId);

        /// <summary>
        /// Gets all shipping methods
        /// </summary>
        /// <param name="filterByCountryId">The country identifier to filter by</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping methods
        /// </returns>
        Task<IList<ShippingMethod>> GetAllShippingMethodsAsync(int? filterByCountryId = null);

        /// <summary>
        /// Inserts a shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Updates the shipping method
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateShippingMethodAsync(ShippingMethod shippingMethod);

        /// <summary>
        /// Does country restriction exist
        /// </summary>
        /// <param name="shippingMethod">Shipping method</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> CountryRestrictionExistsAsync(ShippingMethod shippingMethod, int countryId);

        /// <summary>
        /// Gets shipping country mappings
        /// </summary>
        /// <param name="shippingMethodId">The shipping method identifier</param>
        /// <param name="countryId">Country identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping country mappings
        /// </returns>
        Task<IList<ShippingMethodCountryMapping>> GetShippingMethodCountryMappingAsync(int shippingMethodId, int countryId);

        /// <summary>
        /// Inserts a shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping);

        /// <summary>
        /// Delete the shipping country mapping
        /// </summary>
        /// <param name="shippingMethodCountryMapping">Shipping country mapping</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteShippingMethodCountryMappingAsync(ShippingMethodCountryMapping shippingMethodCountryMapping);

        #endregion

        #region Warehouses

        /// <summary>
        /// Deletes a warehouse
        /// </summary>
        /// <param name="warehouse">The warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task DeleteWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// Gets a warehouse
        /// </summary>
        /// <param name="warehouseId">The warehouse identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warehouse
        /// </returns>
        Task<Warehouse> GetWarehouseByIdAsync(int warehouseId);

        /// <summary>
        /// Gets all warehouses
        /// </summary>
        /// <param name="name">Warehouse name</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the warehouses
        /// </returns>
        Task<IList<Warehouse>> GetAllWarehousesAsync(string name = null);

        /// <summary>
        /// Inserts a warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task InsertWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// Updates the warehouse
        /// </summary>
        /// <param name="warehouse">Warehouse</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task UpdateWarehouseAsync(Warehouse warehouse);

        /// <summary>
        /// Get the nearest warehouse for the specified address
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the 
        /// </returns>
        Task<Warehouse> GetNearestWarehouseAsync(Address address, IList<Warehouse> warehouses = null);

        #endregion

        #region Workflow

        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the tvChannels marked as "Free shipping"</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shopping cart item weight
        /// </returns>
        Task<decimal> GetShoppingCartItemWeightAsync(ShoppingCartItem shoppingCartItem, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets tvChannel item weight (of one item)
        /// </summary>
        /// <param name="tvChannel">TvChannel</param>
        /// <param name="attributesXml">Selected tvChannel attributes in XML</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the tvChannels marked as "Free shipping"</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the item weight
        /// </returns>
        Task<decimal> GetShoppingCartItemWeightAsync(TvChannel tvChannel, string attributesXml, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkout attributes</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the tvChannels marked as "Free shipping"</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the otal weight
        /// </returns>
        Task<decimal> GetTotalWeightAsync(GetShippingOptionRequest request, bool includeCheckoutAttributes = true, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Get total dimensions
        /// </summary>
        /// <param name="packageItems">Package items</param>
        /// <param name="ignoreFreeShippedItems">Whether to ignore the weight of the tvChannels marked as "Free shipping"</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the width. Length. Height
        /// </returns>
        Task<(decimal width, decimal length, decimal height)> GetDimensionsAsync(IList<GetShippingOptionRequest.PackageItem> packageItems, bool ignoreFreeShippedItems = false);

        /// <summary>
        /// Create shipment packages (requests) from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipment packages (requests). Value indicating whether shipping is done from multiple locations (warehouses)
        /// </returns>
        Task<(IList<GetShippingOptionRequest> shipmentPackages, bool shippingFromMultipleLocations)> CreateShippingOptionRequestsAsync(IList<ShoppingCartItem> cart,
            Address shippingAddress, int storeId);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping options
        /// </returns>
        Task<GetShippingOptionResponse> GetShippingOptionsAsync(IList<ShoppingCartItem> cart, Address shippingAddress,
            User user = null, string allowedShippingRateComputationMethodSystemName = "", int storeId = 0);

        /// <summary>
        /// Gets available pickup points
        /// </summary>
        /// <param name="cart">Shopping Cart</param>
        /// <param name="address">Address</param>
        /// <param name="user">Load records allowed only to a specified user; pass null to ignore ACL permissions</param>
        /// <param name="providerSystemName">Filter by provider identifier; null to load pickup points of all providers</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the pickup points
        /// </returns>
        Task<GetPickupPointsResponse> GetPickupPointsAsync(IList<ShoppingCartItem> cart, Address address,
            User user = null, string providerSystemName = null, int storeId = 0);

        /// <summary>
        /// Whether the shopping cart item is ship enabled
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if the shopping cart item requires shipping; otherwise false
        /// </returns>
        Task<bool> IsShipEnabledAsync(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Whether the shopping cart item is free shipping
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the rue if the shopping cart item is free shipping; otherwise false
        /// </returns>
        Task<bool> IsFreeShippingAsync(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Get the additional shipping charge
        /// </summary> 
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the additional shipping charge of the shopping cart item
        /// </returns>
        Task<decimal> GetAdditionalShippingChargeAsync(ShoppingCartItem shoppingCartItem);

        #endregion
    }
}