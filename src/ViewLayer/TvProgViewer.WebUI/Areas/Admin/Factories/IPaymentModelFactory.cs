﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.Payments;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the payment method model factory
    /// </summary>
    public partial interface IPaymentModelFactory
    {
        /// <summary>
        /// Prepare payment methods model
        /// </summary>
        /// <param name="methodsModel">Payment methods model</param>        
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment methods model
        /// </returns>
        Task<PaymentMethodsModel> PreparePaymentMethodsModelAsync(PaymentMethodsModel methodsModel);

        /// <summary>
        /// Prepare paged payment method list model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment method list model
        /// </returns>
        Task<PaymentMethodListModel> PreparePaymentMethodListModelAsync(PaymentMethodSearchModel searchModel);
        
        /// <summary>
        /// Prepare payment method search model
        /// </summary>
        /// <param name="searchModel">Payment method search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment method search model
        /// </returns>
        Task<PaymentMethodSearchModel> PreparePaymentMethodSearchModelAsync(PaymentMethodSearchModel searchModel);

        /// <summary>
        /// Prepare payment method restriction model
        /// </summary>
        /// <param name="model">Payment method restriction model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the payment method restriction model
        /// </returns>
        Task<PaymentMethodRestrictionModel> PreparePaymentMethodRestrictionModelAsync(PaymentMethodRestrictionModel model);
    }
}