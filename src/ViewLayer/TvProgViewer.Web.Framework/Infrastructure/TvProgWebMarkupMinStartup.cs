using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Web.Framework.Infrastructure.Extensions;

namespace TvProgViewer.Web.Framework.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring WebMarkupMin services on application startup
    /// </summary>
    public partial class TvProgWebMarkupMinStartup : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            //add WebMarkupMin services to the services container
            services.AddTvProgWebMarkupMin();
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //use WebMarkupMin
            application.UseTvProgWebMarkupMin();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 300; //Ensure that "UseTvProgWebMarkupMin" method is invoked before "UseRouting". Otherwise, HTML minification won't work
    }
}
