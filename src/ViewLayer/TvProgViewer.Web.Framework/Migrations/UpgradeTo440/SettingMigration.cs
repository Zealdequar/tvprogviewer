using FluentMigrator;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Seo;

namespace TvProgViewer.Web.Framework.Migrations.UpgradeTo440
{
    [TvProgMigration("2020-06-10 00:00:00", "4.40.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
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

            //#4904 External authentication errors logging
            var externalAuthenticationSettings = settingService.LoadSetting<ExternalAuthenticationSettings>();
            if (!settingService.SettingExists(externalAuthenticationSettings, settings => settings.LogErrors))
            {
                externalAuthenticationSettings.LogErrors = false;
                settingService.SaveSetting(externalAuthenticationSettings, settings => settings.LogErrors);
            }

            var multiFactorAuthenticationSettings = settingService.LoadSetting<MultiFactorAuthenticationSettings>();
            if (!settingService.SettingExists(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication))
            {
                multiFactorAuthenticationSettings.ForceMultifactorAuthentication = false;

                settingService.SaveSetting(multiFactorAuthenticationSettings, settings => settings.ForceMultifactorAuthentication);
            }

            //#5102 Delete Full-text settings
            settingRepository
                .Delete(setting => setting.Name == "commonsettings.usefulltextsearch" || setting.Name == "commonsettings.fulltextmode");

            //#4196
            settingRepository
                .Delete(setting => setting.Name == "commonsettings.scheduletaskruntimeout" ||
                    setting.Name == "commonsettings.staticfilescachecontrol" ||
                    setting.Name == "commonsettings.supportprevioustvprogviewerversions" ||
                    setting.Name == "securitysettings.pluginstaticfileextensionsBlacklist");

            //#5384
            var seoSettings = settingService.LoadSetting<SeoSettings>();
            foreach (var slug in TvProgSeoDefaults.ReservedUrlRecordSlugs)
            {
                if (!seoSettings.ReservedUrlRecordSlugs.Contains(slug))
                    seoSettings.ReservedUrlRecordSlugs.Add(slug);
            }
            settingService.SaveSetting(seoSettings, settings => seoSettings.ReservedUrlRecordSlugs);
            
            //#3015
            var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
            if (settingService.GetSettingByKey<string>(homepageTitleKey) == null) 
                settingService.SetSetting(homepageTitleKey, settingService.GetSettingByKey<string>($"{nameof(SeoSettings)}.DefaultTitle"));

            var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();
            if (settingService.GetSettingByKey<string>(homepageDescriptionKey) == null) 
                settingService.SetSetting(homepageDescriptionKey, "Your home page description");

            //#5210
            var adminAreaSettings = settingService.LoadSetting<AdminAreaSettings>();
            if (!settingService.SettingExists(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks))
            {
                adminAreaSettings.ShowDocumentationReferenceLinks = true;
                settingService.SaveSetting(adminAreaSettings, settings => settings.ShowDocumentationReferenceLinks);
            }

            //#4944
            var shippingSettings = settingService.LoadSetting<ShippingSettings>();
            if (!settingService.SettingExists(shippingSettings, settings => settings.RequestDelay))
            {
                shippingSettings.RequestDelay = 300;
                settingService.SaveSetting(shippingSettings, settings => settings.RequestDelay);
            }

            //#276 AJAX filters
            var catalogSettings = settingService.LoadSetting<CatalogSettings>();
            if (!settingService.SettingExists(catalogSettings, settings => settings.UseAjaxCatalogTvChannelsLoading))
            {
                catalogSettings.UseAjaxCatalogTvChannelsLoading = true;
                settingService.SaveSetting(catalogSettings, settings => settings.UseAjaxCatalogTvChannelsLoading);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.EnableManufacturerFiltering))
            {
                catalogSettings.EnableManufacturerFiltering = true;
                settingService.SaveSetting(catalogSettings, settings => settings.EnableManufacturerFiltering);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.EnablePriceRangeFiltering))
            {
                catalogSettings.EnablePriceRangeFiltering = true;
                settingService.SaveSetting(catalogSettings, settings => settings.EnablePriceRangeFiltering);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceRangeFiltering))
            {
                catalogSettings.SearchPagePriceRangeFiltering = true;
                settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceRangeFiltering);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceFrom))
            {
                catalogSettings.SearchPagePriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom;
                settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceFrom);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPagePriceTo))
            {
                catalogSettings.SearchPagePriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo;
                settingService.SaveSetting(catalogSettings, settings => settings.SearchPagePriceTo);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.SearchPageManuallyPriceRange))
            {
                catalogSettings.SearchPageManuallyPriceRange = false;
                settingService.SaveSetting(catalogSettings, settings => settings.SearchPageManuallyPriceRange);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.TvChannelsByTagPriceRangeFiltering))
            {
                catalogSettings.TvChannelsByTagPriceRangeFiltering = true;
                settingService.SaveSetting(catalogSettings, settings => settings.TvChannelsByTagPriceRangeFiltering);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.TvChannelsByTagPriceFrom))
            {
                catalogSettings.TvChannelsByTagPriceFrom = TvProgCatalogDefaults.DefaultPriceRangeFrom;
                settingService.SaveSetting(catalogSettings, settings => settings.TvChannelsByTagPriceFrom);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.TvChannelsByTagPriceTo))
            {
                catalogSettings.TvChannelsByTagPriceTo = TvProgCatalogDefaults.DefaultPriceRangeTo;
                settingService.SaveSetting(catalogSettings, settings => settings.TvChannelsByTagPriceTo);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.TvChannelsByTagManuallyPriceRange))
            {
                catalogSettings.TvChannelsByTagManuallyPriceRange = false;
                settingService.SaveSetting(catalogSettings, settings => settings.TvChannelsByTagManuallyPriceRange);
            }

            //#4303
            var orderSettings = settingService.LoadSetting<OrderSettings>();
            if (!settingService.SettingExists(orderSettings, settings => settings.DisplayUserCurrencyOnOrders))
            {
                orderSettings.DisplayUserCurrencyOnOrders = false;
                settingService.SaveSetting(orderSettings, settings => settings.DisplayUserCurrencyOnOrders);
            }

            //#16 #2909
            if (!settingService.SettingExists(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType))
            {
                catalogSettings.AttributeValueOutOfStockDisplayType = AttributeValueOutOfStockDisplayType.AlwaysDisplay;
                settingService.SaveSetting(catalogSettings, settings => settings.AttributeValueOutOfStockDisplayType);
            }

            //#5482
            settingService.SetSetting("avalarataxsettings.gettaxratebyaddressonly", true);
            settingService.SetSetting("avalarataxsettings.taxratebyaddresscachetime", 480);

            //#5349
            if (!settingService.SettingExists(shippingSettings, settings => settings.EstimateShippingCityNameEnabled))
            {
                shippingSettings.EstimateShippingCityNameEnabled = false;
                settingService.SaveSetting(shippingSettings, settings => settings.EstimateShippingCityNameEnabled);
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}