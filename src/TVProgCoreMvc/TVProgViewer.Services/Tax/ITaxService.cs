﻿using System;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Tax;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TVProgViewer.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial interface ITaxService
    {
        #region Product price

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price, User user);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, decimal price,
            bool includingTax, User user);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetProductPriceAsync(Product product, int taxCategoryId, decimal price,
            bool includingTax, User user,
            bool priceIncludesTax);

        #endregion

        #region Shipping price

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, User user);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, User user);

        #endregion

        #region Payment additional fee

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, User user);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, User user);

        #endregion

        #region Checkout attribute price

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, User user);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>Price. Tax rate</returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, User user);

        #endregion

        #region VAT

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>VAT Number status</returns>
        Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber);

        #endregion

        #region Tax total

        /// <summary>
        /// Get tax total for the passed shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>Result</returns>
        Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true);

        #endregion
    }
}