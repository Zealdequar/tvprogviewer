using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Topics;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Http;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Seo;
using TvProgViewer.Web.Framework.Events;

namespace TvProgViewer.Web.Framework.Mvc.Routing
{
    /// <summary>
    /// Represents slug route transformer
    /// </summary>
    public partial class SlugRouteTransformer : DynamicRouteValueTransformer
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly ICategoryService _categoryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILanguageService _languageService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreContext _storeContext;
        private readonly IUrlRecordService _urlRecordService;
        private readonly LocalizationSettings _localizationSettings;

        #endregion

        #region Ctor

        public SlugRouteTransformer(CatalogSettings catalogSettings,
            ICategoryService categoryService,
            IEventPublisher eventPublisher,
            ILanguageService languageService,
            IManufacturerService manufacturerService,
            IStoreContext storeContext,
            IUrlRecordService urlRecordService,
            LocalizationSettings localizationSettings)
        {
            _catalogSettings = catalogSettings;
            _categoryService = categoryService;
            _eventPublisher = eventPublisher;
            _languageService = languageService;
            _manufacturerService = manufacturerService;
            _storeContext = storeContext;
            _urlRecordService = urlRecordService;
            _localizationSettings = localizationSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Transform route values according to the passed URL record
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>Задача представляет асинхронную операцию</returns>
        protected virtual async Task SingleSlugRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            //if URL record is not active let's find the latest one
            var slug = urlRecord.IsActive
                ? urlRecord.Slug
                : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
            if (string.IsNullOrEmpty(slug))
                return;

            if (!urlRecord.IsActive || !string.IsNullOrEmpty(catalogPath))
            {
                //permanent redirect to new URL with active single slug
                InternalRedirect(httpContext, values, $"/{slug}", true);
                return;
            }

            //Ensure that the slug is the same for the current language, 
            //otherwise it can cause some issues when users choose a new language but a slug stays the same
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(TvProgRoutingDefaults.RouteValue.Language, out var langValue))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                var language = languages
                    .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    ?? languages.FirstOrDefault();

                var slugLocalized = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                if (!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase))
                {
                    //we should make validation above because some entities does not have SeName for standard (Id = 0) language (e.g. news, blog posts)

                    //redirect to the page for current language
                    InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{slugLocalized}", false);
                    return;
                }
            }

            //since we are here, all is ok with the slug, so process URL
            switch (urlRecord.EntityName)
            {
                case var name when name.Equals(nameof(TvChannel), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "TvChannel", "TvChannelDetails", slug, (TvProgRoutingDefaults.RouteValue.TvChannelId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(TvChannelTag), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "TvChannelsByTag", slug, (TvProgRoutingDefaults.RouteValue.TvChannelTagId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Category", slug, (TvProgRoutingDefaults.RouteValue.CategoryId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Manufacturer", slug, (TvProgRoutingDefaults.RouteValue.ManufacturerId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Vendor), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Catalog", "Vendor", slug, (TvProgRoutingDefaults.RouteValue.VendorId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(NewsItem), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "News", "NewsItem", slug, (TvProgRoutingDefaults.RouteValue.NewsItemId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(BlogPost), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Blog", "BlogPost", slug, (TvProgRoutingDefaults.RouteValue.BlogPostId, urlRecord.EntityId));
                    return;

                case var name when name.Equals(nameof(Topic), StringComparison.InvariantCultureIgnoreCase):
                    RouteToAction(values, "Topic", "TopicDetails", slug, (TvProgRoutingDefaults.RouteValue.TopicId, urlRecord.EntityId));
                    return;
            }
        }

        /// <summary>
        /// Try transforming the route values, assuming the passed URL record is of a tvChannel type
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="urlRecord">Record found by the URL slug</param>
        /// <param name="catalogPath">URL catalog path</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains a value whether the route values were processed
        /// </returns>
        protected virtual async Task<bool> TryTvChannelCatalogRoutingAsync(HttpContext httpContext, RouteValueDictionary values, UrlRecord urlRecord, string catalogPath)
        {
            //ensure it's a tvChannel URL record
            if (!urlRecord.EntityName.Equals(nameof(TvChannel), StringComparison.InvariantCultureIgnoreCase))
                return false;

            //if the tvChannel URL structure type is tvChannel seName only, it will be processed later by a single slug
            if (_catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.TvChannel)
                return false;

            //get active slug for the tvChannel
            var slug = urlRecord.IsActive
                ? urlRecord.Slug
                : await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, urlRecord.LanguageId);
            if (string.IsNullOrEmpty(slug))
                return false;

            //try to get active catalog (e.g. category or manufacturer) seName for the tvChannel
            var catalogSeName = string.Empty;
            var isCategoryTvChannelUrl = _catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.CategoryTvChannel;
            if (isCategoryTvChannelUrl)
            {
                var tvChannelCategory = (await _categoryService.GetTvChannelCategoriesByTvChannelIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var category = await _categoryService.GetCategoryByIdAsync(tvChannelCategory?.CategoryId ?? 0);
                catalogSeName = category is not null ? await _urlRecordService.GetSeNameAsync(category) : string.Empty;
            }
            var isManufacturerTvChannelUrl = _catalogSettings.TvChannelUrlStructureTypeId == (int)TvChannelUrlStructureType.ManufacturerTvChannel;
            if (isManufacturerTvChannelUrl)
            {
                var tvChannelManufacturer = (await _manufacturerService.GetTvChannelManufacturersByTvChannelIdAsync(urlRecord.EntityId)).FirstOrDefault();
                var manufacturer = await _manufacturerService.GetManufacturerByIdAsync(tvChannelManufacturer?.ManufacturerId ?? 0);
                catalogSeName = manufacturer is not null ? await _urlRecordService.GetSeNameAsync(manufacturer) : string.Empty;
            }
            if (string.IsNullOrEmpty(catalogSeName))
                return false;

            //get URL record by the specified catalog path
            var catalogUrlRecord = await _urlRecordService.GetBySlugAsync(catalogPath);
            if (catalogUrlRecord is null ||
                (isCategoryTvChannelUrl && !catalogUrlRecord.EntityName.Equals(nameof(Category), StringComparison.InvariantCultureIgnoreCase)) ||
                (isManufacturerTvChannelUrl && !catalogUrlRecord.EntityName.Equals(nameof(Manufacturer), StringComparison.InvariantCultureIgnoreCase)) ||
                !urlRecord.IsActive)
            {
                //permanent redirect to new URL with active catalog seName and active slug
                InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
                return true;
            }

            //ensure the catalog seName and slug are the same for the current language
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled && values.TryGetValue(TvProgRoutingDefaults.RouteValue.Language, out var langValue))
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                var languages = await _languageService.GetAllLanguagesAsync(storeId: store.Id);
                var language = languages
                    .FirstOrDefault(lang => lang.Published && lang.UniqueSeoCode.Equals(langValue?.ToString(), StringComparison.InvariantCultureIgnoreCase))
                    ?? languages.FirstOrDefault();

                var slugLocalized = await _urlRecordService.GetActiveSlugAsync(urlRecord.EntityId, urlRecord.EntityName, language.Id);
                var catalogSlugLocalized = await _urlRecordService.GetActiveSlugAsync(catalogUrlRecord.EntityId, catalogUrlRecord.EntityName, language.Id);
                if ((!string.IsNullOrEmpty(slugLocalized) && !slugLocalized.Equals(slug, StringComparison.InvariantCultureIgnoreCase)) ||
                    (!string.IsNullOrEmpty(catalogSlugLocalized) && !catalogSlugLocalized.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase)))
                {
                    //redirect to localized URL for the current language
                    var activeSlug = !string.IsNullOrEmpty(slugLocalized) ? slugLocalized : slug;
                    var activeCatalogSlug = !string.IsNullOrEmpty(catalogSlugLocalized) ? catalogSlugLocalized : catalogUrlRecord.Slug;
                    InternalRedirect(httpContext, values, $"/{language.UniqueSeoCode}/{activeCatalogSlug}/{activeSlug}", false);
                    return true;
                }
            }

            //ensure the specified catalog path is equal to the active catalog seName
            //we do it here after localization check to avoid double redirect
            if (!catalogSeName.Equals(catalogUrlRecord.Slug, StringComparison.InvariantCultureIgnoreCase))
            {
                //permanent redirect to new URL with active catalog seName and active slug
                InternalRedirect(httpContext, values, $"/{catalogSeName}/{slug}", true);
                return true;
            }

            //all is ok, so select the appropriate action
            RouteToAction(values, "TvChannel", "TvChannelDetails", slug,
                (TvProgRoutingDefaults.RouteValue.TvChannelId, urlRecord.EntityId), (TvProgRoutingDefaults.RouteValue.CatalogSeName, catalogSeName));
            return true;
        }

        /// <summary>
        /// Transform route values to redirect the request
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="path">Path</param>
        /// <param name="permanent">Whether the redirect should be permanent</param>
        protected virtual void InternalRedirect(HttpContext httpContext, RouteValueDictionary values, string path, bool permanent)
        {
            values[TvProgRoutingDefaults.RouteValue.Controller] = "Common";
            values[TvProgRoutingDefaults.RouteValue.Action] = "InternalRedirect";
            values[TvProgRoutingDefaults.RouteValue.Url] = $"{httpContext.Request.PathBase}{path}{httpContext.Request.QueryString}";
            values[TvProgRoutingDefaults.RouteValue.PermanentRedirect] = permanent;
            httpContext.Items[TvProgHttpDefaults.GenericRouteInternalRedirect] = true;
        }

        /// <summary>
        /// Transform route values to set controller, action and action parameters
        /// </summary>
        /// <param name="values">The route values associated with the current match</param>
        /// <param name="controller">Controller name</param>
        /// <param name="action">Action name</param>
        /// <param name="slug">URL slug</param>
        /// <param name="parameters">Action parameters</param>
        protected virtual void RouteToAction(RouteValueDictionary values, string controller, string action, string slug, params (string Key, object Value)[] parameters)
        {
            values[TvProgRoutingDefaults.RouteValue.Controller] = controller;
            values[TvProgRoutingDefaults.RouteValue.Action] = action;
            values[TvProgRoutingDefaults.RouteValue.SeName] = slug;
            foreach (var (key, value) in parameters)
            {
                values[key] = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a set of transformed route values that will be used to select an action
        /// </summary>
        /// <param name="httpContext">HTTP context</param>
        /// <param name="routeValues">The route values associated with the current match</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the set of values
        /// </returns>
        public override async ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary routeValues)
        {
            //get values to transform for action selection
            var values = new RouteValueDictionary(routeValues);
            if (values is null)
                return values;

            if (!values.TryGetValue(TvProgRoutingDefaults.RouteValue.SeName, out var slug))
                return values;

            //find record by the URL slug
            if (await _urlRecordService.GetBySlugAsync(slug.ToString()) is not UrlRecord urlRecord)
                return values;

            //allow third-party handlers to select an action by the found URL record
            var routingEvent = new GenericRoutingEvent(httpContext, values, urlRecord);
            await _eventPublisher.PublishAsync(routingEvent);
            if (routingEvent.Handled)
                return values;

            //then try to select an action by the found URL record and the catalog path
            var catalogPath = values.TryGetValue(TvProgRoutingDefaults.RouteValue.CatalogSeName, out var catalogPathValue)
                ? catalogPathValue.ToString()
                : string.Empty;
            if (await TryTvChannelCatalogRoutingAsync(httpContext, values, urlRecord, catalogPath))
                return values;

            //finally, select an action by the URL record only
            await SingleSlugRoutingAsync(httpContext, values, urlRecord, catalogPath);

            return values;
        }

        #endregion
    }
}