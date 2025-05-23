﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Stores;
using TvProgViewer.Web.Framework.Models;

namespace TvProgViewer.Web.Framework.Factories
{
    /// <summary>
    /// Represents the base store mapping supported model factory implementation
    /// </summary>
    public partial class StoreMappingSupportedModelFactory : IStoreMappingSupportedModelFactory
    {
        #region Fields
        
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;

        #endregion

        #region Ctor

        public StoreMappingSupportedModelFactory(IStoreMappingService storeMappingService,
            IStoreService storeService)
        {
            _storeMappingService = storeMappingService;
            _storeService = storeService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare selected and all available stores for the passed model
        /// </summary>
        /// <typeparam name="TModel">Store mapping supported model type</typeparam>
        /// <param name="model">Model</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareModelStoresAsync<TModel>(TModel model) where TModel : IStoreMappingSupportedModel
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare available stores
            var availableStores = await _storeService.GetAllStoresAsync();
            model.AvailableStores = availableStores.Select(store => new SelectListItem
            {
                Text = store.Name,
                Value = store.Id.ToString(),
                Selected = model.SelectedStoreIds.Contains(store.Id)
            }).ToList();
        }

        /// <summary>
        /// Prepare selected and all available stores for the passed model by store mappings
        /// </summary>
        /// <typeparam name="TModel">Store mapping supported model type</typeparam>
        /// <typeparam name="TEntity">Store mapping supported entity type</typeparam>
        /// <param name="model">Model</param>
        /// <param name="entity">Entity</param>
        /// <param name="ignoreStoreMappings">Whether to ignore existing store mappings</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public virtual async Task PrepareModelStoresAsync<TModel, TEntity>(TModel model, TEntity entity, bool ignoreStoreMappings)
            where TModel : IStoreMappingSupportedModel where TEntity : BaseEntity, IStoreMappingSupported
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            //prepare stores with granted access
            if (!ignoreStoreMappings && entity != null)
                model.SelectedStoreIds = (await _storeMappingService.GetStoresIdsWithAccessAsync(entity)).ToList();

            await PrepareModelStoresAsync(model);
        }
        
        #endregion
    }
}