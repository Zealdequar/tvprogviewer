﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.Services.Payments
{
    /// <summary>
    /// Represents a payment plugin manager
    /// </summary>
    public partial interface IPaymentPluginManager : IPluginManager<IPaymentMethod>
    {
        /// <summary>
        /// Load active payment methods
        /// </summary>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <param name="countryId">Filter by country; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of active payment methods
        /// </returns>
        Task<IList<IPaymentMethod>> LoadActivePluginsAsync(User user = null, int storeId = 0, int countryId = 0);

        /// <summary>
        /// Check whether the passed payment method is active
        /// </summary>
        /// <param name="paymentMethod">Payment method to check</param>
        /// <returns>Result</returns>
        bool IsPluginActive(IPaymentMethod paymentMethod);

        /// <summary>
        /// Check whether the payment method with the passed system name is active
        /// </summary>
        /// <param name="systemName">System name of payment method to check</param>
        /// <param name="user">Filter by user; pass null to load all plugins</param>
        /// <param name="storeId">Filter by store; pass 0 to load all plugins</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the result
        /// </returns>
        Task<bool> IsPluginActiveAsync(string systemName, User user = null, int storeId = 0);

        /// <summary>
        /// Get countries in which the passed payment method is not allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of country identifiers
        /// </returns>
        Task<IList<int>> GetRestrictedCountryIdsAsync(IPaymentMethod paymentMethod);

        /// <summary>
        /// Save countries in which the passed payment method is not allowed
        /// </summary>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="countryIds">List of country identifiers</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        Task SaveRestrictedCountriesAsync(IPaymentMethod paymentMethod, IList<int> countryIds);
    }
}