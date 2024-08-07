﻿using System.Collections.Generic;
using System.Threading.Tasks;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the interface of the external authentication model factory
    /// </summary>
    public partial interface IExternalAuthenticationModelFactory
    {
        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of the external authentication method model
        /// </returns>
        Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync();
    }
}
