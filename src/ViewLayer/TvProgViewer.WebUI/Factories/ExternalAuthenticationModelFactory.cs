﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Core;
using TvProgViewer.Services.Authentication.External;
using TvProgViewer.WebUI.Models.User;

namespace TvProgViewer.WebUI.Factories
{
    /// <summary>
    /// Represents the external authentication model factory
    /// </summary>
    public partial class ExternalAuthenticationModelFactory : IExternalAuthenticationModelFactory
    {
        #region Fields

        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public ExternalAuthenticationModelFactory(IAuthenticationPluginManager authenticationPluginManager,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _authenticationPluginManager = authenticationPluginManager;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the external authentication method model
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of the external authentication method model
        /// </returns>
        public virtual async Task<List<ExternalAuthenticationMethodModel>> PrepareExternalMethodsModelAsync()
        {
            var store = await _storeContext.GetCurrentStoreAsync();

            return (await _authenticationPluginManager
                .LoadActivePluginsAsync(await _workContext.GetCurrentUserAsync(), store.Id))
                .Select(authenticationMethod => new ExternalAuthenticationMethodModel
                {
                    ViewComponent = authenticationMethod.GetPublicViewComponent()
                })
                .ToList();
        }

        #endregion
    }
}