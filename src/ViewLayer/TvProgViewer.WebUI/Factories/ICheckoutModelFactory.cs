﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Services.Payments;
using TvProgViewer.WebUI.Models.Checkout;

namespace TvProgViewer.WebUI.Factories
{
    public partial interface ICheckoutModelFactory
    {
        /// <summary>
        /// Prepare billing address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithUserFields">Pre populate new address with user fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the billing address model
        /// </returns>
        Task<CheckoutBillingAddressModel> PrepareBillingAddressModelAsync(IList<ShoppingCartItem> cart,
            int? selectedCountryId = null,
            bool prePopulateNewAddressWithUserFields = false,
            string overrideAttributesXml = "");

        /// <summary>
        /// Prepare shipping address model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="selectedCountryId">Selected country identifier</param>
        /// <param name="prePopulateNewAddressWithUserFields">Pre populate new address with user fields</param>
        /// <param name="overrideAttributesXml">Override attributes xml</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping address model
        /// </returns>
        Task<CheckoutShippingAddressModel> PrepareShippingAddressModelAsync(IList<ShoppingCartItem> cart, int? selectedCountryId = null,
            bool prePopulateNewAddressWithUserFields = false, string overrideAttributesXml = "");

        /// <summary>
        /// Prepare shipping method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the shipping method model
        /// </returns>
        Task<CheckoutShippingMethodModel> PrepareShippingMethodModelAsync(IList<ShoppingCartItem> cart, Address shippingAddress);

        /// <summary>
        /// Prepare payment method model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <param name="filterByCountryId">Filter by country identifier</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment method model
        /// </returns>
        Task<CheckoutPaymentMethodModel> PreparePaymentMethodModelAsync(IList<ShoppingCartItem> cart, int filterByCountryId);

        /// <summary>
        /// Prepare payment info model
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment info model
        /// </returns>
        Task<CheckoutPaymentInfoModel> PreparePaymentInfoModelAsync(IPaymentMethod paymentMethod);

        /// <summary>
        /// Prepare confirm order model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the confirm order model
        /// </returns>
        Task<CheckoutConfirmModel> PrepareConfirmOrderModelAsync(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Prepare checkout completed model
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout completed model
        /// </returns>
        Task<CheckoutCompletedModel> PrepareCheckoutCompletedModelAsync(Order order);

        /// <summary>
        /// Prepare checkout progress model
        /// </summary>
        /// <param name="step">Step</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the checkout progress model
        /// </returns>
        Task<CheckoutProgressModel> PrepareCheckoutProgressModelAsync(CheckoutProgressStep step);

        /// <summary>
        /// Prepare one page checkout model
        /// </summary>
        /// <param name="cart">Cart</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the one page checkout model
        /// </returns>
        Task<OnePageCheckoutModel> PrepareOnePageCheckoutModelAsync(IList<ShoppingCartItem> cart);
    }
}
