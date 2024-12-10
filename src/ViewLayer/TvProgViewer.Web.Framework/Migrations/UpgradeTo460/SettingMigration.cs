using FluentMigrator;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Configuration;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.Messages;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Stores;

namespace TvProgViewer.Web.Framework.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-12-01 14:00:03", "4.60.0", UpdateMigrationType.Settings, MigrationProcessType.Update)]
    public class SettingMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var settingService = EngineContext.Current.Resolve<ISettingService>();

            var catalogSettings = settingService.LoadSetting<CatalogSettings>();

            //#3075
            if (!settingService.SettingExists(catalogSettings, settings => settings.AllowUsersToSearchWithManufacturerName))
            {
                catalogSettings.AllowUsersToSearchWithManufacturerName = true;
                settingService.SaveSetting(catalogSettings, settings => settings.AllowUsersToSearchWithManufacturerName);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.AllowUsersToSearchWithCategoryName))
            {
                catalogSettings.AllowUsersToSearchWithCategoryName = true;
                settingService.SaveSetting(catalogSettings, settings => settings.AllowUsersToSearchWithCategoryName);
            }

            //#1933
            if (!settingService.SettingExists(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages))
            {
                catalogSettings.DisplayAllPicturesOnCatalogPages = false;
                settingService.SaveSetting(catalogSettings, settings => settings.DisplayAllPicturesOnCatalogPages);
            }

            //#3511
            var newTvChannelsNumber = settingService.GetSetting("catalogsettings.newtvChannelsnumber");
            if (newTvChannelsNumber is not null && int.TryParse(newTvChannelsNumber.Value, out var newTvChannelsPageSize))
            {
                catalogSettings.NewTvChannelsPageSize = newTvChannelsPageSize;
                settingService.SaveSetting(catalogSettings, settings => settings.NewTvChannelsPageSize);
                settingService.DeleteSetting(newTvChannelsNumber);
            }
            else if (!settingService.SettingExists(catalogSettings, settings => settings.NewTvChannelsPageSize))
            {
                catalogSettings.NewTvChannelsPageSize = 6;
                settingService.SaveSetting(catalogSettings, settings => settings.NewTvChannelsPageSize);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.NewTvChannelsAllowUsersToSelectPageSize))
            {
                catalogSettings.NewTvChannelsAllowUsersToSelectPageSize = false;
                settingService.SaveSetting(catalogSettings, settings => settings.NewTvChannelsAllowUsersToSelectPageSize);
            }

            if (!settingService.SettingExists(catalogSettings, settings => settings.NewTvChannelsPageSizeOptions))
            {
                catalogSettings.NewTvChannelsPageSizeOptions = "6, 3, 9";
                settingService.SaveSetting(catalogSettings, settings => settings.NewTvChannelsPageSizeOptions);
            }

            //#29
            if (!settingService.SettingExists(catalogSettings, settings => settings.DisplayFromPrices))
            {
                catalogSettings.DisplayFromPrices = false;
                settingService.SaveSetting(catalogSettings, settings => settings.DisplayFromPrices);
            }

            //#6115
            if (!settingService.SettingExists(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages))
            {
                catalogSettings.ShowShortDescriptionOnCatalogPages = false;
                settingService.SaveSetting(catalogSettings, settings => settings.ShowShortDescriptionOnCatalogPages);
            }

            var storeInformationSettings = settingService.LoadSetting<StoreInformationSettings>();

            //#3997
            if (!settingService.SettingExists(storeInformationSettings, settings => settings.InstagramLink))
            {
                storeInformationSettings.InstagramLink = "";
                settingService.SaveSetting(storeInformationSettings, settings => settings.InstagramLink);
            }

            var commonSettings = settingService.LoadSetting<CommonSettings>();

            //#5802
            if (!settingService.SettingExists(commonSettings, settings => settings.HeaderCustomHtml))
            {
                commonSettings.HeaderCustomHtml = "";
                settingService.SaveSetting(commonSettings, settings => settings.HeaderCustomHtml);
            }

            if (!settingService.SettingExists(commonSettings, settings => settings.FooterCustomHtml))
            {
                commonSettings.FooterCustomHtml = "";
                settingService.SaveSetting(commonSettings, settings => settings.FooterCustomHtml);
            }

            var orderSettings = settingService.LoadSetting<OrderSettings>();

            //#5604
            if (!settingService.SettingExists(orderSettings, settings => settings.ShowTvChannelThumbnailInOrderDetailsPage))
            {
                orderSettings.ShowTvChannelThumbnailInOrderDetailsPage = true;
                settingService.SaveSetting(orderSettings, settings => settings.ShowTvChannelThumbnailInOrderDetailsPage);
            }

            var mediaSettings = settingService.LoadSetting<MediaSettings>();

            //#5604
            if (!settingService.SettingExists(mediaSettings, settings => settings.OrderThumbPictureSize))
            {
                mediaSettings.OrderThumbPictureSize = 80;
                settingService.SaveSetting(mediaSettings, settings => settings.OrderThumbPictureSize);
            }

            var adminSettings = settingService.LoadSetting<AdminAreaSettings>();
            if (!settingService.SettingExists(adminSettings, settings => settings.CheckLicense))
            {
                adminSettings.CheckLicense = true;
                settingService.SaveSetting(adminSettings, settings => settings.CheckLicense);
            }

            var gdprSettings = settingService.LoadSetting<GdprSettings>();

            //#5809
            if (!settingService.SettingExists(gdprSettings, settings => settings.DeleteInactiveUsersAfterMonths))
            {
                gdprSettings.DeleteInactiveUsersAfterMonths = 36;
                settingService.SaveSetting(gdprSettings, settings => settings.DeleteInactiveUsersAfterMonths);
            }

            var captchaSettings = settingService.LoadSetting<CaptchaSettings>();

            //#6182
            if (!settingService.SettingExists(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests))
            {
                captchaSettings.ShowOnCheckoutPageForGuests = false;
                settingService.SaveSetting(captchaSettings, settings => settings.ShowOnCheckoutPageForGuests);
            }

            //#7
            if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeAllow))
            {
                mediaSettings.VideoIframeAllow = "fullscreen";
                settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeAllow);
            }

            //#7
            if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeWidth))
            {
                mediaSettings.VideoIframeWidth = 300;
                settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeWidth);
            }

            //#7
            if (!settingService.SettingExists(mediaSettings, settings => settings.VideoIframeHeight))
            {
                mediaSettings.VideoIframeHeight = 150;
                settingService.SaveSetting(mediaSettings, settings => settings.VideoIframeHeight);
            }

            //#385
            if (!settingService.SettingExists(catalogSettings, settings => settings.TvChannelUrlStructureTypeId))
            {
                catalogSettings.TvChannelUrlStructureTypeId = (int)TvChannelUrlStructureType.TvChannel;
                settingService.SaveSetting(catalogSettings, settings => settings.TvChannelUrlStructureTypeId);
            }

            //#5261
            var robotsTxtSettings = settingService.LoadSetting<RobotsTxtSettings>();

            if (!settingService.SettingExists(robotsTxtSettings, settings => settings.DisallowPaths))
            {
                robotsTxtSettings.DisallowPaths.AddRange(new[]
                {
                    "/admin",
                    "/bin/",
                    "/files/",
                    "/files/exportimport/",
                    "/country/getstatesbycountryid",
                    "/install",
                    "/settvchannelreviewhelpfulness",
                    "/*?*returnUrl="
                });

                settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowPaths);
            }

            if (!settingService.SettingExists(robotsTxtSettings, settings => settings.LocalizableDisallowPaths))
            {
                robotsTxtSettings.LocalizableDisallowPaths.AddRange(new[]
                {
                    "/addtvchanneltocart/catalog/",
                    "/addtvchanneltocart/details/",
                    "/backinstocksubscriptions/manage",
                    "/boards/forumsubscriptions",
                    "/boards/forumwatch",
                    "/boards/postedit",
                    "/boards/postdelete",
                    "/boards/postcreate",
                    "/boards/topicedit",
                    "/boards/topicdelete",
                    "/boards/topiccreate",
                    "/boards/topicmove",
                    "/boards/topicwatch",
                    "/cart$",
                    "/changecurrency",
                    "/changelanguage",
                    "/changetaxtype",
                    "/checkout",
                    "/checkout/billingaddress",
                    "/checkout/completed",
                    "/checkout/confirm",
                    "/checkout/shippingaddress",
                    "/checkout/shippingmethod",
                    "/checkout/paymentinfo",
                    "/checkout/paymentmethod",
                    "/clearcomparelist",
                    "/comparetvchannels",
                    "/comparetvchannels/add/*",
                    "/user/avatar",
                    "/user/activation",
                    "/user/addresses",
                    "/user/changepassword",
                    "/user/checkusernameavailability",
                    "/user/downloadabletvchannels",
                    "/user/info",
                    "/user/tvchannelreviews",
                    "/deletepm",
                    "/emailwishlist",
                    "/eucookielawaccept",
                    "/inboxupdate",
                    "/newsletter/subscriptionactivation",
                    "/onepagecheckout",
                    "/order/history",
                    "/orderdetails",
                    "/passwordrecovery/confirm",
                    "/poll/vote",
                    "/privatemessages",
                    "/recentlyviewedtvchannels",
                    "/returnrequest",
                    "/returnrequest/history",
                    "/rewardpoints/history",
                    "/search?",
                    "/sendpm",
                    "/sentupdate",
                    "/shoppingcart/*",
                    "/storeclosed",
                    "/subscribenewsletter",
                    "/topic/authenticate",
                    "/viewpm",
                    "/uploadfilecheckoutattribute",
                    "/uploadfiletvchannelattribute",
                    "/uploadfilereturnrequest",
                    "/wishlist"
                });

                settingService.SaveSetting(robotsTxtSettings, settings => settings.LocalizableDisallowPaths);
            }

            if (!settingService.SettingExists(robotsTxtSettings, settings => settings.DisallowLanguages))
                settingService.SaveSetting(robotsTxtSettings, settings => settings.DisallowLanguages);

            if (!settingService.SettingExists(robotsTxtSettings, settings => settings.AdditionsRules))
                settingService.SaveSetting(robotsTxtSettings, settings => settings.AdditionsRules);

            if (!settingService.SettingExists(robotsTxtSettings, settings => settings.AllowSitemapXml))
                settingService.SaveSetting(robotsTxtSettings, settings => settings.AllowSitemapXml);

            //#5753
            if (!settingService.SettingExists(mediaSettings, settings => settings.TvChannelDefaultImageId))
            {
                mediaSettings.TvChannelDefaultImageId = 0;
                settingService.SaveSetting(mediaSettings, settings => settings.TvChannelDefaultImageId);
            }

            //#3651
            if (!settingService.SettingExists(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail))
            {
                orderSettings.AttachPdfInvoiceToOrderProcessingEmail = false;
                settingService.SaveSetting(orderSettings, settings => settings.AttachPdfInvoiceToOrderProcessingEmail);
            }

            var taxSettings = settingService.LoadSetting<TaxSettings>();

            //#1961
            if (!settingService.SettingExists(taxSettings, settings => settings.EuVatEnabledForGuests))
            {
                taxSettings.EuVatEnabledForGuests = false;
                settingService.SaveSetting(taxSettings, settings => settings.EuVatEnabledForGuests);
            }

            //#5570
            var sitemapXmlSettings = settingService.LoadSetting<SitemapXmlSettings>();

            if (!settingService.SettingExists(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours))
            {
                sitemapXmlSettings.RebuildSitemapXmlAfterHours = 2 * 24;
                settingService.SaveSetting(sitemapXmlSettings, settings => settings.RebuildSitemapXmlAfterHours);
            }

            if (!settingService.SettingExists(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay))
            {
                sitemapXmlSettings.SitemapBuildOperationDelay = 60;
                settingService.SaveSetting(sitemapXmlSettings, settings => settings.SitemapBuildOperationDelay);
            }

            //#6378
            if (!settingService.SettingExists(mediaSettings, settings => settings.AllowSVGUploads))
            {
                mediaSettings.AllowSVGUploads = false;
                settingService.SaveSetting(mediaSettings, settings => settings.AllowSVGUploads);
            }

            //#5599
            var messagesSettings = settingService.LoadSetting<MessagesSettings>();

            if (!settingService.SettingExists(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails))
            {
                messagesSettings.UseDefaultEmailAccountForSendStoreOwnerEmails = false;
                settingService.SaveSetting(messagesSettings, settings => settings.UseDefaultEmailAccountForSendStoreOwnerEmails);
            }

            //#228
            if (!settingService.SettingExists(catalogSettings, settings => settings.ActiveSearchProviderSystemName))
            {
                catalogSettings.ActiveSearchProviderSystemName = string.Empty;
                settingService.SaveSetting(catalogSettings, settings => settings.ActiveSearchProviderSystemName);
            }

            //#43
            var metaTitleKey = $"{nameof(SeoSettings)}.DefaultTitle".ToLower();
            var metaKeywordsKey = $"{nameof(SeoSettings)}.DefaultMetaKeywords".ToLower();
            var metaDescriptionKey = $"{nameof(SeoSettings)}.DefaultMetaDescription".ToLower();
            var homepageTitleKey = $"{nameof(SeoSettings)}.HomepageTitle".ToLower();
            var homepageDescriptionKey = $"{nameof(SeoSettings)}.HomepageDescription".ToLower();

            var settingRepository = EngineContext.Current.Resolve<IRepository<Setting>>();
            var storeService = EngineContext.Current.Resolve<IStoreService>();

            foreach (var store in storeService.GetAllStores())
            {
                var metaTitle = settingService.GetSettingByKey<string>(metaTitleKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaTitleKey);
                var metaKeywords = settingService.GetSettingByKey<string>(metaKeywordsKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaKeywordsKey);
                var metaDescription = settingService.GetSettingByKey<string>(metaDescriptionKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(metaDescriptionKey);
                var homepageTitle = settingService.GetSettingByKey<string>(homepageTitleKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(homepageTitleKey);
                var homepageDescription = settingService.GetSettingByKey<string>(homepageDescriptionKey, storeId: store.Id) ?? settingService.GetSettingByKey<string>(homepageDescriptionKey);

                if (metaTitle != null)
                    store.DefaultTitle = metaTitle;

                if (metaKeywords != null)
                    store.DefaultMetaKeywords = metaKeywords;

                if (metaDescription != null)
                    store.DefaultMetaDescription = metaDescription;

                if (homepageTitle != null)
                    store.HomepageTitle = homepageTitle;

                if (homepageDescription != null)
                    store.HomepageDescription = homepageDescription;

                storeService.UpdateStore(store);
            }

            settingRepository.Delete(setting => setting.Name == metaTitleKey);
            settingRepository.Delete(setting => setting.Name == metaKeywordsKey);
            settingRepository.Delete(setting => setting.Name == metaDescriptionKey);
            settingRepository.Delete(setting => setting.Name == homepageTitleKey);
            settingRepository.Delete(setting => setting.Name == homepageDescriptionKey);

            //#6464
            var pdfSettings = settingService.LoadSetting<PdfSettings>();
            if (!settingService.SettingExists(pdfSettings, settings => settings.FontFamily))
            {
                pdfSettings.FontFamily = "FreeSerif";
                settingService.SaveSetting(pdfSettings, settings => settings.FontFamily);

                //delete old setting
                settingRepository.Delete(setting => setting.Name == $"{nameof(PdfSettings)}.FontFileName".ToLower());
            }
        }

        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}