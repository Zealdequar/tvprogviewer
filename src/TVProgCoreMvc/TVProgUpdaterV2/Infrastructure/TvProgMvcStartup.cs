using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.TVProgUpdaterV2.Infrastructure.Extensions;

namespace TVProgViewer.TVProgUpdaterV2.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring MVC on application startup
    /// </summary>
    public class TvProgMvcStartup : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add MiniProfiler services
            services.AddTVProgViewerMiniProfiler();

            //add WebMarkupMin services to the services container
            services.AddTVProgViewerWebMarkupMin();

            //add and configure MVC feature
            services.AddTVProgViewerMvc();

            //add custom redirect result executor
            services.AddTVProgViewerRedirectResultExecutor();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use MiniProfiler
            application.UseMiniProfiler();

            //use WebMarkupMin
            application.UseTVProgViewerWebMarkupMin();

            //Endpoint routing
            application.UseTvProgViewerEndpoints();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 1000; //MVC should be loaded last
    }
}