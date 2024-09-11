using System.Threading.Tasks;
using TvProgViewer.Core.Caching;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Polls;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Cms;
using TvProgViewer.Services.Events;
using TvProgViewer.Services.Plugins;

namespace TvProgViewer.WebUI.Infrastructure.Cache
{
    /// <summary>
    /// Model cache event consumer (used for caching of presentation layer models)
    /// </summary>
    public partial class ModelCacheEventConsumer :
        //languages
        IConsumer<EntityInsertedEvent<Language>>,
        IConsumer<EntityUpdatedEvent<Language>>,
        IConsumer<EntityDeletedEvent<Language>>,
        //settings
        IConsumer<EntityUpdatedEvent<Setting>>,
        //manufacturers
        IConsumer<EntityInsertedEvent<Manufacturer>>,
        IConsumer<EntityUpdatedEvent<Manufacturer>>,
        IConsumer<EntityDeletedEvent<Manufacturer>>,
        //vendors
        IConsumer<EntityInsertedEvent<Vendor>>,
        IConsumer<EntityUpdatedEvent<Vendor>>,
        IConsumer<EntityDeletedEvent<Vendor>>,
        //categories
        IConsumer<EntityInsertedEvent<Category>>,
        IConsumer<EntityUpdatedEvent<Category>>,
        IConsumer<EntityDeletedEvent<Category>>,
        //tvChannel categories
        IConsumer<EntityInsertedEvent<TvChannelCategory>>,
        IConsumer<EntityDeletedEvent<TvChannelCategory>>,
        //tvChannels
        IConsumer<EntityInsertedEvent<TvChannel>>,
        IConsumer<EntityUpdatedEvent<TvChannel>>,
        IConsumer<EntityDeletedEvent<TvChannel>>,
        //tvChannel tags
        IConsumer<EntityInsertedEvent<TvChannelTag>>,
        IConsumer<EntityUpdatedEvent<TvChannelTag>>,
        IConsumer<EntityDeletedEvent<TvChannelTag>>,
        //TvChannel attribute values
        IConsumer<EntityUpdatedEvent<TvChannelAttributeValue>>,
        //Topics
        IConsumer<EntityInsertedEvent<Topic>>,
        IConsumer<EntityUpdatedEvent<Topic>>,
        IConsumer<EntityDeletedEvent<Topic>>,
        //Orders
        IConsumer<EntityInsertedEvent<Order>>,
        IConsumer<EntityUpdatedEvent<Order>>,
        IConsumer<EntityDeletedEvent<Order>>,
        //Picture
        IConsumer<EntityInsertedEvent<Picture>>,
        IConsumer<EntityUpdatedEvent<Picture>>,
        IConsumer<EntityDeletedEvent<Picture>>,
        //TvChannel picture mapping
        IConsumer<EntityInsertedEvent<TvChannelPicture>>,
        IConsumer<EntityUpdatedEvent<TvChannelPicture>>,
        IConsumer<EntityDeletedEvent<TvChannelPicture>>,
        //TvChannel review
        IConsumer<EntityDeletedEvent<TvChannelReview>>,
        //polls
        IConsumer<EntityInsertedEvent<Poll>>,
        IConsumer<EntityUpdatedEvent<Poll>>,
        IConsumer<EntityDeletedEvent<Poll>>,
        //blog posts
        IConsumer<EntityInsertedEvent<BlogPost>>,
        IConsumer<EntityUpdatedEvent<BlogPost>>,
        IConsumer<EntityDeletedEvent<BlogPost>>,
        //news items
        IConsumer<EntityInsertedEvent<NewsItem>>,
        IConsumer<EntityUpdatedEvent<NewsItem>>,
        IConsumer<EntityDeletedEvent<NewsItem>>,
        //shopping cart items
        IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,
        //plugins
        IConsumer<PluginUpdatedEvent>
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IStaticCacheManager _staticCacheManager;

        #endregion

        #region Ctor

        public ModelCacheEventConsumer(CatalogSettings catalogSettings, IStaticCacheManager staticCacheManager)
        {
            _staticCacheManager = staticCacheManager;
            _catalogSettings = catalogSettings;
        }

        #endregion

        #region Methods

        #region Languages

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion

        #region Setting

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryTvChannelNumber and CatalogSettings.ShowCategoryTvChannelNumberIncludingSubcategories
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.TvChannelsAlsoPurchasedNumber
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        #endregion

        #region Vendors

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.VendorPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
        }

        #endregion

        #region  Manufacturers

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Categories

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region TvChannel categories

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryTvChannelNumber)
            {
                //depends on CatalogSettings.ShowCategoryTvChannelNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryTvChannelNumber)
            {
                //depends on CatalogSettings.ShowCategoryTvChannelNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        #endregion

        #region TvChannels

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannel> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannel> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelReviewsPrefixCacheKeyById, eventMessage.Entity.Id));
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannel> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region TvChannel tags

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region TvChannel attributes

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelAttributeValue> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributeImageSquarePicturePrefixCacheKey);
        }

        #endregion

        #region Topics

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Orders

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelsAlsoPurchasedIdsPrefixCacheKey);
        }

        #endregion

        #region Pictures

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelOverviewPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        #endregion

        #region TvChannel picture mappings

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<TvChannelPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<TvChannelPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelOverviewPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.TvChannelAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.OrderPicturePrefixCacheKey);
        }

        #endregion

        #region Polls

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        #endregion

        #region Blog posts

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region News items

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Shopping cart items

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region TvChannel reviews

        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(EntityDeletedEvent<TvChannelReview> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.TvChannelReviewsPrefixCacheKeyById, eventMessage.Entity.TvChannelId));
        }

        #endregion

        #region Plugin

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
        {
            if (eventMessage?.Plugin?.Instance<IWidgetPlugin>() != null)
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.WidgetPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}