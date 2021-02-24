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
using TVProgViewer.Services.Cms;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Plugins;

namespace TVProgViewer.WebUI.Infrastructure.Cache
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
        //product categories
        IConsumer<EntityInsertedEvent<ProductCategory>>,
        IConsumer<EntityDeletedEvent<ProductCategory>>,
        //products
        IConsumer<EntityInsertedEvent<Product>>,
        IConsumer<EntityUpdatedEvent<Product>>,
        IConsumer<EntityDeletedEvent<Product>>,
        //product tags
        IConsumer<EntityInsertedEvent<ProductTag>>,
        IConsumer<EntityUpdatedEvent<ProductTag>>,
        IConsumer<EntityDeletedEvent<ProductTag>>,
        //specification attributes
        IConsumer<EntityUpdatedEvent<SpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<SpecificationAttribute>>,
        //specification attribute options
        IConsumer<EntityUpdatedEvent<SpecificationAttributeOption>>,
        IConsumer<EntityDeletedEvent<SpecificationAttributeOption>>,
        //Product specification attribute
        IConsumer<EntityInsertedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityUpdatedEvent<ProductSpecificationAttribute>>,
        IConsumer<EntityDeletedEvent<ProductSpecificationAttribute>>,
        //Product attribute values
        IConsumer<EntityUpdatedEvent<ProductAttributeValue>>,
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
        //Product picture mapping
        IConsumer<EntityInsertedEvent<ProductPicture>>,
        IConsumer<EntityUpdatedEvent<ProductPicture>>,
        IConsumer<EntityDeletedEvent<ProductPicture>>,
        //Product review
        IConsumer<EntityDeletedEvent<ProductReview>>,
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

        public async Task HandleEventAsync(EntityInsertedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Language> eventMessage)
        {
            //clear all localizable models
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
        }

        #endregion

        #region Setting

        public async Task HandleEventAsync(EntityUpdatedEvent<Setting> eventMessage)
        {
            //clear models which depend on settings
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey); //depends on CatalogSettings.ManufacturersBlockItemsToDisplay
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey); //depends on VendorSettings.VendorBlockItemsToDisplay
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey); //depends on CatalogSettings.ShowCategoryProductNumber and CatalogSettings.ShowCategoryProductNumberIncludingSubcategories
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey); //depends on CatalogSettings.NumberOfBestsellersOnHomepage
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey); //depends on CatalogSettings.ProductsAlsoPurchasedNumber
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey); //depends on BlogSettings.NumberOfTags
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey); //depends on NewsSettings.MainPageNewsCount
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey); //depends on distinct sitemap settings
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.WidgetPrefixCacheKey); //depends on WidgetSettings and certain settings of widgets
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.StoreLogoPathPrefixCacheKey); //depends on StoreInformationSettings.LogoPictureId
        }

        #endregion

        #region Vendors

        public async Task HandleEventAsync(EntityInsertedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.VendorPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Vendor> eventMessage)
        {
            await _staticCacheManager.RemoveAsync(TvProgModelCacheDefaults.VendorNavigationModelKey);
        }

        #endregion

        #region  Manufacturers

        public async Task HandleEventAsync(EntityInsertedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Manufacturer> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerNavigationPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Categories

        public async Task HandleEventAsync(EntityInsertedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Category> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryHomepagePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product categories

        public async Task HandleEventAsync(EntityInsertedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        public async Task HandleEventAsync(EntityDeletedEvent<ProductCategory> eventMessage)
        {
            if (_catalogSettings.ShowCategoryProductNumber)
            {
                //depends on CatalogSettings.ShowCategoryProductNumber (when enabled)
                //so there's no need to clear this cache in other cases
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryAllPrefixCacheKey);
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryXmlAllPrefixCacheKey);
            }
        }

        #endregion

        #region Products

        public async Task HandleEventAsync(EntityInsertedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.Id));
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Product> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Product tags

        public async Task HandleEventAsync(EntityInsertedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<ProductTag> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Specification attributes

        public async Task HandleEventAsync(EntityUpdatedEvent<SpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<SpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Specification attribute options

        public async Task HandleEventAsync(EntityUpdatedEvent<SpecificationAttributeOption> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<SpecificationAttributeOption> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Product specification attribute

        public async Task HandleEventAsync(EntityInsertedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<ProductSpecificationAttribute> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SpecsFilterPrefixCacheKey);
        }

        #endregion

        #region Product attributes

        public async Task HandleEventAsync(EntityUpdatedEvent<ProductAttributeValue> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributeImageSquarePicturePrefixCacheKey);
        }

        #endregion

        #region Topics

        public async Task HandleEventAsync(EntityInsertedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Topic> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Orders

        public async Task HandleEventAsync(EntityInsertedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Order> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.HomepageBestsellersIdsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductsAlsoPurchasedIdsPrefixCacheKey);
        }

        #endregion

        #region Pictures

        public async Task HandleEventAsync(EntityInsertedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Picture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductDetailsPicturesPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductDefaultPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CategoryPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ManufacturerPicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.VendorPicturePrefixCacheKey);
        }

        #endregion

        #region Product picture mappings

        public async Task HandleEventAsync(EntityInsertedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<ProductPicture> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDefaultPicturePrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductDetailsPicturesPrefixCacheKeyById, eventMessage.Entity.ProductId));
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.ProductAttributePicturePrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region Polls

        public async Task HandleEventAsync(EntityInsertedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<Poll> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.PollsPrefixCacheKey);
        }

        #endregion

        #region Blog posts

        public async Task HandleEventAsync(EntityInsertedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<BlogPost> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.BlogPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region News items

        public async Task HandleEventAsync(EntityInsertedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityUpdatedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        public async Task HandleEventAsync(EntityDeletedEvent<NewsItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.NewsPrefixCacheKey);
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.SitemapPrefixCacheKey);
        }

        #endregion

        #region Shopping cart items

        public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.CartPicturePrefixCacheKey);
        }

        #endregion

        #region Product reviews

        public async Task HandleEventAsync(EntityDeletedEvent<ProductReview> eventMessage)
        {
            await _staticCacheManager.RemoveByPrefixAsync(string.Format(TvProgModelCacheDefaults.ProductReviewsPrefixCacheKeyById, eventMessage.Entity.ProductId));
        }

        #endregion

        #region Plugin

        /// <summary>
        /// Handle plugin updated event
        /// </summary>
        /// <param name="eventMessage">Event message</param>
        public async Task HandleEventAsync(PluginUpdatedEvent eventMessage)
        {
            if (eventMessage?.Plugin?.Instance<IWidgetPlugin>() != null)
                await _staticCacheManager.RemoveByPrefixAsync(TvProgModelCacheDefaults.WidgetPrefixCacheKey);
        }

        #endregion

        #endregion
    }
}