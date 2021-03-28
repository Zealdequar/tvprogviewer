using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.TVProgUpdaterV2.Infrastructure.Extensions;
using TVProgViewer.TVProgUpdaterV2.Mvc.Routing;

namespace TVProgViewer.TVProgUpdaterV2.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring common features and middleware on application startup
    /// </summary>
    public class TvProgCommonStartup : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //compression
            services.AddResponseCompression();

            //add options feature
            services.AddOptions();

            //add distributed memory cache
            services.AddDistributedMemoryCache();

            //add HTTP session state feature
            services.AddHttpSession();

            //add default HTTP clients
            services.AddTvProgHttpClients();

            //add anti-forgery
            services.AddAntiForgery();

            //add localization
            services.AddLocalization();

            //add theme support
            services.AddThemes();

            services.AddLogging();

            //add routing
            services.AddRouting(options =>
            {
                // Добавить ключ для языка
                options.ConstraintMap["lang"] = typeof(LanguageParameterTransformer);
            });
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use response compression
            application.UseTVProgViewerResponseCompression();

            //use static files feature
            //application.UseTVProgViewerStaticFiles();

            //check whether requested page is keep alive page
            application.UseKeepAlive();

            //check whether database is installed
            application.UseInstallUrl();

            //use HTTP session
            application.UseSession();

            //use request localization
            application.UseTVProgViewerRequestLocalization();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 100; //common services should be loaded after error handlers
    }
}