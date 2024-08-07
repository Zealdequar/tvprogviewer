﻿using System.Threading.Tasks;
using TvProgViewer.WebUI.Areas.Admin.Models.ExternalAuthentication;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the external authentication method model factory
    /// </summary>
    public partial interface IExternalAuthenticationMethodModelFactory
    {
        /// <summary>
        /// Prepare external authentication method search model
        /// </summary>
        /// <param name="searchModel">External authentication method search model</param>
        /// <returns>External authentication method search model</returns>
        ExternalAuthenticationMethodSearchModel PrepareExternalAuthenticationMethodSearchModel(
            ExternalAuthenticationMethodSearchModel searchModel);

        /// <summary>
        /// Prepare paged external authentication method list model
        /// </summary>
        /// <param name="searchModel">External authentication method search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the external authentication method list model
        /// </returns>
        Task<ExternalAuthenticationMethodListModel> PrepareExternalAuthenticationMethodListModelAsync(
            ExternalAuthenticationMethodSearchModel searchModel);
    }
}