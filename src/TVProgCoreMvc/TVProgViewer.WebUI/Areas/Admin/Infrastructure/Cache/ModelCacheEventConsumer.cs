using System.Threading.Tasks;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Domain.Blogs;
using TVProgViewer.Core.Domain.Catalog;
using TVProgViewer.Core.Domain.Configuration;
using TVProgViewer.Core.Domain.Localization;
using TVProgViewer.Core.Domain.Media;
using TVProgViewer.Core.Domain.News;
using TVProgViewer.Core.Domain.Orders;
using TVProgViewer.Core.Domain.Polls;
using TVProgViewer.Core.Domain.Topics;
using TVProgViewer.Core.Domain.Vendors;
using TVProgViewer.Core.Events;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.WebUI.Areas.Admin.Infrastructure.Cache
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

        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.OfficialNewsModelKey); //depends on AdminAreaSettings.HideAdvertisementsOnAdminArea
        }

        //categories
        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoriesListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoriesListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoriesListPrefixCacheKey);
        }

        //manufacturers
        public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturersListPrefixCacheKey);
        }

        //vendors
        public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorsListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorsListPrefixCacheKey);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorsListPrefixCacheKey);
        }

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event</param>
        public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgPluginDefaults.AdminNavigationPluginsPrefix);
        }

        #endregion
    }
}