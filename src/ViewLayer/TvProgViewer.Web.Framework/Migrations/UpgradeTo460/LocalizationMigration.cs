using System.Collections.Generic;
using FluentMigrator;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Migrations;
using TvProgViewer.Services.Localization;
using TvProgViewer.Web.Framework.Extensions;

namespace TvProgViewer.Web.Framework.Migrations.UpgradeTo460
{
    [TvProgMigration("2022-02-07 00:00:00", "4.60.0", UpdateMigrationType.Localization, MigrationProcessType.Update)]
    public class LocalizationMigration : MigrationBase
    {
        /// <summary>Collect the UP migration expressions</summary>
        public override void Up()
        {
            if (!DataSettingsManager.IsDatabaseInstalled())
                return;

            //do not use DI, because it produces exception on the installation process
            var localizationService = EngineContext.Current.Resolve<ILocalizationService>();

            var (languageId, languages) = this.GetLanguageData();

            #region Delete locales

            localizationService.DeleteLocaleResources(new List<string>
            {
                //#6102
                "Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup",
                "Admin.Configuration.AppSettings.Plugin.ClearPluginShadowDirectoryOnStartup.Hint",
                "Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup",
                "Admin.Configuration.AppSettings.Plugin.CopyLockedPluginAssembilesToSubdirectoriesOnStartup.Hint",
                "Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy",
                "Admin.Configuration.AppSettings.Plugin.UsePluginsShadowCopy.Hint",

                //#5123
                "Admin.Catalog.TvChannels.Pictures.Alert.AddNew",

                //#7
                "Admin.Catalog.TvChannels.Pictures.SaveBeforeEdit",
                "Admin.Catalog.TvChannels.Pictures.AddButton",

                "Admin.Configuration.AppSettings.Common.SupportPreviousTvProgcommerceVersions",
                "Admin.Configuration.AppSettings.Common.SupportPreviousTvProgcommerceVersions.Hint",

				//4622
                "PDFInvoice.OrderDate",
                "PDFInvoice.Company",
                "PDFInvoice.Name",
                "PDFInvoice.SmartPhone",
                "PDFInvoice.Fax",
                "PDFInvoice.Address",
                "PDFInvoice.Address2",
                "PDFInvoice.VATNumber",
                "PDFInvoice.PaymentMethod",
                "PDFInvoice.ShippingMethod",
                "PDFInvoice.BillingInformation",
                "PDFInvoice.ShippingInformation",
                "PDFInvoice.OrderNotes",
                "PDFInvoice.OrderNotes.CreatedOn",
                "PDFInvoice.OrderNotes.Note",
                "PDFPackagingSlip.Shipment",
                "PDFInvoice.Order#",
                "PDFInvoice.Discount",
                "PDFInvoice.Sub-Total",
                "PDFInvoice.Shipping",
                "PDFInvoice.OrderTotal",
                "PDFInvoice.PaymentMethodAdditionalFee",
                "PDFInvoice.Pickup",
                "PDFInvoice.TvChannel(s)",
                "PDFInvoice.Tax",
                "PDFPackagingSlip.Address",
                "PDFPackagingSlip.Address2",
                "PDFPackagingSlip.Company",
                "PDFPackagingSlip.Name",
                "PDFPackagingSlip.Order",
                "PDFPackagingSlip.SmartPhone",
                "PDFPackagingSlip.TvChannelName",
                "PDFPackagingSlip.QTY",
                "PDFPackagingSlip.ShippingMethod",
                "PDFPackagingSlip.SKU",
                "PDFTvChannelCatalog.Price",
                "PDFTvChannelCatalog.SKU",

            });

            #endregion

            #region Add or update locales

            localizationService.AddOrUpdateLocaleResource(new Dictionary<string, string>
            {
                //#3075
                ["Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithCategoryName"] = "Allow users to search with category name",
                ["Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithCategoryName.Hint"] = "Check to allow users to search with category name.",
                ["Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithManufacturerName"] = "Allow users to search with manufacturer name",
                ["Admin.Configuration.Settings.Catalog.AllowUsersToSearchWithManufacturerName.Hint"] = "Check to allow users to search with manufacturer name.",

                //#3997
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink"] = "Instagram URL",
                ["Admin.Configuration.Settings.GeneralCommon.InstagramLink.Hint"] = "Specify your Instagram page URL. Leave empty if you have no such page.",

                ["Footer.FollowUs.Instagram"] = "Instagram",

                //#5802
                ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.CustomHtml"] = "Custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml"] = "Footer custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.FooterCustomHtml.Hint"] = "Enter custom HTML here for footer section.",
                ["Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml"] = "Header custom HTML",
                ["Admin.Configuration.Settings.GeneralCommon.HeaderCustomHtml.Hint"] = "Enter custom HTML here for header section.",

                //#5604
                ["Admin.Configuration.Settings.Order.ShowTvChannelThumbnailInOrderDetailsPage"] = "Show tvChannel thumbnail in order details page",
                ["Admin.Configuration.Settings.Order.ShowTvChannelThumbnailInOrderDetailsPage.Hint"] = "Check to show tvChannel thumbnail in order details page.",
                ["Admin.Configuration.Settings.Media.OrderThumbPictureSize"] = "Order thumbnail image size",
                ["Admin.Configuration.Settings.Media.OrderThumbPictureSize.Hint"] = "The default size (pixels) for tvChannel thumbnail images on the order details page.",
                ["Order.TvChannel(s).Image"] = "Image",

                //#3777
                ["ActivityLog.ExportCategories"] = "{0} categories were exported",
                ["ActivityLog.ExportUsers"] = "{0} users were exported",
                ["ActivityLog.ExportManufacturers"] = "{0} manufacturers were exported",
                ["ActivityLog.ExportOrders"] = "{0} orders were exported",
                ["ActivityLog.ExportTvChannels"] = "{0} tvChannels were exported",
                ["ActivityLog.ExportStates"] = "{0} states and provinces were exported",
                ["ActivityLog.ExportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were exported",
                ["ActivityLog.ImportNewsLetterSubscriptions"] = "{0} newsletter subscriptions were imported",

                //#5947
                ["Admin.Users.Users.List.SearchLastActivityFrom"] = "Last activity from",
                ["Admin.Users.Users.List.SearchLastActivityFrom.Hint"] = "The last activity from date for the search.",
                ["Admin.Users.Users.List.SearchLastActivityTo"] = "Last activity to",
                ["Admin.Users.Users.List.SearchLastActivityTo.Hint"] = "The last activity to date for the search.",
                ["Admin.Users.Users.List.SearchRegistrationDateFrom"] = "Registration date from",
                ["Admin.Users.Users.List.SearchRegistrationDateFrom.Hint"] = "The registration from date for the search.",
                ["Admin.Users.Users.List.SearchRegistrationDateTo"] = "Registration date to",
                ["Admin.Users.Users.List.SearchRegistrationDateTo.Hint"] = "The registration to date for the search.",

                //#5313
                ["ActivityLog.ImportOrders"] = "{0} orders were imported",
                ["Admin.Orders.Import.UsersDontExist"] = "Users with the following GUIDs don't exist: {0}",
                ["Admin.Orders.Import.TvChannelsDontExist"] = "TvChannels with the following SKUs don't exist: {0}",
                ["Admin.Orders.Imported"] = "Orders have been imported successfully.",
                ["Admin.Orders.List.ImportFromExcelTip"] = "Imported orders are distinguished by order GUID. If the order GUID already exists, then its details will be updated.",

                //#1933
                ["Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages"] = "Display all pictures on catalog pages",
                ["Admin.Configuration.Settings.Catalog.DisplayAllPicturesOnCatalogPages.Hint"] = "Check to display all pictures on catalog pages.",

                //#3511
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsAllowUsersToSelectPageSize"] = "'New tvChannels' page. Allow users to select page size",
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsAllowUsersToSelectPageSize.Hint"] = "'New tvChannels' page. Check to allow users to select the page size from a predefined list of options.",
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsPageSizeOptions"] = "'New tvChannels' page. Page size options",
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsPageSizeOptions.Hint"] = "'New tvChannels' page. Comma separated list of page size options (e.g. 10, 5, 15, 20). First option is the default page size if none are selected.",

                //#5123
                ["Admin.Catalog.TvChannels.Pictures.Fields.Picture.Hint"] = "You can choose multiple images to upload at once. If the picture size exceeds your stores max image size setting, it will be automatically resized.",
                ["Common.FileUploader.Upload.Files"] = "Upload files",

                //#5809
                ["Admin.Configuration.Settings.Gdpr.DeleteInactiveUsersAfterMonths"] = "Delete inactive users after months",
                ["Admin.Configuration.Settings.Gdpr.DeleteInactiveUsersAfterMonths.Hint"] = "Enter the number of months after which the users and their personal data will be deleted.",

                //#29
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices"] = "Display 'From' prices",
                ["Admin.Configuration.Settings.Catalog.DisplayFromPrices.Hint"] = "Check to display 'From' prices on catalog pages. This will display the minimum possible price of a tvChannel based on price adjustments of attributes and combinations instead of the fixed base price. If enabled, it is also recommended to enable setting 'Cache tvChannel prices'. But please note that it can affect performance if you use some complex discounts, discount requirement rules, etc.",

                //#5089
                ["TvChannels.Availability.LowStock"] = "Low stock",
                ["TvChannels.Availability.LowStockWithQuantity"] = "{0} low stock",
                ["Admin.Catalog.TvChannels.Fields.LowStockActivity.Hint"] = "Action to be taken when your current stock quantity falls below (reaches) the 'Minimum stock quantity'. Activation of the action will occur only after an order is placed. If the value is 'Nothing', the tvChannel detail page will display a low-stock message in public store.",

                //#6101
                ["Admin.System.Warnings.PluginNotInstalled.HelpText"] = "You may delete the plugins you don't use in order to decrease startup time",

                //#6182
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckoutPageForGuests"] = "Show on checkout page for guests",
                ["Admin.Configuration.Settings.GeneralCommon.CaptchaShowOnCheckoutPageForGuests.Hint"] = "Check to show CAPTCHA on checkout page for guests.",

                //#6111
                ["Admin.ReturnRequests.Fields.ReturnedQuantity.Hint"] = "The quantity to be returned to the stock.",

                //#7
                ["Admin.Catalog.TvChannels.Multimedia"] = "Multimedia",
                ["Admin.Catalog.TvChannels.Multimedia.Videos"] = "Videos",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.SaveBeforeEdit"] = "You need to save the tvChannel before you can upload videos for this tvChannel page.",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.AddNew"] = "Add a new video",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Alert.VideoAdd"] = "Failed to add tvChannel video.",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Alert.VideoUpdate"] = "Failed to update tvChannel video.",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Fields.DisplayOrder"] = "Display order",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Fields.DisplayOrder.Hint"] = "Display order of the video. 1 represents the top of the list.",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Fields.Preview"] = "Preview",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Fields.VideoUrl"] = "Embed video URL",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.Fields.VideoUrl.Hint"] = "Specify the URL path to the video.",
                ["Admin.Catalog.TvChannels.Multimedia.Videos.AddButton"] = "Add tvChannel video",
                ["Admin.Catalog.TvChannels.Copy.CopyMultimedia"] = "Copy multimedia",
                ["Admin.Catalog.TvChannels.Copy.CopyMultimedia.Hint"] = "Check to copy the images and videos.",

                //#6115
                ["Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages"] = "Show short description on catalog pages",
                ["Admin.Configuration.Settings.Catalog.ShowShortDescriptionOnCatalogPages.Hint"] = "Check to show tvChannel short description on catalog pages.",

                //#5905
                ["Admin.ContentManagement.MessageTemplates.List.IsActive"] = "Is active",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.ActiveOnly"] = "Active only",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.All"] = "All",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.Hint"] = "Search by a \"IsActive\" property.",
                ["Admin.ContentManagement.MessageTemplates.List.IsActive.InactiveOnly"] = "Inactive only",

                //#6062
                ["Account.UserAddresses.Added"] = "The new address has been added successfully.",
                ["Account.UserAddresses.Updated"] = "The address has been updated successfully.",
                ["Account.UserInfo.Updated"] = "The user info has been updated successfully.",

                //#385
                ["Admin.Configuration.Settings.Catalog.TvChannelUrlStructureType"] = "TvChannel URL structure type",
                ["Admin.Configuration.Settings.Catalog.TvChannelUrlStructureType.Hint"] = "Select the tvChannel URL structure type (e.g. '/tvChannel-seo-name' or '/category-seo-name/tvChannel-seo-name' or '/manufacturer-seo-name/tvChannel-seo-name').",
                ["Enums.TvProg.Core.Domain.Catalog.TvChannelUrlStructureType.CategoryTvChannel"] = "/Category/TvChannel",
                ["Enums.TvProg.Core.Domain.Catalog.TvChannelUrlStructureType.ManufacturerTvChannel"] = "/Manufacturer/TvChannel",
                ["Enums.TvProg.Core.Domain.Catalog.TvChannelUrlStructureType.TvChannel"] = "/TvChannel",

                //#5261
                ["Admin.Configuration.Settings.GeneralCommon.BlockTitle.RobotsTxt"] = "robots.txt",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsInstruction"] = "You also may extend the robots.txt data by adding the {0} file to the wwwroot directory of your site.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules"] = "Additions rules",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAdditionsRules.Hint"] = "Enter additional rules for the robots.txt file.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml"] = "Allow sitemap.xml",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsAllowSitemapXml.Hint"] = "Check to allow robots to access the sitemap.xml file.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsCustomFileExists"] = "robots.txt file data overridden by {0} file in site root.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages"] = "Disallow languages",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowLanguages.Hint"] = "The list of languages to disallow.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths"] = "Disallow paths",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsDisallowPaths.Hint"] = "The list of paths to disallow.",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths"] = "Localizable disallow paths",
                ["Admin.Configuration.Settings.GeneralCommon.RobotsLocalizableDisallowPaths.Hint"] = "The list of localizable paths to disallow.",

                //#5753
                ["Admin.Configuration.Settings.Media.TvChannelDefaultImage"] = "Default image",
                ["Admin.Configuration.Settings.Media.TvChannelDefaultImage.Hint"] = "Upload a picture to be used as the default image. If nothing is uploaded, {0} will be used.",

                ["Admin.Help.Training"] = "Training",

                //5607
                ["Admin.Configuration.Settings.UserUser.ForceMultifactorAuthentication.Hint"] = "Force activation of multi-factor authentication for user roles specified in Access control list (at least one MFA provider must be active).",
                ["Permission.Authentication.EnableMultiFactorAuthentication"] = "Security. Enable Multi-factor authentication",

                //#3651
                ["Admin.ContentManagement.MessageTemplates.Description.OrderProcessing.UserNotification"] = "This message template is used to notify a user that the certain order is processing. Orders can be viewed by a user on the account page.",
                ["Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderProcessingEmail"] = "Attach PDF invoice (\"order processing\" email)",
                ["Admin.Configuration.Settings.Order.AttachPdfInvoiceToOrderProcessingEmail.Hint"] = "Check to attach PDF invoice to the \"order processing\" email sent to a user.",

                //5705
                ["Admin.Promotions.Discounts.Fields.IsActive"] = "Is active",
                ["Admin.Promotions.Discounts.Fields.IsActive.Hint"] = "Indicating whether the discount is active.",
                ["Admin.Promotions.Discounts.List.IsActive"] = "Is Active",
                ["Admin.Promotions.Discounts.List.IsActive.ActiveOnly"] = "Active only",
                ["Admin.Promotions.Discounts.List.IsActive.All"] = "All",
                ["Admin.Promotions.Discounts.List.IsActive.Hint"] = "Search by \"IsActive\" property.",
                ["Admin.Promotions.Discounts.List.IsActive.InactiveOnly"] = "Inactive only",

                //#1961
                ["Admin.Configuration.Settings.Tax.EuVatEnabledForGuests"] = "EU VAT enabled for guests",
                ["Admin.Configuration.Settings.Tax.EuVatEnabledForGuests.Hint"] = "Check to enable EU VAT (the European Union Value Added Tax) for guest users. They will have to enter it during the checkout at the billing address step.",
                ["Checkout.VatNumber"] = "VAT number",
                ["Checkout.VatNumber.Disabled"] = "VAT number can be entered (on the <a href=\"{0}\">user info page</a>) and used only after registration",
                ["Checkout.VatNumber.Warning"] = "VAT number is {0}",

                //#4591
                ["Admin.Configuration.Stores.Fields.SslEnabled"] = "SSL",
                ["Admin.Configuration.Stores.Fields.SslEnabled.Hint"] = "SSL (Secure Socket Layer) is the standard security technology for establishing an encrypted connection between a web server and the browser. This ensures that all data exchanged between web server and browser arrives unchanged.",
                ["Admin.Configuration.Stores.Ssl.Enable"] = "Enable SSL",
                ["Admin.Configuration.Stores.Ssl.Disable"] = "Disable SSL",
                ["Admin.Configuration.Stores.Ssl.Updated"] = "The SSL setting has been successfully changed. Do not forget to synchronize the store URL with the current HTTP protocol.",

                ["Admin.Reports.SalesSummary.Vendor"] = "Vendor",
                ["Admin.Reports.SalesSummary.Vendor.Hint"] = "Search by a specific vendor.",

                //#6353
                ["Admin.Promotions.Discounts.Fields.CouponCode.Reserved"] = "The entered coupon code is already reserved for the discount '{0}'",

                //#6378
                ["Admin.Configuration.Settings.Media.AllowSVGUploads"] = "Allow SVG uploads in admin area",
                ["Admin.Configuration.Settings.Media.AllowSVGUploads.Hint"] = "Check to allow uploading of SVG files in admin area.",

                //#6396
                ["Admin.Catalog.TvChannels.Fields.MinStockQuantity.Hint"] = "If you track inventory, you can perform a number of different actions when the current stock quantity falls below (reaches) your minimum stock quantity.",
                ["Admin.Catalog.TvChannels.TvChannelAttributes.AttributeCombinations.Fields.MinStockQuantity.Hint"] = "If you track inventory by tvChannel attributes, you can perform a number of different actions when the current stock quantity falls below (reaches) your minimum stock quantity (e.g. Low stock report).",
                //#6213
                ["Admin.System.Maintenance.DeleteMinificationFiles"] = "Delete minification files",
                ["Admin.System.Maintenance.DeleteMinificationFiles.Text"] = "Clear the bundles directory.",
                ["Admin.System.Maintenance.DeleteMinificationFiles.TotalDeleted"] = "{0} files were deleted",

                //#6336
                ["Admin.Users.Users.RewardPoints.Fields.AddNegativePointsValidity"] = "Points validity is not allowed for point reduction.",

                //#6411
                ["Admin.StockQuantityHistory.Messages.ReadyForPickupByUser"] = "The stock quantity has been reduced when an order item of the order #{0} became a ready for pickup by user",

                //4622
                ["Pdf.OrderDate"] = "Date",
                ["Pdf.Address.Company"] = "Company",
                ["Pdf.Address.Name"] = "Name",
                ["Pdf.Address.SmartPhone"] = "SmartPhone",
                ["Pdf.Address.Fax"] = "Fax",
                ["Pdf.Address"] = "Address",
                ["Pdf.Address2"] = "Address 2",
                ["Pdf.Address.VATNumber"] = "VAT number",
                ["Pdf.BillingInformation"] = "Billing Information",
                ["Pdf.ShippingInformation"] = "Shipping Information",
                ["Pdf.OrderNotes"] = "Order notes",
                ["Pdf.Address.PaymentMethod"] = "Payment method",
                ["Pdf.Address.ShippingMethod"] = "Shipping method",
                ["Pdf.Shipment"] = "Shipment",
                ["Pdf.Order"] = "Order",
                ["Pdf.Shipping"] = "Shipping",
                ["Pdf.SubTotal"] = "Sub-total",
                ["Pdf.Discount"] = "Discount",
                ["Pdf.OrderTotal"] = "Order total",
                ["Pdf.PaymentMethodAdditionalFee"] = "Payment Method Additional Fee",
                ["Pdf.PickupPoint"] = "Pickup point",
                ["Pdf.Tax"] = "Tax",

                //#43
                ["admin.configuration.stores.info"] = "Info",

                //5701
                ["Admin.Configuration.AppSettings.Common.UseAutofac"] = "Use Autofac IoC",
                ["Admin.Configuration.AppSettings.Common.UseAutofac.Hint"] = "The value indicating whether to use Autofac IoC container. If disabled, then the default .Net IoC container will be used.",
            }, languageId);

            #endregion

            #region Rename locales

            this.RenameLocales(new Dictionary<string, string>
            {
                //#6255
                ["Forum.BreadCrumb.HomeTitle"] = "Forum.Breadcrumb.HomeTitle",
                ["Forum.BreadCrumb.ForumHomeTitle"] = "Forum.Breadcrumb.ForumHomeTitle",
                ["Forum.BreadCrumb.ForumGroupTitle"] = "Forum.Breadcrumb.ForumGroupTitle",
                ["Forum.BreadCrumb.ForumTitle"] = "Forum.Breadcrumb.ForumTitle",
                ["Forum.BreadCrumb.TopicTitle"] = "Forum.Breadcrumb.TopicTitle",

                //#3511
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsNumber"] = "Admin.Configuration.Settings.Catalog.NewTvChannelsPageSize",
                ["Admin.Configuration.Settings.Catalog.NewTvChannelsNumber.Hint"] = "Admin.Configuration.Settings.Catalog.NewTvChannelsPageSize.Hint",

                //#7
                ["Admin.Catalog.TvChannels.Pictures"] = "Admin.Catalog.TvChannels.Multimedia.Pictures",
                ["Admin.Catalog.TvChannels.Pictures.AddNew"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.AddNew",
                ["Admin.Catalog.TvChannels.Pictures.Alert.PictureAdd"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Alert.PictureAdd",
                ["Admin.Catalog.TvChannels.Pictures.Fields.DisplayOrder"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.DisplayOrder",
                ["Admin.Catalog.TvChannels.Pictures.Fields.DisplayOrder.Hint"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.DisplayOrder.Hint",
                ["Admin.Catalog.TvChannels.Pictures.Fields.OverrideAltAttribute"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideAltAttribute",
                ["Admin.Catalog.TvChannels.Pictures.Fields.OverrideAltAttribute.Hint"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideAltAttribute.Hint",
                ["Admin.Catalog.TvChannels.Pictures.Fields.OverrideTitleAttribute"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideTitleAttribute",
                ["Admin.Catalog.TvChannels.Pictures.Fields.OverrideTitleAttribute.Hint"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.OverrideTitleAttribute.Hint",
                ["Admin.Catalog.TvChannels.Pictures.Fields.Picture"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.Picture",
                ["Admin.Catalog.TvChannels.Pictures.Fields.Picture.Hint"] = "Admin.Catalog.TvChannels.Multimedia.Pictures.Fields.Picture.Hint",
                ["Admin.Catalog.TvChannels.Copy.CopyImages"] = "Admin.Catalog.TvChannels.Copy.CopyMultimedia",
                ["Admin.Catalog.TvChannels.Copy.CopyImages.Hint"] = "Admin.Catalog.TvChannels.Copy.CopyMultimedia.Hint",

                //#43
                ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription"] = "Admin.Configuration.Stores.Fields.DefaultMetaDescription",
                ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaDescription.Hint"] = "Admin.Configuration.Stores.Fields.DefaultMetaDescription.Hint",
                ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords"] = "Admin.Configuration.Stores.Fields.DefaultMetaKeywords",
                ["Admin.Configuration.Settings.GeneralCommon.DefaultMetaKeywords.Hint"] = "Admin.Configuration.Stores.Fields.DefaultMetaKeywords.Hint",
                ["Admin.Configuration.Settings.GeneralCommon.DefaultTitle"] = "Admin.Configuration.Stores.Fields.DefaultTitle",
                ["Admin.Configuration.Settings.GeneralCommon.DefaultTitle.Hint"] = "Admin.Configuration.Stores.Fields.DefaultTitle.Hint",
                ["Admin.Configuration.Settings.GeneralCommon.HomepageDescription"] = "Admin.Configuration.Stores.Fields.HomepageDescription",
                ["Admin.Configuration.Settings.GeneralCommon.HomepageDescription.Hint"] = "Admin.Configuration.Stores.Fields.HomepageDescription.Hint",
                ["Admin.Configuration.Settings.GeneralCommon.HomepageTitle"] = "Admin.Configuration.Stores.Fields.HomepageTitle",
                ["Admin.Configuration.Settings.GeneralCommon.HomepageTitle.Hint"] = "Admin.Configuration.Stores.Fields.HomepageTitle.Hint",

                //4622
                ["PDFInvoice.TvChannelName"] = "Pdf.TvChannel.Name",
                ["PDFInvoice.SKU"] = "Pdf.TvChannel.Sku",
                ["PDFInvoice.VendorName"] = "Pdf.TvChannel.VendorName",
                ["PDFTvChannelCatalog.Weight"] = "Pdf.TvChannel.Weight",
                ["PDFInvoice.TvChannelPrice"] = "Pdf.TvChannel.Price",
                ["PDFInvoice.TvChannelQuantity"] = "Pdf.TvChannel.Quantity",
                ["PDFTvChannelCatalog.StockQuantity"] = "Pdf.TvChannel.StockQuantity",
                ["PDFInvoice.TvChannelTotal"] = "Pdf.TvChannel.Total",
                ["PDFInvoice.RewardPoints"] = "Pdf.RewardPoints",
                ["PDFInvoice.TaxRate"] = "Pdf.TaxRate",
                ["PDFInvoice.GiftCardInfo"] = "Pdf.GiftCardInfo"
            }, languages, localizationService);

            #endregion
        }

        /// <summary>Collects the DOWN migration expressions</summary>
        public override void Down()
        {
            //add the downgrade logic if necessary 
        }
    }
}
