﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Mvc.Routing;

namespace TvProgViewer.Plugin.MultiFactorAuth.GoogleAuthenticator.Infrastructure
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
            endpointRouteBuilder.MapControllerRoute(GoogleAuthenticatorDefaults.ConfigurationRouteName, "Plugins/GoogleAuthenticator/Configure",
                new { controller = "GoogleAuthenticator", action = "Configure", area = AreaNames.Admin });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
