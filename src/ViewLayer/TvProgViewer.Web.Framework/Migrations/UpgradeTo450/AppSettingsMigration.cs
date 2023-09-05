using System.Collections.Generic;
using FluentMigrator;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Web.Framework.WebOptimizer;

namespace TvProgViewer.Web.Framework.Migrations.UpgradeTo450
{

    [TvProgMigration("2021-10-07 00:00:00", "Pseudo-migration to update appSettings.json file", MigrationProcessType.Update)]
    public class AppSettingsMigration : MigrationBase
    {
        public override void Up()
        {
            var fileProvider = EngineContext.Current.Resolve<ITvProgFileProvider>();

            var rootDir = fileProvider.MapPath("~/");

            var config = new WebOptimizerConfig {
                EnableTagHelperBundling = false,
                EnableCaching = true,
                EnableDiskCache = true,
                AllowEmptyBundle = true,
                CacheDirectory = fileProvider.Combine(rootDir, @"wwwroot\bundles")
            };

            AppSettingsHelper.SaveAppSettings(new List<IConfig> { config }, fileProvider);
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}