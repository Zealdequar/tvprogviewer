using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Tax;

namespace TvProgViewer.Services.Tax
{
    /// <summary>
    /// Tax service
    /// </summary>
    public partial interface ITaxService
    {
        #region TvChannel price

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price, User user);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, decimal price,
            bool includingTax, User user);

        /// <summary>
        /// Gets price
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="taxCategoryId">Tax category identifier</param>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <param name="priceIncludesTax">A value indicating whether price already includes tax</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetTvChannelPriceAsync(TvChannel tvchannel, int taxCategoryId, decimal price,
            bool includingTax, User user,
            bool priceIncludesTax);

        /// <summary>
        /// Gets a value indicating whether a tvchannel is tax exempt
        /// </summary>
        /// <param name="tvchannel">TvChannel</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a value indicating whether a tvchannel is tax exempt
        /// </returns>
        Task<bool> IsTaxExemptAsync(TvChannel tvchannel, User user);

        #endregion

        #region Shipping price

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, User user);

        /// <summary>
        /// Gets shipping price
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetShippingPriceAsync(decimal price, bool includingTax, User user);

        #endregion

        #region Payment additional fee

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, User user);

        /// <summary>
        /// Gets payment method additional handling fee
        /// </summary>
        /// <param name="price">Price</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetPaymentMethodAdditionalFeeAsync(decimal price, bool includingTax, User user);

        #endregion

        #region Checkout attribute price

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav, User user);

        /// <summary>
        /// Gets checkout attribute value price
        /// </summary>
        /// <param name="ca">Checkout attribute</param>
        /// <param name="cav">Checkout attribute value</param>
        /// <param name="includingTax">A value indicating whether calculated price should include tax</param>
        /// <param name="user">User</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the price. Tax rate
        /// </returns>
        Task<(decimal price, decimal taxRate)> GetCheckoutAttributePriceAsync(CheckoutAttribute ca, CheckoutAttributeValue cav,
            bool includingTax, User user);

        #endregion

        #region VAT

        /// <summary>
        /// Gets VAT Number status
        /// </summary>
        /// <param name="fullVatNumber">Two letter ISO code of a country and VAT number (e.g. GB 111 1111 111)</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the vAT Number status
        /// </returns>
        Task<(VatNumberStatus vatNumberStatus, string name, string address)> GetVatNumberStatusAsync(string fullVatNumber);

        #endregion

        #region Tax total

        /// <summary>
        /// Get tax total for the passed shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="usePaymentMethodAdditionalFee">A value indicating whether we should use payment method additional fee when calculating tax</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<TaxTotalResult> GetTaxTotalAsync(IList<ShoppingCartItem> cart, bool usePaymentMethodAdditionalFee = true);

        #endregion
    }
}
