﻿using FluentMigrator;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Core.Domain.Shipping;

namespace TvProgViewer.Web.Framework.Migrations.UpgradeTo450
{
    [TvProgMigration("2021-04-23 00:00:00", "4.50.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            //miniprofiler settings are moved to appSettings
            settingRepository
                .Delete(setting => setting.Name == "storeinformationsettings.displayminiprofilerforadminonly" ||
                    setting.Name == "storeinformationsettings.displayminiprofilerinpublicstore");

            //#4363
            var commonSettings = settingService.LoadSetting<CommonSettings>();

            if (!settingService.SettingExists(commonSettings, settings => settings.ClearLogOlderThanDays))
            {
                commonSettings.ClearLogOlderThanDays = 0;
                settingService.SaveSetting(commonSettings, settings => settings.ClearLogOlderThanDays);
            }

            //#5551
            var catalogSettings = settingService.LoadSetting<CatalogSettings>();

            if (!settingService.SettingExists(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering))
            {
                catalogSettings.EnableSpecificationAttributeFiltering = true;
                settingService.SaveSetting(catalogSettings, settings => settings.EnableSpecificationAttributeFiltering);
            }

            //#5204
            var shippingSettings = settingService.LoadSetting<ShippingSettings>();

            if (!settingService.SettingExists(shippingSettings, settings => settings.ShippingSorting))
            {
                shippingSettings.ShippingSorting = ShippingSortingEnum.Position;
                settingService.SaveSetting(shippingSettings, settings => settings.ShippingSorting);
            }

            //#5698
            var orderSettings = settingService.LoadSetting<OrderSettings>();
            if (!settingService.SettingExists(orderSettings, settings => settings.DisplayOrderSummary))
            {
                orderSettings.DisplayOrderSummary = true;
                settingService.SaveSetting(orderSettings, settings => settings.DisplayOrderSummary);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}