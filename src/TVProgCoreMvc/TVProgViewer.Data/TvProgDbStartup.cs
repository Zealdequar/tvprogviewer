using System;
using System.Linq;
using FluentMigrator.Runner;
using LinqToDB.Data;
using LinqToDB.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TVProgViewer.Core.Caching;
using TVProgViewer.Core.Infrastructure;
using TVProgViewer.Data.Extensions;
using TVProgViewer.Data.Migrations;

namespace TVProgViewer.Data
{
    /// <summary>
    /// Represents object for the configuring DB context on application startup
    /// </summary>
    public class TvProgDbStartup : ITvProgStartup
    {
        /// <summary>
        /// Add and configure any of the middleware
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var mappingBuilder = new FluentMappingBuilder(TvProgDataConnection.AdditionalSchema);
            
            //find database mapping configuration by other assemblies
            var typeFinder = new AppDomainTypeFinder();
            var typeConfigurations = typeFinder.FindClassesOfType<IMappingConfiguration>().ToList();

            foreach (var typeConfiguration in typeConfigurations)
            {
                var mappingConfiguration = (IMappingConfiguration)Activator.CreateInstance(typeConfiguration);
                mappingConfiguration.ApplyConfiguration(mappingBuilder);
            }

            //further actions are performed only when the database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            DataConnection.DefaultSettings = Singleton<DataSettings>.Instance;

            MappingSchema.Default.SetConvertExpression<string, Guid>(strGuid => new Guid(strGuid));

            services
                // add common FluentMigrator services
                .AddFluentMigratorCore()
                .ConfigureRunner(rb => rb.SetServer()
                    .WithVersionTable(new MigrationVersionInfo())
                    // define the assembly containing the migrations
                    .ScanIn(typeConfigurations.Select(p => p.Assembly).Distinct().ToArray()).For.Migrations());
        }

        /// <summary>
        /// Configure the using of added middleware
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public void Configure(IApplicationBuilder application)
        {
            //further actions are performed only when the database is installed
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            EngineContext.Current.Resolve<ILocker>().PerformActionWithLock(typeof(TvProgDbStartup).FullName, TimeSpan.FromSeconds(300),
                () => EngineContext.Current.Resolve<IDataProvider>().ApplyUpMigrations());
        }

        /// <summary>
        /// Gets order of this startup configuration implementation
        /// </summary>
        public int Order => 10;
    }
}