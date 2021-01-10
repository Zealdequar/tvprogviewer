using System.Collections.Generic;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Directory;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Shipping;

namespace TVProgViewer.Services.Shipping
{
    /// <summary>
    /// Represents a request for getting shipping rate options
    /// </summary>
    public partial class GetShippingOptionRequest
    {
        #region Ctor

        public GetShippingOptionRequest()
        {
            Items = new List<PackageItem>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a User
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets a shopping cart items
        /// </summary>
        public IList<PackageItem> Items { get; set; }

        /// <summary>
        /// Gets or sets a shipping address (where we ship to)
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        /// Shipped from warehouse
        /// </summary>
        public Warehouse WarehouseFrom { get; set; }

        /// <summary>
        /// Shipped from country
        /// </summary>
        public Country CountryFrom { get; set; }

        /// <summary>
        /// Shipped from state/province
        /// </summary>
        public StateProvince StateProvinceFrom { get; set; }

        /// <summary>
        /// Shipped from zip/postal code
        /// </summary>
        public string ZipPostalCodeFrom { get; set; }

        /// <summary>
        /// Shipped from county
        /// </summary>
        public string CountyFrom { get; set; }

        /// <summary>
        /// Shipped from city
        /// </summary>
        public string CityFrom { get; set; }

        /// <summary>
        /// Shipped from address
        /// </summary>
        public string AddressFrom { get; set; }

        /// <summary>
        /// Limit to store (identifier)
        /// </summary>
        public int StoreId { get; set; }

        #endregion

        #region Nested classes

        /// <summary>
        /// Package item
        /// </summary>
        public class PackageItem
        {
            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="sci">Shopping cart item</param>
            /// <param name="qty">Override "Quantity" property of shopping cart item</param>
            public PackageItem(ShoppingCartItem sci, Product product, int? qty = null)
            {
                ShoppingCartItem = sci;
                Product = product;
                OverriddenQuantity = qty;
            }

            /// <summary>
            /// Shopping cart item
            /// </summary>
            public ShoppingCartItem ShoppingCartItem { get; set; }

            /// <summary>
            /// Product
            /// </summary>
            public Product Product { get; set; }

            /// <summary>
            /// If specified, override "Quantity" property of "ShoppingCartItem
            /// </summary>
            public int? OverriddenQuantity { get; set; }

            /// <summary>
            /// Get quantity
            /// </summary>
            /// <returns></returns>
            public int GetQuantity()
            {
                if (OverriddenQuantity.HasValue)
                    return OverriddenQuantity.Value;

                return ShoppingCartItem.Quantity;
            }
        }

        #endregion
    }
}