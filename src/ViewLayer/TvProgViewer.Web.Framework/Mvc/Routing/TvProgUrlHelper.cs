﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Seo;
using TvProgViewer.Services.Topics;

namespace TvProgViewer.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents the helper implementation to build specific URLs within an application 
    /// </summary>
    public partial class TvProgUrlHelper : ITvProgUrlHelper
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreContext _storeContext;
        private readonly ITopicService _topicService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IUrlRecordService _urlRecordService;

        #endregion

        #region Ctor

        public TvProgUrlHelper(CatalogSettings catalogSettings,
            IActionContextAccessor actionContextAccessor,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IStoreContext storeContext,
            ITopicService topicService,
            IUrlHelperFactory urlHelperFactory,
            IUrlRecordService urlRecordService)
        {
            _catalogSettings = catalogSettings;
            _actionContextAccessor = actionContextAccessor;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _storeContext = storeContext;
            _topicService = topicService;
            _urlHelperFactory = urlHelperFactory;
            _urlRecordService = urlRecordService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Generate a URL for a tvChannel with the specified route values
        /// </summary>
        /// <param name="urlHelper">URL helper</param>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the generated URL
        /// </returns>
        protected virtual async Task<string> RouteTvChannelUrlAsync(IUrlHelper urlHelper,
            object values = null, string protocol = null, string host = null, string fragment = null)
        {
            if (_catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.TvChannel)
                return urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannel, values, protocol, host, fragment);

            var routeValues = new RouteValueDictionary(values);
            if (!routeValues.TryGetValue(TvProgRoutingDefaults.RouteValue.SeName, out var slug))
                return urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannel, values, protocol, host, fragment);

            var urlRecord = await _urlRecordService.GetBySlugAsync(slug.ToString());
            if (urlRecord is null || !urlRecord.EntityName.Equals(nameof(TvChannel), StringComparison.InvariantCultureIgnoreCase))
                return urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannel, values, protocol, host, fragment);

            var catalogSeName = string.Empty;
            if (_catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.CategoryTvChannel)
            {
                var tvChannelCategory = (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(tvChannelCategory?.CategoryId ?? 0);
                catalogSeName = category is not null ? await _urlRecordService.GetSeNameAsync(category) : string.Empty;
            }
            if (_catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.ManufacturerTvChannel)
            {
                var tvChannelManufacturer = (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(tvChannelManufacturer?.ManufacturerId ?? 0);
                catalogSeName = manufacturer is not null ? await _urlRecordService.GetSeNameAsync(manufacturer) : string.Empty;
            }
            if (string.IsNullOrEmpty(catalogSeName))
                return urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannel, values, protocol, host, fragment);

            routeValues[TvProgRoutingDefaults.RouteValue.CatalogSeName] = catalogSeName;
            return urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannelCatalog, routeValues, protocol, host, fragment);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate a generic URL for the specified entity type and route values
        /// </summary>
        /// <typeparam name="TEntity">Entity type that supports slug</typeparam>
        /// <param name="values">An object that contains route values</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the generated URL
        /// </returns>
        public virtual async Task<string> RouteGenericUrlAsync<TEntity>(object values = null, string protocol = null, string host = null, string fragment = null)
            where TEntity : BaseEntity, ISlugSupported
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
            return typeof(TEntity) switch
            {
                var entityType when entityType == typeof(TvChannel)
                    => await RouteTvChannelUrlAsync(urlHelper, values, protocol, host, fragment),
                var entityType when entityType == typeof(Category)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.Category, values, protocol, host, fragment),
                var entityType when entityType == typeof(Manufacturer)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.Manufacturer, values, protocol, host, fragment),
                var entityType when entityType == typeof(Vendor)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.Vendor, values, protocol, host, fragment),
                var entityType when entityType == typeof(NewsItem)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.NewsItem, values, protocol, host, fragment),
                var entityType when entityType == typeof(BlogPost)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.BlogPost, values, protocol, host, fragment),
                var entityType when entityType == typeof(Topic)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.Topic, values, protocol, host, fragment),
                var entityType when entityType == typeof(TvChannelTag)
                    => urlHelper.RouteUrl(TvProgRoutingDefaults.RouteName.Generic.TvChannelTag, values, protocol, host, fragment),
                var entityType => urlHelper.RouteUrl(entityType.Name, values, protocol, host, fragment)
            };
        }

        /// <summary>
        /// Generate a URL for topic by the specified system name
        /// </summary>
        /// <param name="systemName">Topic system name</param>
        /// <param name="protocol">The protocol for the URL, such as "http" or "https"</param>
        /// <param name="host">The host name for the URL</param>
        /// <param name="fragment">The fragment for the URL</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the generated URL
        /// </returns>
        public virtual async Task<string> RouteTopicUrlAsync(string systemName, string protocol = null, string host = null, string fragment = null)
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var topic = await _topicService.GetTopicBySystemNameAsync(systemName, store.Id);
            if (topic is null)
                return string.Empty;

            var seName = await _urlRecordService.GetSeNameAsync(topic);
            return await RouteGenericUrlAsync<Topic>(new { SeName = seName }, protocol, host, fragment);
        }

        #endregion
    }
}