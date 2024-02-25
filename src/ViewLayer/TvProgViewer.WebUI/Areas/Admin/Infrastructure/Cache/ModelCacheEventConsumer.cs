using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.WebUI.Areas.Admin.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>,

        IConsumer<PluginUpdatedEvent>
    {
        #region Fields

        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
        }

        #endregion

        #region Methods

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.OfficialNewsModelKey); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }

        //categories
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.CategoriesListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.CategoriesListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.CategoriesListKey);
        }

        //manufacturers
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.ManufacturersListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.ManufacturersListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.ManufacturersListKey);
        }

        //vendors
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorsListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorsListKey);
        }
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorsListKey);
        }

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgPluginDefaults.AdminNavigationPluginsPrefix);
        }

        #endregion
    }
}