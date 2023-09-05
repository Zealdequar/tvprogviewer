using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(PayPalViewerDefaults.ConfigurationRouteName,
                "Admin/PayPalViewer/Configure",
                new { controller = "PayPalViewer", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(PayPalViewerDefaults.WebhookRouteName,
                "Plugins/PayPalViewer/Webhook",
                new { controller = "PayPalViewerWebhook", action = "WebhookHandler" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}