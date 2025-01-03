﻿using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.TvProgUpdaterV3.Infrastructure.Extensions;

namespace TvProgViewer.TvProgUpdaterV3.Infrastructure
{
    /// <summary>
    /// Represents object for the configuring endpoints on application startup
    /// </summary>
    public partial class TvProgEndpoints : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //Endpoints routing
            application.UseTvProgEndpoints();
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 900; //authentication should be loaded before MVC
    }
}
