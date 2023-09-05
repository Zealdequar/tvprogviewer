using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Web.Framework.Mvc.Routing;
using TvProgViewer.WebUI.Infrastructure;

namespace TvProgViewer.Plugin.Payments.CyberSource.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : BaseRouteProvider, IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var lang = GetLanguageRoutePattern();

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.ConfigurationRouteName,
                pattern: "Admin/CyberSource/Configure",
                defaults: new { controller = "CyberSource", action = "Configure" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.UserTokensRouteName,
                pattern: $"{lang}/user/cybersource-tokens",
                defaults: new { controller = "CyberSourceUserToken", action = "UserTokens" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.UserTokenAddRouteName,
                pattern: $"{lang}/user/cybersource-token-add",
                defaults: new { controller = "CyberSourceUserToken", action = "TokenAdd" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.UserTokenEditRouteName,
                pattern: $"{lang}/user/cybersource-token-edit/{{tokenId:min(0)}}",
                defaults: new { controller = "CyberSourceUserToken", action = "TokenEdit" });

            endpointRouteBuilder.MapControllerRoute(name: CyberSourceDefaults.PayerRedirectRouteName,
                pattern: "cybersource-payer-redirect",
                defaults: new { controller = "CyberSourceWebhook", action = "PayerRedirect" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}