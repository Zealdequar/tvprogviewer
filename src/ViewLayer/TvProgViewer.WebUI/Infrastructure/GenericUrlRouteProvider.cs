using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Data;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.WebUI.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    public partial class GenericUrlRouteProvider : BaseRouteProvider, IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            //default routes
            //these routes are not generic, they are just default to map requests that don't match other patterns, 
            //but we define them here since this route provider is with the lowest priority, to allow to add additional routes before them
            if (!string.IsNullOrEmpty(lang))
            {
                endpointRouteBuilder.MapControllerRoute(name: "DefaultWithLanguageCode",
                    pattern: $"{lang}/{{controller=Home}}/{{action=Index}}/{{id?}}");
            }

            endpointRouteBuilder.MapControllerRoute(name: "Default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //generic routes (actually routing is processed later in SlugRouteTransformer)
            var genericCatalogPattern = $"{lang}/{{{TvProgRoutingDefaults.RouteValue.CatalogSeName}}}/{{{TvProgRoutingDefaults.RouteValue.SeName}}}";
            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(genericCatalogPattern);

            var genericPattern = $"{lang}/{{{TvProgRoutingDefaults.RouteValue.SeName}}}";
            endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(genericPattern);

            //routes for not found slugs
            if (!string.IsNullOrEmpty(lang))
            {
                endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.GenericUrlWithLanguageCode,
                    pattern: genericPattern,
                    defaults: new { controller = "Common", action = "GenericUrl" });

                endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.GenericCatalogUrlWithLanguageCode,
                    pattern: genericCatalogPattern,
                    defaults: new { controller = "Common", action = "GenericUrl" });
            }

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.GenericUrl,
                pattern: $"{{{TvProgRoutingDefaults.RouteValue.SeName}}}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.GenericCatalogUrl,
                pattern: $"{{{TvProgRoutingDefaults.RouteValue.CatalogSeName}}}/{{{TvProgRoutingDefaults.RouteValue.SeName}}}",
                defaults: new { controller = "Common", action = "GenericUrl" });

            //routes for entities that support catalog path and slug (e.g. '/category-seo-name/tvChannel-seo-name')
            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.TvChannelCatalog,
                pattern: genericCatalogPattern,
                defaults: new { controller = "TvChannel", action = "TvChannelDetails" });

            //routes for entities that support single slug (e.g. '/tvChannel-seo-name')
            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.TvChannel,
                pattern: genericPattern,
                defaults: new { controller = "TvChannel", action = "TvChannelDetails" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.Category,
                pattern: genericPattern,
                defaults: new { controller = "Catalog", action = "Category" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.Manufacturer,
                pattern: genericPattern,
                defaults: new { controller = "Catalog", action = "Manufacturer" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.Vendor,
                pattern: genericPattern,
                defaults: new { controller = "Catalog", action = "Vendor" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.NewsItem,
                pattern: genericPattern,
                defaults: new { controller = "News", action = "NewsItem" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.BlogPost,
                pattern: genericPattern,
                defaults: new { controller = "Blog", action = "BlogPost" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.Topic,
                pattern: genericPattern,
                defaults: new { controller = "Topic", action = "TopicDetails" });

            endpointRouteBuilder.MapControllerRoute(name: TvProgRoutingDefaults.RouteName.Generic.TvChannelTag,
                pattern: genericPattern,
                defaults: new { controller = "Catalog", action = "TvChannelsByTag" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        /// <remarks>
        /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
        /// </remarks>
        public int Priority => -1000000;

        #endregion
    }
}