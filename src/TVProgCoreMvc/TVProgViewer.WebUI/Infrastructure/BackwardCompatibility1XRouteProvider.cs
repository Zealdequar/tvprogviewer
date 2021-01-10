using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TVProgViewer.Core.Domain.Common;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data;
using TVProgViewer.Web.Framework.Mvc.Routing;

namespace TVProgViewer.WebUI.Infrastructure
{
    /// <summary>
    /// Represents provider that provided routes used for backward compatibility with 1.x versions of nopCommerce
    /// </summary>
    public partial class BackwardCompatibility1XRouteProvider : IRouteProvider
    {
        #region Methods

        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            if (DataSettingsManager.IsDatabaseInstalled() && !EngineContext.Current.Resolve<CommonSettings>().SupportPreviousTvProgViewerVersions)
                return;

            //all old aspx URLs
            endpointRouteBuilder.MapControllerRoute("", "{oldfilename}.aspx",
                new { controller = "BackwardCompatibility1X", action = "GeneralRedirect" });

            //products
            endpointRouteBuilder.MapControllerRoute("", "products/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProduct" });

            //categories
            endpointRouteBuilder.MapControllerRoute("", "category/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectCategory" });

            //manufacturers
            endpointRouteBuilder.MapControllerRoute("", "manufacturer/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectManufacturer" });

            //product tags
            endpointRouteBuilder.MapControllerRoute("", "producttag/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectProductTag" });

            //news
            endpointRouteBuilder.MapControllerRoute("", "news/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectNewsItem" });

            //blog posts
            endpointRouteBuilder.MapControllerRoute("", "blog/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectBlogPost" });

            //topics
            endpointRouteBuilder.MapControllerRoute("", "topic/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectTopic" });

            //forums
            endpointRouteBuilder.MapControllerRoute("", "boards/fg/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForumGroup" });

            endpointRouteBuilder.MapControllerRoute("", "boards/f/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForum" });

            endpointRouteBuilder.MapControllerRoute("", "boards/t/{id}.aspx",
                new { controller = "BackwardCompatibility1X", action = "RedirectForumTopic" });
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => -1000; //register it after all other IRouteProvider are processed

        #endregion
    }
}