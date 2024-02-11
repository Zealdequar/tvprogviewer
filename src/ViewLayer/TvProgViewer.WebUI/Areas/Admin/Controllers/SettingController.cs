using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Core.Domain;
using TvProgViewer.Core.Domain.Blogs;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Forums;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Core.Domain.Localization;
using TvProgViewer.Core.Domain.Media;
using TvProgViewer.Core.Domain.News;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Core.Domain.Security;
using TvProgViewer.Core.Domain.Seo;
using TvProgViewer.Core.Domain.Shipping;
using TvProgViewer.Core.Domain.Tax;
using TvProgViewer.Core.Domain.Vendors;
using TvProgViewer.Core.Events;
using TvProgViewer.Core.Infrastructure;
using TvProgViewer.Data;
using TvProgViewer.Data.Configuration;
using TvProgViewer.Services.Authentication.MultiFactor;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Gdpr;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Media;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Orders;
using TvProgViewer.Services.Plugins;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Settings;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;
using TvProgViewer.Web.Framework.Mvc.ModelBinding;
using TvProgViewer.Web.Framework.WebOptimizer;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class SettingController : BaseAdminController
    {
        #region Fields

        private readonly AppSettings _appSettings;
        private readonly IAddressService _addressService;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly ITvProgDataProvider _dataProvider;
        private readonly IEncryptionService _encryptionService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGdprService _gdprService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly ITvProgFileProvider _fileProvider;
        private readonly INotificationService _notificationService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly ISettingModelFactory _settingModelFactory;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly IUploadService _uploadService;

        #endregion

        #region Ctor

        public SettingController(AppSettings appSettings,
            IAddressService addressService,
            IUserActivityService userActivityService,
            IUserService userService,
            ITvProgDataProvider dataProvider,
            IEncryptionService encryptionService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IGdprService gdprService,
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            ITvProgFileProvider fileProvider,
            INotificationService notificationService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            ISettingModelFactory settingModelFactory,
            ISettingService settingService,
            IStoreContext storeContext,
            IStoreService storeService,
            IWorkContext workContext,
            IUploadService uploadService)
        {
            _appSettings = appSettings;
            _addressService = addressService;
            _userActivityService = userActivityService;
            _userService = userService;
            _dataProvider = dataProvider;
            _encryptionService = encryptionService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _gdprService = gdprService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _fileProvider = fileProvider;
            _notificationService = notificationService;
            _orderService = orderService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _settingModelFactory = settingModelFactory;
            _settingService = settingService;
            _storeContext = storeContext;
            _storeService = storeService;
            _workContext = workContext;
            _uploadService = uploadService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateGdprConsentLocalesAsync(GdprConsent gdprConsent, GdprConsentModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(gdprConsent,
                    x => x.Message,
                    localized.Message,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(gdprConsent,
                    x => x.RequiredMessage,
                    localized.RequiredMessage,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> ChangeStoreScopeConfiguration(int storeid, string returnUrl = "")
        {
            var store = await _storeService.GetStoreByIdAsync(storeid);
            if (store != null || storeid == 0)
            {
                await _genericAttributeService
                    .SaveAttributeAsync(await _workContext.GetCurrentUserAsync(), TvProgUserDefaults.AdminAreaStoreScopeConfigurationAttribute, storeid);
            }

            //home page
            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = Url.Action("Index", "Home", new { area = AreaNames.Admin });

            //prevent open redirection attack
            if (!Url.IsLocalUrl(returnUrl))
                return RedirectToAction("Index", "Home", new { area = AreaNames.Admin });

            return Redirect(returnUrl);
        }

        public virtual async Task<IActionResult> AppSettings()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAppSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareAppSettingsModel();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AppSettings(AppSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAppSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var configurations = new List<IConfig>
                {
                    model.CacheConfigModel.ToConfig(_appSettings.Get<CacheConfig>()),
                    model.HostingConfigModel.ToConfig(_appSettings.Get<HostingConfig>()),
                    model.DistributedCacheConfigModel.ToConfig(_appSettings.Get<DistributedCacheConfig>()),
                    model.AzureBlobConfigModel.ToConfig(_appSettings.Get<AzureBlobConfig>()),
                    model.InstallationConfigModel.ToConfig(_appSettings.Get<InstallationConfig>()),
                    model.PluginConfigModel.ToConfig(_appSettings.Get<PluginConfig>()),
                    model.CommonConfigModel.ToConfig(_appSettings.Get<CommonConfig>()),
                    model.DataConfigModel.ToConfig(_appSettings.Get<DataConfig>()),
                    model.WebOptimizerConfigModel.ToConfig(_appSettings.Get<WebOptimizerConfig>())
                };

                await _eventPublisher.PublishAsync(new AppSettingsSavingEvent(configurations));

                AppSettingsHelper.SaveAppSettings(configurations, _fileProvider);

                await _userActivityService.InsertActivityAsync("EditSettings",
                    await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(
                    await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                var returnUrl = Url.Action("AppSettings", "Setting", new { area = AreaNames.Admin });
                return View("RestartApplication", returnUrl);
            }

            //prepare model
            model = await _settingModelFactory.PrepareAppSettingsModel(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Blog()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareBlogSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Blog(BlogSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var blogSettings = await _settingService.LoadSettingAsync<BlogSettings>(storeScope);
                blogSettings = model.ToSettings(blogSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.NotifyAboutNewBlogComments, model.NotifyAboutNewBlogComments_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.NumberOfTags, model.NumberOfTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(blogSettings, x => x.BlogCommentsMustBeApproved, model.BlogCommentsMustBeApproved_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingAsync(blogSettings, x => x.ShowBlogCommentsPerStore, clearCache: false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Blog");
            }

            //prepare model
            model = await _settingModelFactory.PrepareBlogSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Vendor()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareVendorSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Vendor(VendorSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var vendorSettings = await _settingService.LoadSettingAsync<VendorSettings>(storeScope);
                vendorSettings = model.ToSettings(vendorSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.VendorsBlockItemsToDisplay, model.VendorsBlockItemsToDisplay_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.ShowVendorOnTvChannelDetailsPage, model.ShowVendorOnTvChannelDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.ShowVendorOnOrderDetailsPage, model.ShowVendorOnOrderDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.AllowUsersToContactVendors, model.AllowUsersToContactVendors_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.AllowUsersToApplyForVendorAccount, model.AllowUsersToApplyForVendorAccount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.TermsOfServiceEnabled, model.TermsOfServiceEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.AllowSearchByVendor, model.AllowSearchByVendor_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.AllowVendorsToEditInfo, model.AllowVendorsToEditInfo_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.NotifyStoreOwnerAboutVendorInformationChange, model.NotifyStoreOwnerAboutVendorInformationChange_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.MaximumTvChannelNumber, model.MaximumTvChannelNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(vendorSettings, x => x.AllowVendorsToImportTvChannels, model.AllowVendorsToImportTvChannels_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Vendor");
            }

            //prepare model
            model = await _settingModelFactory.PrepareVendorSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Forum()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareForumSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Forum(ForumSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var forumSettings = await _settingService.LoadSettingAsync<ForumSettings>(storeScope);
                forumSettings = model.ToSettings(forumSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumsEnabled, model.ForumsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.RelativeDateTimeFormattingEnabled, model.RelativeDateTimeFormattingEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ShowUsersPostCount, model.ShowUsersPostCount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowGuestsToCreatePosts, model.AllowGuestsToCreatePosts_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowGuestsToCreateTopics, model.AllowGuestsToCreateTopics_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowUsersToEditPosts, model.AllowUsersToEditPosts_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowUsersToDeletePosts, model.AllowUsersToDeletePosts_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowPostVoting, model.AllowPostVoting_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.MaxVotesPerDay, model.MaxVotesPerDay_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowUsersToManageSubscriptions, model.AllowUsersToManageSubscriptions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.TopicsPageSize, model.TopicsPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.PostsPageSize, model.PostsPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumEditor, model.ForumEditor_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.SignaturesEnabled, model.SignaturesEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.AllowPrivateMessages, model.AllowPrivateMessages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ShowAlertForPM, model.ShowAlertForPM_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.NotifyAboutPrivateMessages, model.NotifyAboutPrivateMessages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsFeedEnabled, model.ActiveDiscussionsFeedEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsFeedCount, model.ActiveDiscussionsFeedCount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumFeedsEnabled, model.ForumFeedsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ForumFeedCount, model.ForumFeedCount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.SearchResultsPageSize, model.SearchResultsPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(forumSettings, x => x.ActiveDiscussionsPageSize, model.ActiveDiscussionsPageSize_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Forum");
            }

            //prepare model
            model = await _settingModelFactory.PrepareForumSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> News()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareNewsSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> News(NewsSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var newsSettings = await _settingService.LoadSettingAsync<NewsSettings>(storeScope);
                newsSettings = model.ToSettings(newsSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.AllowNotRegisteredUsersToLeaveComments, model.AllowNotRegisteredUsersToLeaveComments_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NotifyAboutNewNewsComments, model.NotifyAboutNewNewsComments_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.ShowNewsOnMainPage, model.ShowNewsOnMainPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.MainPageNewsCount, model.MainPageNewsCount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NewsArchivePageSize, model.NewsArchivePageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.ShowHeaderRssUrl, model.ShowHeaderRssUrl_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(newsSettings, x => x.NewsCommentsMustBeApproved, model.NewsCommentsMustBeApproved_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingAsync(newsSettings, x => x.ShowNewsCommentsPerStore, clearCache: false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("News");
            }

            //prepare model
            model = await _settingModelFactory.PrepareNewsSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Shipping()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareShippingSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Shipping(ShippingSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var shippingSettings = await _settingService.LoadSettingAsync<ShippingSettings>(storeScope);
                shippingSettings = model.ToSettings(shippingSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.ShipToSameAddress, model.ShipToSameAddress_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.AllowPickupInStore, model.AllowPickupInStore_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.DisplayPickupPointsOnMap, model.DisplayPickupPointsOnMap_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.IgnoreAdditionalShippingChargeForPickupInStore, model.IgnoreAdditionalShippingChargeForPickupInStore_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.GoogleMapsApiKey, model.GoogleMapsApiKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.UseWarehouseLocation, model.UseWarehouseLocation_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.NotifyUserAboutShippingFromMultipleLocations, model.NotifyUserAboutShippingFromMultipleLocations_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.FreeShippingOverXEnabled, model.FreeShippingOverXEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.FreeShippingOverXValue, model.FreeShippingOverXValue_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.FreeShippingOverXIncludingTax, model.FreeShippingOverXIncludingTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.EstimateShippingCartPageEnabled, model.EstimateShippingCartPageEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.EstimateShippingTvChannelPageEnabled, model.EstimateShippingTvChannelPageEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.EstimateShippingCityNameEnabled, model.EstimateShippingCityNameEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.DisplayShipmentEventsToUsers, model.DisplayShipmentEventsToUsers_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.DisplayShipmentEventsToStoreOwner, model.DisplayShipmentEventsToStoreOwner_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.HideShippingTotal, model.HideShippingTotal_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.BypassShippingMethodSelectionIfOnlyOne, model.BypassShippingMethodSelectionIfOnlyOne_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.ConsiderAssociatedTvChannelsDimensions, model.ConsiderAssociatedTvChannelsDimensions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shippingSettings, x => x.ShippingSorting, model.ShippingSorting_OverrideForStore, storeScope, false);

                if (model.ShippingOriginAddress_OverrideForStore || storeScope == 0)
                {
                    //update address
                    var addressId = await _settingService.SettingExistsAsync(shippingSettings, x => x.ShippingOriginAddressId, storeScope) ?
                        shippingSettings.ShippingOriginAddressId : 0;
                    var originAddress = await _addressService.GetAddressByIdAsync(addressId) ??
                        new Address
                        {
                            CreatedOnUtc = DateTime.UtcNow
                        };
                    //update ID manually (in case we're in multi-store configuration mode it'll be set to the shared one)
                    model.ShippingOriginAddress.Id = addressId;
                    originAddress = model.ShippingOriginAddress.ToEntity(originAddress);
                    if (originAddress.Id > 0)
                        await _addressService.UpdateAddressAsync(originAddress);
                    else
                        await _addressService.InsertAddressAsync(originAddress);
                    shippingSettings.ShippingOriginAddressId = originAddress.Id;

                    await _settingService.SaveSettingAsync(shippingSettings, x => x.ShippingOriginAddressId, storeScope, false);
                }
                else if (storeScope > 0)
                    await _settingService.DeleteSettingAsync(shippingSettings, x => x.ShippingOriginAddressId, storeScope);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Shipping");
            }

            //prepare model
            model = await _settingModelFactory.PrepareShippingSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Tax()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareTaxSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Tax(TaxSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var taxSettings = await _settingService.LoadSettingAsync<TaxSettings>(storeScope);
                taxSettings = model.ToSettings(taxSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.PricesIncludeTax, model.PricesIncludeTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.AllowUsersToSelectTaxDisplayType, model.AllowUsersToSelectTaxDisplayType_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.TaxDisplayType, model.TaxDisplayType_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.DisplayTaxSuffix, model.DisplayTaxSuffix_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.DisplayTaxRates, model.DisplayTaxRates_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.HideZeroTax, model.HideZeroTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.HideTaxInOrderSummary, model.HideTaxInOrderSummary_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.ForceTaxExclusionFromOrderSubtotal, model.ForceTaxExclusionFromOrderSubtotal_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.DefaultTaxCategoryId, model.DefaultTaxCategoryId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.TaxBasedOn, model.TaxBasedOn_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.TaxBasedOnPickupPointAddress, model.TaxBasedOnPickupPointAddress_OverrideForStore, storeScope, false);

                if (model.DefaultTaxAddress_OverrideForStore || storeScope == 0)
                {
                    //update address
                    var addressId = await _settingService.SettingExistsAsync(taxSettings, x => x.DefaultTaxAddressId, storeScope) ?
                        taxSettings.DefaultTaxAddressId : 0;
                    var originAddress = await _addressService.GetAddressByIdAsync(addressId) ??
                        new Address
                        {
                            CreatedOnUtc = DateTime.UtcNow
                        };
                    //update ID manually (in case we're in multi-store configuration mode it'll be set to the shared one)
                    model.DefaultTaxAddress.Id = addressId;
                    originAddress = model.DefaultTaxAddress.ToEntity(originAddress);
                    if (originAddress.Id > 0)
                        await _addressService.UpdateAddressAsync(originAddress);
                    else
                        await _addressService.InsertAddressAsync(originAddress);
                    taxSettings.DefaultTaxAddressId = originAddress.Id;

                    await _settingService.SaveSettingAsync(taxSettings, x => x.DefaultTaxAddressId, storeScope, false);
                }
                else if (storeScope > 0)
                    await _settingService.DeleteSettingAsync(taxSettings, x => x.DefaultTaxAddressId, storeScope);

                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.ShippingIsTaxable, model.ShippingIsTaxable_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.ShippingPriceIncludesTax, model.ShippingPriceIncludesTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.ShippingTaxClassId, model.ShippingTaxClassId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIsTaxable, model.PaymentMethodAdditionalFeeIsTaxable_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.PaymentMethodAdditionalFeeIncludesTax, model.PaymentMethodAdditionalFeeIncludesTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.PaymentMethodAdditionalFeeTaxClassId, model.PaymentMethodAdditionalFeeTaxClassId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatEnabled, model.EuVatEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatEnabledForGuests, model.EuVatEnabledForGuests_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatShopCountryId, model.EuVatShopCountryId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatAllowVatExemption, model.EuVatAllowVatExemption_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatUseWebService, model.EuVatUseWebService_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatAssumeValid, model.EuVatAssumeValid_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(taxSettings, x => x.EuVatEmailAdminWhenNewVatSubmitted, model.EuVatEmailAdminWhenNewVatSubmitted_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Tax");
            }

            //prepare model
            model = await _settingModelFactory.PrepareTaxSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Catalog()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareCatalogSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Catalog(CatalogSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeScope);
                catalogSettings = model.ToSettings(catalogSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowViewUnpublishedTvChannelPage, model.AllowViewUnpublishedTvChannelPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayDiscontinuedMessageForUnpublishedTvChannels, model.DisplayDiscontinuedMessageForUnpublishedTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowSkuOnTvChannelDetailsPage, model.ShowSkuOnTvChannelDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowSkuOnCatalogPages, model.ShowSkuOnCatalogPages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowManufacturerPartNumber, model.ShowManufacturerPartNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowGtin, model.ShowGtin_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowFreeShippingNotification, model.ShowFreeShippingNotification_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowShortDescriptionOnCatalogPages, model.ShowShortDescriptionOnCatalogPages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowTvChannelSorting, model.AllowTvChannelSorting_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowTvChannelViewModeChanging, model.AllowTvChannelViewModeChanging_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DefaultViewMode, model.DefaultViewMode_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowTvChannelsFromSubcategories, model.ShowTvChannelsFromSubcategories_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowCategoryTvChannelNumber, model.ShowCategoryTvChannelNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowCategoryTvChannelNumberIncludingSubcategories, model.ShowCategoryTvChannelNumberIncludingSubcategories_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.CategoryBreadcrumbEnabled, model.CategoryBreadcrumbEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowShareButton, model.ShowShareButton_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.PageShareCode, model.PageShareCode_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelReviewsMustBeApproved, model.TvChannelReviewsMustBeApproved_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.OneReviewPerTvChannelFromUser, model.OneReviewPerTvChannelFromUser, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowAnonymousUsersToReviewTvChannel, model.AllowAnonymousUsersToReviewTvChannel_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelReviewPossibleOnlyAfterPurchasing, model.TvChannelReviewPossibleOnlyAfterPurchasing_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NotifyStoreOwnerAboutNewTvChannelReviews, model.NotifyStoreOwnerAboutNewTvChannelReviews_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NotifyUserAboutTvChannelReviewReply, model.NotifyUserAboutTvChannelReviewReply_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.EmailAFriendEnabled, model.EmailAFriendEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowAnonymousUsersToEmailAFriend, model.AllowAnonymousUsersToEmailAFriend_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.RecentlyViewedTvChannelsNumber, model.RecentlyViewedTvChannelsNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.RecentlyViewedTvChannelsEnabled, model.RecentlyViewedTvChannelsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NewTvChannelsEnabled, model.NewTvChannelsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NewTvChannelsPageSize, model.NewTvChannelsPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NewTvChannelsAllowUsersToSelectPageSize, model.NewTvChannelsAllowUsersToSelectPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NewTvChannelsPageSizeOptions, model.NewTvChannelsPageSizeOptions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.CompareTvChannelsEnabled, model.CompareTvChannelsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowBestsellersOnHomepage, model.ShowBestsellersOnHomepage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NumberOfBestsellersOnHomepage, model.NumberOfBestsellersOnHomepage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPageTvChannelsPerPage, model.SearchPageTvChannelsPerPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPageAllowUsersToSelectPageSize, model.SearchPageAllowUsersToSelectPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPagePageSizeOptions, model.SearchPagePageSizeOptions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPagePriceRangeFiltering, model.SearchPagePriceRangeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPagePriceFrom, model.SearchPagePriceFrom_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPagePriceTo, model.SearchPagePriceTo_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.SearchPageManuallyPriceRange, model.SearchPageManuallyPriceRange_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelSearchAutoCompleteEnabled, model.TvChannelSearchAutoCompleteEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelSearchEnabled, model.TvChannelSearchEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelSearchAutoCompleteNumberOfTvChannels, model.TvChannelSearchAutoCompleteNumberOfTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowTvChannelImagesInSearchAutoComplete, model.ShowTvChannelImagesInSearchAutoComplete_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowLinkToAllResultInSearchAutoComplete, model.ShowLinkToAllResultInSearchAutoComplete_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelSearchTermMinimumLength, model.TvChannelSearchTermMinimumLength_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsAlsoPurchasedEnabled, model.TvChannelsAlsoPurchasedEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsAlsoPurchasedNumber, model.TvChannelsAlsoPurchasedNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.NumberOfTvChannelTags, model.NumberOfTvChannelTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagPageSize, model.TvChannelsByTagPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagAllowUsersToSelectPageSize, model.TvChannelsByTagAllowUsersToSelectPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagPageSizeOptions, model.TvChannelsByTagPageSizeOptions_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagPriceRangeFiltering, model.TvChannelsByTagPriceRangeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagPriceFrom, model.TvChannelsByTagPriceFrom_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagPriceTo, model.TvChannelsByTagPriceTo_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelsByTagManuallyPriceRange, model.TvChannelsByTagManuallyPriceRange_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.IncludeShortDescriptionInCompareTvChannels, model.IncludeShortDescriptionInCompareTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.IncludeFullDescriptionInCompareTvChannels, model.IncludeFullDescriptionInCompareTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ManufacturersBlockItemsToDisplay, model.ManufacturersBlockItemsToDisplay_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoFooter, model.DisplayTaxShippingInfoFooter_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoTvChannelDetailsPage, model.DisplayTaxShippingInfoTvChannelDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoTvChannelBoxes, model.DisplayTaxShippingInfoTvChannelBoxes_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoShoppingCart, model.DisplayTaxShippingInfoShoppingCart_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoWishlist, model.DisplayTaxShippingInfoWishlist_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayTaxShippingInfoOrderDetailsPage, model.DisplayTaxShippingInfoOrderDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowTvChannelReviewsPerStore, model.ShowTvChannelReviewsPerStore_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ShowTvChannelReviewsTabOnAccountPage, model.ShowTvChannelReviewsOnAccountPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelReviewsPageSizeOnAccountPage, model.TvChannelReviewsPageSizeOnAccountPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelReviewsSortByCreatedDateAscending, model.TvChannelReviewsSortByCreatedDateAscending_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportTvChannelAttributes, model.ExportImportTvChannelAttributes_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportTvChannelSpecificationAttributes, model.ExportImportTvChannelSpecificationAttributes_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportTvChannelCategoryBreadcrumb, model.ExportImportTvChannelCategoryBreadcrumb_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportCategoriesUsingCategoryName, model.ExportImportCategoriesUsingCategoryName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportAllowDownloadImages, model.ExportImportAllowDownloadImages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportSplitTvChannelsFile, model.ExportImportSplitTvChannelsFile_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.RemoveRequiredTvChannels, model.RemoveRequiredTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportRelatedEntitiesByName, model.ExportImportRelatedEntitiesByName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.ExportImportTvChannelUseLimitedToStores, model.ExportImportTvChannelUseLimitedToStores_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayDatePreOrderAvailability, model.DisplayDatePreOrderAvailability_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.UseAjaxCatalogTvChannelsLoading, model.UseAjaxCatalogTvChannelsLoading_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.EnableManufacturerFiltering, model.EnableManufacturerFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.EnablePriceRangeFiltering, model.EnablePriceRangeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.EnableSpecificationAttributeFiltering, model.EnableSpecificationAttributeFiltering_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayFromPrices, model.DisplayFromPrices_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AttributeValueOutOfStockDisplayType, model.AttributeValueOutOfStockDisplayType_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowUsersToSearchWithManufacturerName, model.AllowUsersToSearchWithManufacturerName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.AllowUsersToSearchWithCategoryName, model.AllowUsersToSearchWithCategoryName_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.DisplayAllPicturesOnCatalogPages, model.DisplayAllPicturesOnCatalogPages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(catalogSettings, x => x.TvChannelUrlStructureTypeId, model.TvChannelUrlStructureTypeId_OverrideForStore, storeScope, false);

                //now settings not overridable per store
                await _settingService.SaveSettingAsync(catalogSettings, x => x.IgnoreDiscounts, 0, false);
                await _settingService.SaveSettingAsync(catalogSettings, x => x.IgnoreFeaturedTvChannels, 0, false);
                await _settingService.SaveSettingAsync(catalogSettings, x => x.IgnoreAcl, 0, false);
                await _settingService.SaveSettingAsync(catalogSettings, x => x.IgnoreStoreLimitations, 0, false);
                await _settingService.SaveSettingAsync(catalogSettings, x => x.CacheTvChannelPrices, 0, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Catalog");
            }

            //prepare model
            model = await _settingModelFactory.PrepareCatalogSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SortOptionsList(SortOptionSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _settingModelFactory.PrepareSortOptionListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SortOptionUpdate(SortOptionModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var catalogSettings = await _settingService.LoadSettingAsync<CatalogSettings>(storeScope);

            catalogSettings.TvChannelSortingEnumDisplayOrder[model.Id] = model.DisplayOrder;
            if (model.IsActive && catalogSettings.TvChannelSortingEnumDisabled.Contains(model.Id))
                catalogSettings.TvChannelSortingEnumDisabled.Remove(model.Id);
            if (!model.IsActive && !catalogSettings.TvChannelSortingEnumDisabled.Contains(model.Id))
                catalogSettings.TvChannelSortingEnumDisabled.Add(model.Id);

            await _settingService.SaveSettingAsync(catalogSettings, x => x.TvChannelSortingEnumDisplayOrder, storeScope, false);
            await _settingService.SaveSettingAsync(catalogSettings, x => x.TvChannelSortingEnumDisabled, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RewardPoints()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareRewardPointsSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> RewardPoints(RewardPointsSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var rewardPointsSettings = await _settingService.LoadSettingAsync<RewardPointsSettings>(storeScope);
                rewardPointsSettings = model.ToSettings(rewardPointsSettings);

                if (model.ActivatePointsImmediately)
                    rewardPointsSettings.ActivationDelay = 0;

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.Enabled, model.Enabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.ExchangeRate, model.ExchangeRate_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.MinimumRewardPointsToUse, model.MinimumRewardPointsToUse_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.MaximumRewardPointsToUsePerOrder, model.MaximumRewardPointsToUsePerOrder_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.MaximumRedeemedRate, model.MaximumRedeemedRate_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.PointsForRegistration, model.PointsForRegistration_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.RegistrationPointsValidity, model.RegistrationPointsValidity_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.PointsForPurchases_Amount, model.PointsForPurchases_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.PointsForPurchases_Points, model.PointsForPurchases_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.MinOrderTotalToAwardPoints, model.MinOrderTotalToAwardPoints_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.PurchasesPointsValidity, model.PurchasesPointsValidity_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.ActivationDelay, model.ActivationDelay_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.ActivationDelayPeriodId, model.ActivationDelay_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.DisplayHowMuchWillBeEarned, model.DisplayHowMuchWillBeEarned_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(rewardPointsSettings, x => x.PageSize, model.PageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingAsync(rewardPointsSettings, x => x.PointsAccumulatedForAllStores, 0, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("RewardPoints");
            }

            //prepare model
            model = await _settingModelFactory.PrepareRewardPointsSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Order()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareOrderSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Order(OrderSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var orderSettings = await _settingService.LoadSettingAsync<OrderSettings>(storeScope);
                orderSettings = model.ToSettings(orderSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.IsReOrderAllowed, model.IsReOrderAllowed_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.MinOrderSubtotalAmount, model.MinOrderSubtotalAmount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.MinOrderSubtotalAmountIncludingTax, model.MinOrderSubtotalAmountIncludingTax_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.MinOrderTotalAmount, model.MinOrderTotalAmount_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AutoUpdateOrderTotalsOnEditingOrder, model.AutoUpdateOrderTotalsOnEditingOrder_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AnonymousCheckoutAllowed, model.AnonymousCheckoutAllowed_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.CheckoutDisabled, model.CheckoutDisabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.TermsOfServiceOnShoppingCartPage, model.TermsOfServiceOnShoppingCartPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.TermsOfServiceOnOrderConfirmPage, model.TermsOfServiceOnOrderConfirmPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.OnePageCheckoutEnabled, model.OnePageCheckoutEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab, model.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.DisableBillingAddressCheckoutStep, model.DisableBillingAddressCheckoutStep_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.DisableOrderCompletedPage, model.DisableOrderCompletedPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.DisplayPickupInStoreOnShippingMethodPage, model.DisplayPickupInStoreOnShippingMethodPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPlacedEmail, model.AttachPdfInvoiceToOrderPlacedEmail_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AttachPdfInvoiceToOrderPaidEmail, model.AttachPdfInvoiceToOrderPaidEmail_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AttachPdfInvoiceToOrderProcessingEmail, model.AttachPdfInvoiceToOrderProcessingEmail_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AttachPdfInvoiceToOrderCompletedEmail, model.AttachPdfInvoiceToOrderCompletedEmail_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.ReturnRequestsEnabled, model.ReturnRequestsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.ReturnRequestsAllowFiles, model.ReturnRequestsAllowFiles_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.ReturnRequestNumberMask, model.ReturnRequestNumberMask_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.NumberOfDaysReturnRequestAvailable, model.NumberOfDaysReturnRequestAvailable_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.CustomOrderNumberMask, model.CustomOrderNumberMask_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.ExportWithTvChannels, model.ExportWithTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.AllowAdminsToBuyCallForPriceTvChannels, model.AllowAdminsToBuyCallForPriceTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.ShowTvChannelThumbnailInOrderDetailsPage, model.ShowTvChannelThumbnailInOrderDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(orderSettings, x => x.DeleteGiftCardUsageHistory, model.DeleteGiftCardUsageHistory_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingAsync(orderSettings, x => x.ActivateGiftCardsAfterCompletingOrder, 0, false);
                await _settingService.SaveSettingAsync(orderSettings, x => x.DeactivateGiftCardsAfterCancellingOrder, 0, false);
                await _settingService.SaveSettingAsync(orderSettings, x => x.DeactivateGiftCardsAfterDeletingOrder, 0, false);
                await _settingService.SaveSettingAsync(orderSettings, x => x.CompleteOrderWhenDelivered, 0, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //order ident
                if (model.OrderIdent.HasValue)
                {
                    try
                    {
                        await _dataProvider.SetTableIdentAsync<Order>(model.OrderIdent.Value);
                    }
                    catch (Exception exc)
                    {
                        _notificationService.ErrorNotification(exc.Message);
                    }
                }

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Order");
            }

            //prepare model
            model = await _settingModelFactory.PrepareOrderSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> ShoppingCart()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareShoppingCartSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ShoppingCart(ShoppingCartSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var shoppingCartSettings = await _settingService.LoadSettingAsync<ShoppingCartSettings>(storeScope);
                shoppingCartSettings = model.ToSettings(shoppingCartSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.DisplayCartAfterAddingTvChannel, model.DisplayCartAfterAddingTvChannel_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.DisplayWishlistAfterAddingTvChannel, model.DisplayWishlistAfterAddingTvChannel_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.MaximumShoppingCartItems, model.MaximumShoppingCartItems_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.MaximumWishlistItems, model.MaximumWishlistItems_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.AllowOutOfStockItemsToBeAddedToWishlist, model.AllowOutOfStockItemsToBeAddedToWishlist_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.MoveItemsFromWishlistToCart, model.MoveItemsFromWishlistToCart_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.CartsSharedBetweenStores, model.CartsSharedBetweenStores_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.ShowTvChannelImagesOnShoppingCart, model.ShowTvChannelImagesOnShoppingCart_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.ShowTvChannelImagesOnWishList, model.ShowTvChannelImagesOnWishList_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.ShowDiscountBox, model.ShowDiscountBox_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.ShowGiftCardBox, model.ShowGiftCardBox_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.CrossSellsNumber, model.CrossSellsNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.EmailWishlistEnabled, model.EmailWishlistEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.AllowAnonymousUsersToEmailWishlist, model.AllowAnonymousUsersToEmailWishlist_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.MiniShoppingCartEnabled, model.MiniShoppingCartEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.ShowTvChannelImagesInMiniShoppingCart, model.ShowTvChannelImagesInMiniShoppingCart_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.MiniShoppingCartTvChannelNumber, model.MiniShoppingCartTvChannelNumber_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.AllowCartItemEditing, model.AllowCartItemEditing_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(shoppingCartSettings, x => x.GroupTierPricesForDistinctShoppingCartItems, model.GroupTierPricesForDistinctShoppingCartItems_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("ShoppingCart");
            }

            //prepare model
            model = await _settingModelFactory.PrepareShoppingCartSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Media()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareMediaSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> Media(MediaSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var mediaSettings = await _settingService.LoadSettingAsync<MediaSettings>(storeScope);
                mediaSettings = model.ToSettings(mediaSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AvatarPictureSize, model.AvatarPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.TvChannelThumbPictureSize, model.TvChannelThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.TvChannelDetailsPictureSize, model.TvChannelDetailsPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.TvChannelThumbPictureSizeOnTvChannelDetailsPage, model.TvChannelThumbPictureSizeOnTvChannelDetailsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AssociatedTvChannelPictureSize, model.AssociatedTvChannelPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.CategoryThumbPictureSize, model.CategoryThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ManufacturerThumbPictureSize, model.ManufacturerThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.VendorThumbPictureSize, model.VendorThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.CartThumbPictureSize, model.CartThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.OrderThumbPictureSize, model.OrderThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MiniCartThumbPictureSize, model.MiniCartThumbPictureSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MaximumImageSize, model.MaximumImageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.MultipleThumbDirectories, model.MultipleThumbDirectories_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.DefaultImageQuality, model.DefaultImageQuality_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.ImportTvChannelImagesUsingHash, model.ImportTvChannelImagesUsingHash_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.DefaultPictureZoomEnabled, model.DefaultPictureZoomEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.AllowSVGUploads, model.AllowSVGUploads_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(mediaSettings, x => x.TvChannelDefaultImageId, model.TvChannelDefaultImageId_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Media");
            }

            //prepare model
            model = await _settingModelFactory.PrepareMediaSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("Media")]
        [FormValueRequired("change-picture-storage")]
        public virtual async Task<IActionResult> ChangePictureStorage()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            await _pictureService.SetIsStoreInDbAsync(!await _pictureService.IsStoreInDbAsync());

            //activity log
            await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

            return RedirectToAction("Media");
        }

        public virtual async Task<IActionResult> UserUser()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareUserUserSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> UserUser(UserUserSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var userSettings = await _settingService.LoadSettingAsync<UserSettings>(storeScope);

                var lastUsernameValidationRule = userSettings.UsernameValidationRule;
                var lastUsernameValidationEnabledValue = userSettings.UsernameValidationEnabled;
                var lastUsernameValidationUseRegexValue = userSettings.UsernameValidationUseRegex;

                //SmartPhone number validation settings
                var lastPhoneNumberValidationRule = userSettings.PhoneNumberValidationRule;
                var lastPhoneNumberValidationEnabledValue = userSettings.PhoneNumberValidationEnabled;
                var lastPhoneNumberValidationUseRegexValue = userSettings.PhoneNumberValidationUseRegex;

                var addressSettings = await _settingService.LoadSettingAsync<AddressSettings>(storeScope);
                var dateTimeSettings = await _settingService.LoadSettingAsync<DateTimeSettings>(storeScope);
                var externalAuthenticationSettings = await _settingService.LoadSettingAsync<ExternalAuthenticationSettings>(storeScope);
                var multiFactorAuthenticationSettings = await _settingService.LoadSettingAsync<MultiFactorAuthenticationSettings>(storeScope);

                userSettings = model.UserSettings.ToSettings(userSettings);

                if (userSettings.UsernameValidationEnabled && userSettings.UsernameValidationUseRegex)
                {
                    try
                    {
                        //validate regex rule
                        var unused = Regex.IsMatch("test_user_name", userSettings.UsernameValidationRule);
                    }
                    catch (ArgumentException)
                    {
                        //restoring previous settings
                        userSettings.UsernameValidationRule = lastUsernameValidationRule;
                        userSettings.UsernameValidationEnabled = lastUsernameValidationEnabledValue;
                        userSettings.UsernameValidationUseRegex = lastUsernameValidationUseRegexValue;

                        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.UserSettings.RegexValidationRule.Error"));
                    }
                }

                if (userSettings.PhoneNumberValidationEnabled && userSettings.PhoneNumberValidationUseRegex)
                {
                    try
                    {
                        //validate regex rule
                        var unused = Regex.IsMatch("123456789", userSettings.PhoneNumberValidationRule);
                    }
                    catch (ArgumentException)
                    {
                        //restoring previous settings
                        userSettings.PhoneNumberValidationRule = lastPhoneNumberValidationRule;
                        userSettings.PhoneNumberValidationEnabled = lastPhoneNumberValidationEnabledValue;
                        userSettings.PhoneNumberValidationUseRegex = lastPhoneNumberValidationUseRegexValue;

                        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.UserSettings.PhoneNumberRegexValidationRule.Error"));
                    }
                }

                await _settingService.SaveSettingAsync(userSettings);

                addressSettings = model.AddressSettings.ToSettings(addressSettings);
                await _settingService.SaveSettingAsync(addressSettings);

                dateTimeSettings.DefaultStoreGmtZone = model.DateTimeSettings.DefaultStoreGmtZone;
                dateTimeSettings.AllowUsersToSetTimeZone = model.DateTimeSettings.AllowUsersToSetTimeZone;
                await _settingService.SaveSettingAsync(dateTimeSettings);

                externalAuthenticationSettings.AllowUsersToRemoveAssociations = model.ExternalAuthenticationSettings.AllowUsersToRemoveAssociations;
                await _settingService.SaveSettingAsync(externalAuthenticationSettings);

                multiFactorAuthenticationSettings = model.MultiFactorAuthenticationSettings.ToSettings(multiFactorAuthenticationSettings);
                await _settingService.SaveSettingAsync(multiFactorAuthenticationSettings);

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("UserUser");
            }

            //prepare model
            model = await _settingModelFactory.PrepareUserUserSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        #region GDPR

        public virtual async Task<IActionResult> Gdpr()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareGdprSettingsModelAsync();

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Gdpr(GdprSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var gdprSettings = await _settingService.LoadSettingAsync<GdprSettings>(storeScope);
                gdprSettings = model.ToSettings(gdprSettings);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(gdprSettings, x => x.GdprEnabled, model.GdprEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(gdprSettings, x => x.LogPrivacyPolicyConsent, model.LogPrivacyPolicyConsent_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(gdprSettings, x => x.LogNewsletterConsent, model.LogNewsletterConsent_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(gdprSettings, x => x.LogUserProfileChanges, model.LogUserProfileChanges_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(gdprSettings, x => x.DeleteInactiveUsersAfterMonths, model.DeleteInactiveUsersAfterMonths_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("Gdpr");
            }

            //prepare model
            model = await _settingModelFactory.PrepareGdprSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> GdprConsentList(GdprConsentSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _settingModelFactory.PrepareGdprConsentListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> CreateGdprConsent()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareGdprConsentModelAsync(new GdprConsentModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> CreateGdprConsent(GdprConsentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var gdprConsent = model.ToEntity<GdprConsent>();
                await _gdprService.InsertConsentAsync(gdprConsent);

                //locales                
                await UpdateGdprConsentLocalesAsync(gdprConsent, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Added"));

                return continueEditing ? RedirectToAction("EditGdprConsent", new { gdprConsent.Id }) : RedirectToAction("Gdpr");
            }

            //prepare model
            model = await _settingModelFactory.PrepareGdprConsentModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> EditGdprConsent(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a consent with the specified id
            var gdprConsent = await _gdprService.GetConsentByIdAsync(id);
            if (gdprConsent == null)
                return RedirectToAction("Gdpr");

            //prepare model
            var model = await _settingModelFactory.PrepareGdprConsentModelAsync(null, gdprConsent);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> EditGdprConsent(GdprConsentModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a GDPR consent with the specified id
            var gdprConsent = await _gdprService.GetConsentByIdAsync(model.Id);
            if (gdprConsent == null)
                return RedirectToAction("Gdpr");

            if (ModelState.IsValid)
            {
                gdprConsent = model.ToEntity(gdprConsent);
                await _gdprService.UpdateConsentAsync(gdprConsent);

                //locales                
                await UpdateGdprConsentLocalesAsync(gdprConsent, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Updated"));

                return continueEditing ? RedirectToAction("EditGdprConsent", gdprConsent.Id) : RedirectToAction("Gdpr");
            }

            //prepare model
            model = await _settingModelFactory.PrepareGdprConsentModelAsync(model, gdprConsent, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteGdprConsent(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a GDPR consent with the specified id
            var gdprConsent = await _gdprService.GetConsentByIdAsync(id);
            if (gdprConsent == null)
                return RedirectToAction("Gdpr");

            await _gdprService.DeleteConsentAsync(gdprConsent);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.Gdpr.Consent.Deleted"));

            return RedirectToAction("Gdpr");
        }

        #endregion

        public virtual async Task<IActionResult> GeneralCommon(bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync();

            //show configuration tour
            if (showtour)
            {
                var user = await _workContext.GetCurrentUserAsync();
                var hideCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.HideConfigurationStepsAttribute);
                var closeCard = await _genericAttributeService.GetAttributeAsync<bool>(user, TvProgUserDefaults.CloseConfigurationStepsAttribute);

                if (!hideCard && !closeCard)
                    ViewBag.ShowTour = true;
            }

            return View(model);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public virtual async Task<IActionResult> GeneralCommon(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();

                //store information settings
                var storeInformationSettings = await _settingService.LoadSettingAsync<StoreInformationSettings>(storeScope);
                var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeScope);
                var sitemapSettings = await _settingService.LoadSettingAsync<SitemapSettings>(storeScope);

                storeInformationSettings.StoreClosed = model.StoreInformationSettings.StoreClosed;
                storeInformationSettings.DefaultStoreTheme = model.StoreInformationSettings.DefaultStoreTheme;
                storeInformationSettings.AllowUserToSelectTheme = model.StoreInformationSettings.AllowUserToSelectTheme;
                storeInformationSettings.LogoPictureId = model.StoreInformationSettings.LogoPictureId;
                //EU Cookie law
                storeInformationSettings.DisplayEuCookieLawWarning = model.StoreInformationSettings.DisplayEuCookieLawWarning;
                //social pages
                storeInformationSettings.FacebookLink = model.StoreInformationSettings.FacebookLink;
                storeInformationSettings.TwitterLink = model.StoreInformationSettings.TwitterLink;
                storeInformationSettings.YoutubeLink = model.StoreInformationSettings.YoutubeLink;
                storeInformationSettings.InstagramLink = model.StoreInformationSettings.InstagramLink;
                //contact us
                commonSettings.SubjectFieldOnContactUsForm = model.StoreInformationSettings.SubjectFieldOnContactUsForm;
                commonSettings.UseSystemEmailForContactUsForm = model.StoreInformationSettings.UseSystemEmailForContactUsForm;
                //terms of service
                commonSettings.PopupForTermsOfServiceLinks = model.StoreInformationSettings.PopupForTermsOfServiceLinks;
                //sitemap
                sitemapSettings.SitemapEnabled = model.SitemapSettings.SitemapEnabled;
                sitemapSettings.SitemapPageSize = model.SitemapSettings.SitemapPageSize;
                sitemapSettings.SitemapIncludeCategories = model.SitemapSettings.SitemapIncludeCategories;
                sitemapSettings.SitemapIncludeManufacturers = model.SitemapSettings.SitemapIncludeManufacturers;
                sitemapSettings.SitemapIncludeTvChannels = model.SitemapSettings.SitemapIncludeTvChannels;
                sitemapSettings.SitemapIncludeTvChannelTags = model.SitemapSettings.SitemapIncludeTvChannelTags;
                sitemapSettings.SitemapIncludeBlogPosts = model.SitemapSettings.SitemapIncludeBlogPosts;
                sitemapSettings.SitemapIncludeNews = model.SitemapSettings.SitemapIncludeNews;
                sitemapSettings.SitemapIncludeTopics = model.SitemapSettings.SitemapIncludeTopics;

                //minification
                commonSettings.EnableHtmlMinification = model.MinificationSettings.EnableHtmlMinification;
                //use response compression
                commonSettings.UseResponseCompression = model.MinificationSettings.UseResponseCompression;
                //custom header and footer HTML
                commonSettings.HeaderCustomHtml = model.CustomHtmlSettings.HeaderCustomHtml;
                commonSettings.FooterCustomHtml = model.CustomHtmlSettings.FooterCustomHtml;

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.StoreClosed, model.StoreInformationSettings.StoreClosed_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.DefaultStoreTheme, model.StoreInformationSettings.DefaultStoreTheme_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.AllowUserToSelectTheme, model.StoreInformationSettings.AllowUserToSelectTheme_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.LogoPictureId, model.StoreInformationSettings.LogoPictureId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.DisplayEuCookieLawWarning, model.StoreInformationSettings.DisplayEuCookieLawWarning_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.FacebookLink, model.StoreInformationSettings.FacebookLink_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.TwitterLink, model.StoreInformationSettings.TwitterLink_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.YoutubeLink, model.StoreInformationSettings.YoutubeLink_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(storeInformationSettings, x => x.InstagramLink, model.StoreInformationSettings.InstagramLink_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.SubjectFieldOnContactUsForm, model.StoreInformationSettings.SubjectFieldOnContactUsForm_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.UseSystemEmailForContactUsForm, model.StoreInformationSettings.UseSystemEmailForContactUsForm_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.PopupForTermsOfServiceLinks, model.StoreInformationSettings.PopupForTermsOfServiceLinks_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapEnabled, model.SitemapSettings.SitemapEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapPageSize, model.SitemapSettings.SitemapPageSize_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeCategories, model.SitemapSettings.SitemapIncludeCategories_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeManufacturers, model.SitemapSettings.SitemapIncludeManufacturers_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeTvChannels, model.SitemapSettings.SitemapIncludeTvChannels_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeTvChannelTags, model.SitemapSettings.SitemapIncludeTvChannelTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeBlogPosts, model.SitemapSettings.SitemapIncludeBlogPosts_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeNews, model.SitemapSettings.SitemapIncludeNews_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(sitemapSettings, x => x.SitemapIncludeTopics, model.SitemapSettings.SitemapIncludeTopics_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.EnableHtmlMinification, model.MinificationSettings.EnableHtmlMinification_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.UseResponseCompression, model.MinificationSettings.UseResponseCompression_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.HeaderCustomHtml, model.CustomHtmlSettings.HeaderCustomHtml_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.FooterCustomHtml, model.CustomHtmlSettings.FooterCustomHtml_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //seo settings
                var seoSettings = await _settingService.LoadSettingAsync<SeoSettings>(storeScope);
                seoSettings.PageTitleSeparator = model.SeoSettings.PageTitleSeparator;
                seoSettings.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)model.SeoSettings.PageTitleSeoAdjustment;
                seoSettings.GenerateTvChannelMetaDescription = model.SeoSettings.GenerateTvChannelMetaDescription;
                seoSettings.ConvertNonWesternChars = model.SeoSettings.ConvertNonWesternChars;
                seoSettings.CanonicalUrlsEnabled = model.SeoSettings.CanonicalUrlsEnabled;
                seoSettings.WwwRequirement = (WwwRequirement)model.SeoSettings.WwwRequirement;
                seoSettings.TwitterMetaTags = model.SeoSettings.TwitterMetaTags;
                seoSettings.OpenGraphMetaTags = model.SeoSettings.OpenGraphMetaTags;
                seoSettings.MicrodataEnabled = model.SeoSettings.MicrodataEnabled;
                seoSettings.CustomHeadTags = model.SeoSettings.CustomHeadTags;

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.PageTitleSeparator, model.SeoSettings.PageTitleSeparator_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.PageTitleSeoAdjustment, model.SeoSettings.PageTitleSeoAdjustment_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.GenerateTvChannelMetaDescription, model.SeoSettings.GenerateTvChannelMetaDescription_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.ConvertNonWesternChars, model.SeoSettings.ConvertNonWesternChars_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.CanonicalUrlsEnabled, model.SeoSettings.CanonicalUrlsEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.WwwRequirement, model.SeoSettings.WwwRequirement_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.TwitterMetaTags, model.SeoSettings.TwitterMetaTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.OpenGraphMetaTags, model.SeoSettings.OpenGraphMetaTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.CustomHeadTags, model.SeoSettings.CustomHeadTags_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(seoSettings, x => x.MicrodataEnabled, model.SeoSettings.MicrodataEnabled_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //security settings
                var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeScope);
                if (securitySettings.AdminAreaAllowedIpAddresses == null)
                    securitySettings.AdminAreaAllowedIpAddresses = new List<string>();
                securitySettings.AdminAreaAllowedIpAddresses.Clear();
                if (!string.IsNullOrEmpty(model.SecuritySettings.AdminAreaAllowedIpAddresses))
                    foreach (var s in model.SecuritySettings.AdminAreaAllowedIpAddresses.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                        if (!string.IsNullOrWhiteSpace(s))
                            securitySettings.AdminAreaAllowedIpAddresses.Add(s.Trim());
                securitySettings.HoneypotEnabled = model.SecuritySettings.HoneypotEnabled;
                await _settingService.SaveSettingAsync(securitySettings);

                //robots.txt settings
                var robotsTxtSettings = await _settingService.LoadSettingAsync<RobotsTxtSettings>(storeScope);
                robotsTxtSettings.AllowSitemapXml = model.RobotsTxtSettings.AllowSitemapXml;
                robotsTxtSettings.AdditionsRules = model.RobotsTxtSettings.AdditionsRules?.Split(Environment.NewLine).ToList();
                robotsTxtSettings.DisallowLanguages = model.RobotsTxtSettings.DisallowLanguages?.ToList() ?? new List<int>(); 
                robotsTxtSettings.DisallowPaths = model.RobotsTxtSettings.DisallowPaths?.Split(Environment.NewLine).ToList();
                robotsTxtSettings.LocalizableDisallowPaths = model.RobotsTxtSettings.LocalizableDisallowPaths?.Split(Environment.NewLine).ToList();

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.AllowSitemapXml, model.RobotsTxtSettings.AllowSitemapXml_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.AdditionsRules, model.RobotsTxtSettings.AdditionsRules_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.DisallowLanguages, model.RobotsTxtSettings.DisallowLanguages_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.DisallowPaths, model.RobotsTxtSettings.DisallowPaths_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(robotsTxtSettings, x => x.LocalizableDisallowPaths, model.RobotsTxtSettings.LocalizableDisallowPaths_OverrideForStore, storeScope, false);

                // now clear settings cache
                await _settingService.ClearCacheAsync();

                //captcha settings
                var captchaSettings = await _settingService.LoadSettingAsync<CaptchaSettings>(storeScope);
                captchaSettings.Enabled = model.CaptchaSettings.Enabled;
                captchaSettings.ShowOnLoginPage = model.CaptchaSettings.ShowOnLoginPage;
                captchaSettings.ShowOnRegistrationPage = model.CaptchaSettings.ShowOnRegistrationPage;
                captchaSettings.ShowOnContactUsPage = model.CaptchaSettings.ShowOnContactUsPage;
                captchaSettings.ShowOnEmailWishlistToFriendPage = model.CaptchaSettings.ShowOnEmailWishlistToFriendPage;
                captchaSettings.ShowOnEmailTvChannelToFriendPage = model.CaptchaSettings.ShowOnEmailTvChannelToFriendPage;
                captchaSettings.ShowOnBlogCommentPage = model.CaptchaSettings.ShowOnBlogCommentPage;
                captchaSettings.ShowOnNewsCommentPage = model.CaptchaSettings.ShowOnNewsCommentPage;
                captchaSettings.ShowOnTvChannelReviewPage = model.CaptchaSettings.ShowOnTvChannelReviewPage;
                captchaSettings.ShowOnForgotPasswordPage = model.CaptchaSettings.ShowOnForgotPasswordPage;
                captchaSettings.ShowOnApplyVendorPage = model.CaptchaSettings.ShowOnApplyVendorPage;
                captchaSettings.ShowOnForum = model.CaptchaSettings.ShowOnForum;
                captchaSettings.ShowOnCheckoutPageForGuests = model.CaptchaSettings.ShowOnCheckoutPageForGuests;
                captchaSettings.ReCaptchaPublicKey = model.CaptchaSettings.ReCaptchaPublicKey;
                captchaSettings.ReCaptchaPrivateKey = model.CaptchaSettings.ReCaptchaPrivateKey;
                captchaSettings.CaptchaType = (CaptchaType)model.CaptchaSettings.CaptchaType;
                captchaSettings.ReCaptchaV3ScoreThreshold = model.CaptchaSettings.ReCaptchaV3ScoreThreshold;

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.Enabled, model.CaptchaSettings.Enabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnLoginPage, model.CaptchaSettings.ShowOnLoginPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnRegistrationPage, model.CaptchaSettings.ShowOnRegistrationPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnContactUsPage, model.CaptchaSettings.ShowOnContactUsPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnEmailWishlistToFriendPage, model.CaptchaSettings.ShowOnEmailWishlistToFriendPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnEmailTvChannelToFriendPage, model.CaptchaSettings.ShowOnEmailTvChannelToFriendPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnBlogCommentPage, model.CaptchaSettings.ShowOnBlogCommentPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnNewsCommentPage, model.CaptchaSettings.ShowOnNewsCommentPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnTvChannelReviewPage, model.CaptchaSettings.ShowOnTvChannelReviewPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnApplyVendorPage, model.CaptchaSettings.ShowOnApplyVendorPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnForgotPasswordPage, model.CaptchaSettings.ShowOnForgotPasswordPage_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnForum, model.CaptchaSettings.ShowOnForum_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ShowOnCheckoutPageForGuests, model.CaptchaSettings.ShowOnCheckoutPageForGuests_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaPublicKey, model.CaptchaSettings.ReCaptchaPublicKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaPrivateKey, model.CaptchaSettings.ReCaptchaPrivateKey_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.ReCaptchaV3ScoreThreshold, model.CaptchaSettings.ReCaptchaV3ScoreThreshold_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(captchaSettings, x => x.CaptchaType, model.CaptchaSettings.CaptchaType_OverrideForStore, storeScope, false);

                // now clear settings cache
                await _settingService.ClearCacheAsync();

                if (captchaSettings.Enabled &&
                    (string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPublicKey) || string.IsNullOrWhiteSpace(captchaSettings.ReCaptchaPrivateKey)))
                {
                    //captcha is enabled but the keys are not entered
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.CaptchaAppropriateKeysNotEnteredError"));
                }

                //PDF settings
                var pdfSettings = await _settingService.LoadSettingAsync<PdfSettings>(storeScope);
                pdfSettings.LetterPageSizeEnabled = model.PdfSettings.LetterPageSizeEnabled;
                pdfSettings.LogoPictureId = model.PdfSettings.LogoPictureId;
                pdfSettings.DisablePdfInvoicesForPendingOrders = model.PdfSettings.DisablePdfInvoicesForPendingOrders;
                pdfSettings.InvoiceFooterTextColumn1 = model.PdfSettings.InvoiceFooterTextColumn1;
                pdfSettings.InvoiceFooterTextColumn2 = model.PdfSettings.InvoiceFooterTextColumn2;

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.LetterPageSizeEnabled, model.PdfSettings.LetterPageSizeEnabled_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.LogoPictureId, model.PdfSettings.LogoPictureId_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.DisablePdfInvoicesForPendingOrders, model.PdfSettings.DisablePdfInvoicesForPendingOrders_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.InvoiceFooterTextColumn1, model.PdfSettings.InvoiceFooterTextColumn1_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(pdfSettings, x => x.InvoiceFooterTextColumn2, model.PdfSettings.InvoiceFooterTextColumn2_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //localization settings
                var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeScope);
                localizationSettings.UseImagesForLanguageSelection = model.LocalizationSettings.UseImagesForLanguageSelection;
                if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled != model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    localizationSettings.SeoFriendlyUrlsForLanguagesEnabled = model.LocalizationSettings.SeoFriendlyUrlsForLanguagesEnabled;
                }

                localizationSettings.AutomaticallyDetectLanguage = model.LocalizationSettings.AutomaticallyDetectLanguage;
                localizationSettings.LoadAllLocaleRecordsOnStartup = model.LocalizationSettings.LoadAllLocaleRecordsOnStartup;
                localizationSettings.LoadAllLocalizedPropertiesOnStartup = model.LocalizationSettings.LoadAllLocalizedPropertiesOnStartup;
                localizationSettings.LoadAllUrlRecordsOnStartup = model.LocalizationSettings.LoadAllUrlRecordsOnStartup;
                await _settingService.SaveSettingAsync(localizationSettings);

                //display default menu item
                var displayDefaultMenuItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultMenuItemSettings>(storeScope);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                displayDefaultMenuItemSettings.DisplayHomepageMenuItem = model.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem;
                displayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem;
                displayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem = model.DisplayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem;
                displayDefaultMenuItemSettings.DisplayUserInfoMenuItem = model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem;
                displayDefaultMenuItemSettings.DisplayBlogMenuItem = model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem;
                displayDefaultMenuItemSettings.DisplayForumsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem;
                displayDefaultMenuItemSettings.DisplayContactUsMenuItem = model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem;

                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayHomepageMenuItem, model.DisplayDefaultMenuItemSettings.DisplayHomepageMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayNewTvChannelsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayNewTvChannelsMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayTvChannelSearchMenuItem, model.DisplayDefaultMenuItemSettings.DisplayTvChannelSearchMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayUserInfoMenuItem, model.DisplayDefaultMenuItemSettings.DisplayUserInfoMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayBlogMenuItem, model.DisplayDefaultMenuItemSettings.DisplayBlogMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayForumsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayForumsMenuItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultMenuItemSettings, x => x.DisplayContactUsMenuItem, model.DisplayDefaultMenuItemSettings.DisplayContactUsMenuItem_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //display default footer item
                var displayDefaultFooterItemSettings = await _settingService.LoadSettingAsync<DisplayDefaultFooterItemSettings>(storeScope);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                displayDefaultFooterItemSettings.DisplaySitemapFooterItem = model.DisplayDefaultFooterItemSettings.DisplaySitemapFooterItem;
                displayDefaultFooterItemSettings.DisplayContactUsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayContactUsFooterItem;
                displayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem = model.DisplayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem;
                displayDefaultFooterItemSettings.DisplayNewsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayNewsFooterItem;
                displayDefaultFooterItemSettings.DisplayBlogFooterItem = model.DisplayDefaultFooterItemSettings.DisplayBlogFooterItem;
                displayDefaultFooterItemSettings.DisplayForumsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayForumsFooterItem;
                displayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem;
                displayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem;
                displayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem = model.DisplayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem;
                displayDefaultFooterItemSettings.DisplayUserInfoFooterItem = model.DisplayDefaultFooterItemSettings.DisplayUserInfoFooterItem;
                displayDefaultFooterItemSettings.DisplayUserOrdersFooterItem = model.DisplayDefaultFooterItemSettings.DisplayUserOrdersFooterItem;
                displayDefaultFooterItemSettings.DisplayUserAddressesFooterItem = model.DisplayDefaultFooterItemSettings.DisplayUserAddressesFooterItem;
                displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem = model.DisplayDefaultFooterItemSettings.DisplayShoppingCartFooterItem;
                displayDefaultFooterItemSettings.DisplayWishlistFooterItem = model.DisplayDefaultFooterItemSettings.DisplayWishlistFooterItem;
                displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem = model.DisplayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem;

                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplaySitemapFooterItem, model.DisplayDefaultFooterItemSettings.DisplaySitemapFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayContactUsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayContactUsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayTvChannelSearchFooterItem, model.DisplayDefaultFooterItemSettings.DisplayTvChannelSearchFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayNewsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayNewsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayBlogFooterItem, model.DisplayDefaultFooterItemSettings.DisplayBlogFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayForumsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayForumsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayRecentlyViewedTvChannelsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayRecentlyViewedTvChannelsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayCompareTvChannelsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayCompareTvChannelsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayNewTvChannelsFooterItem, model.DisplayDefaultFooterItemSettings.DisplayNewTvChannelsFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayUserInfoFooterItem, model.DisplayDefaultFooterItemSettings.DisplayUserInfoFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayUserOrdersFooterItem, model.DisplayDefaultFooterItemSettings.DisplayUserOrdersFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayUserAddressesFooterItem, model.DisplayDefaultFooterItemSettings.DisplayUserAddressesFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayShoppingCartFooterItem, model.DisplayDefaultFooterItemSettings.DisplayShoppingCartFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayWishlistFooterItem, model.DisplayDefaultFooterItemSettings.DisplayWishlistFooterItem_OverrideForStore, storeScope, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(displayDefaultFooterItemSettings, x => x.DisplayApplyVendorAccountFooterItem, model.DisplayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //admin area
                var adminAreaSettings = await _settingService.LoadSettingAsync<AdminAreaSettings>(storeScope);

                //we do not clear cache after each setting update.
                //this behavior can increase performance because cached settings will not be cleared 
                //and loaded from database after each update
                adminAreaSettings.UseRichEditorInMessageTemplates = model.AdminAreaSettings.UseRichEditorInMessageTemplates;

                await _settingService.SaveSettingOverridablePerStoreAsync(adminAreaSettings, x => x.UseRichEditorInMessageTemplates, model.AdminAreaSettings.UseRichEditorInMessageTemplates_OverrideForStore, storeScope, false);

                //now clear settings cache
                await _settingService.ClearCacheAsync();

                //activity log
                await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"));

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Updated"));

                return RedirectToAction("GeneralCommon");
            }

            //prepare model
            model = await _settingModelFactory.PrepareGeneralCommonSettingsModelAsync(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost, ActionName("GeneralCommon")]
        [FormValueRequired("changeencryptionkey")]
        public virtual async Task<IActionResult> ChangeEncryptionKey(GeneralCommonSettingsModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var securitySettings = await _settingService.LoadSettingAsync<SecuritySettings>(storeScope);

            try
            {
                if (model.SecuritySettings.EncryptionKey == null)
                    model.SecuritySettings.EncryptionKey = string.Empty;

                model.SecuritySettings.EncryptionKey = model.SecuritySettings.EncryptionKey.Trim();

                var newEncryptionPrivateKey = model.SecuritySettings.EncryptionKey;
                if (string.IsNullOrEmpty(newEncryptionPrivateKey) || newEncryptionPrivateKey.Length != 16)
                    throw new TvProgException(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TooShort"));

                var oldEncryptionPrivateKey = securitySettings.EncryptionKey;
                if (oldEncryptionPrivateKey == newEncryptionPrivateKey)
                    throw new TvProgException(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.TheSame"));

                //update encrypted order info
                var orders = await _orderService.SearchOrdersAsync();
                foreach (var order in orders)
                {
                    var decryptedCardType = _encryptionService.DecryptText(order.CardType, oldEncryptionPrivateKey);
                    var decryptedCardName = _encryptionService.DecryptText(order.CardName, oldEncryptionPrivateKey);
                    var decryptedCardNumber = _encryptionService.DecryptText(order.CardNumber, oldEncryptionPrivateKey);
                    var decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(order.MaskedCreditCardNumber, oldEncryptionPrivateKey);
                    var decryptedCardCvv2 = _encryptionService.DecryptText(order.CardCvv2, oldEncryptionPrivateKey);
                    var decryptedCardExpirationMonth = _encryptionService.DecryptText(order.CardExpirationMonth, oldEncryptionPrivateKey);
                    var decryptedCardExpirationYear = _encryptionService.DecryptText(order.CardExpirationYear, oldEncryptionPrivateKey);

                    var encryptedCardType = _encryptionService.EncryptText(decryptedCardType, newEncryptionPrivateKey);
                    var encryptedCardName = _encryptionService.EncryptText(decryptedCardName, newEncryptionPrivateKey);
                    var encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, newEncryptionPrivateKey);
                    var encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, newEncryptionPrivateKey);
                    var encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, newEncryptionPrivateKey);
                    var encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, newEncryptionPrivateKey);
                    var encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, newEncryptionPrivateKey);

                    order.CardType = encryptedCardType;
                    order.CardName = encryptedCardName;
                    order.CardNumber = encryptedCardNumber;
                    order.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
                    order.CardCvv2 = encryptedCardCvv2;
                    order.CardExpirationMonth = encryptedCardExpirationMonth;
                    order.CardExpirationYear = encryptedCardExpirationYear;
                    await _orderService.UpdateOrderAsync(order);
                }

                //update password information
                //optimization - load only passwords with PasswordFormat.Encrypted
                var userPasswords = await _userService.GetUserPasswordsAsync(passwordFormat: PasswordFormat.Encrypted);
                foreach (var userPassword in userPasswords)
                {
                    var decryptedPassword = _encryptionService.DecryptText(userPassword.Password, oldEncryptionPrivateKey);
                    var encryptedPassword = _encryptionService.EncryptText(decryptedPassword, newEncryptionPrivateKey);

                    userPassword.Password = encryptedPassword;
                    await _userService.UpdateUserPasswordAsync(userPassword);
                }

                securitySettings.EncryptionKey = newEncryptionPrivateKey;
                await _settingService.SaveSettingAsync(securitySettings);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.EncryptionKey.Changed"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("GeneralCommon");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UploadLocalePattern()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            try
            {
                await _uploadService.UploadLocalePatternAsync();
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.LocalePattern.SuccessUpload"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("GeneralCommon");
        }

        [HttpPost]
        public virtual async Task<IActionResult> UploadIcons(IFormFile iconsFile)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            try
            {
                if (iconsFile == null || iconsFile.Length == 0)
                {
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Admin.Common.UploadFile"));
                    return RedirectToAction("GeneralCommon");
                }

                //load settings for a chosen store scope
                var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
                var commonSettings = await _settingService.LoadSettingAsync<CommonSettings>(storeScope);

                switch (_fileProvider.GetFileExtension(iconsFile.FileName))
                {
                    case ".ico":
                        await _uploadService.UploadFaviconAsync(iconsFile);
                        commonSettings.FaviconAndAppIconsHeadCode = string.Format(TvProgCommonDefaults.SingleFaviconHeadLink, storeScope, iconsFile.FileName);

                        break;

                    case ".zip":
                        await _uploadService.UploadIconsArchiveAsync(iconsFile);

                        var headCodePath = _fileProvider.GetAbsolutePath(string.Format(TvProgCommonDefaults.FaviconAndAppIconsPath, storeScope), TvProgCommonDefaults.HeadCodeFileName);
                        if (!_fileProvider.FileExists(headCodePath))
                            throw new Exception(string.Format(await _localizationService.GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.FaviconAndAppIcons.MissingFile"), TvProgCommonDefaults.HeadCodeFileName));

                        using (var sr = new StreamReader(headCodePath))
                            commonSettings.FaviconAndAppIconsHeadCode = await sr.ReadToEndAsync();

                        break;

                    default:
                        throw new InvalidOperationException("File is not supported.");
                }

                await _settingService.SaveSettingOverridablePerStoreAsync(commonSettings, x => x.FaviconAndAppIconsHeadCode, true, storeScope);

                //delete old favicon icon if exist
                var oldFaviconIconPath = _fileProvider.GetAbsolutePath(string.Format(TvProgCommonDefaults.OldFaviconIconName, storeScope));
                if (_fileProvider.FileExists(oldFaviconIconPath))
                {
                    _fileProvider.DeleteFile(oldFaviconIconPath);
                }

                //activity log
                await _userActivityService.InsertActivityAsync("UploadIcons", string.Format(await _localizationService.GetResourceAsync("ActivityLog.UploadNewIcons"), storeScope));
                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.FaviconAndAppIcons.Uploaded"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("GeneralCommon");
        }

        public virtual async Task<IActionResult> AllSettings(string settingName)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _settingModelFactory.PrepareSettingSearchModelAsync(new SettingSearchModel { SearchSettingName = WebUtility.HtmlEncode(settingName) });

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> AllSettings(SettingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _settingModelFactory.PrepareSettingListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> SettingUpdate(SettingModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();

            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a setting with the specified id
            var setting = await _settingService.GetSettingByIdAsync(model.Id)
                ?? throw new ArgumentException("No setting found with the specified id");

            if (!setting.Name.Equals(model.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                //setting name has been changed
                await _settingService.DeleteSettingAsync(setting);
            }

            await _settingService.SetSettingAsync(model.Name, model.Value, setting.StoreId);

            //activity log
            await _userActivityService.InsertActivityAsync("EditSettings", await _localizationService.GetResourceAsync("ActivityLog.EditSettings"), setting);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> SettingAdd(SettingModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (model.Name != null)
                model.Name = model.Name.Trim();

            if (model.Value != null)
                model.Value = model.Value.Trim();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            var storeId = model.StoreId;
            await _settingService.SetSettingAsync(model.Name, model.Value, storeId);

            //activity log
            await _userActivityService.InsertActivityAsync("AddNewSetting",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewSetting"), model.Name),
                await _settingService.GetSettingAsync(model.Name, storeId));

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> SettingDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get a setting with the specified id
            var setting = await _settingService.GetSettingByIdAsync(id)
                ?? throw new ArgumentException("No setting found with the specified id", nameof(id));

            await _settingService.DeleteSettingAsync(setting);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteSetting",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteSetting"), setting.Name), setting);

            return new NullJsonResult();
        }

        //action displaying notification (warning) to a store owner about a lot of traffic 
        //between the distributed cache server and the application when LoadAllLocaleRecordsOnStartup setting is set
        public async Task<IActionResult> DistributedCacheHighTrafficWarning(bool loadAllLocaleRecordsOnStartup)
        {
            //LoadAllLocaleRecordsOnStartup is set and distributed cache is used, so display warning
            if (_appSettings.Get<DistributedCacheConfig>().Enabled && _appSettings.Get<DistributedCacheConfig>().DistributedCacheType != DistributedCacheType.Memory && loadAllLocaleRecordsOnStartup)
            {
                return Json(new
                {
                    Result = await _localizationService
                        .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.LoadAllLocaleRecordsOnStartup.Warning")
                });
            }

            return Json(new { Result = string.Empty });
        }

        //Action that displays a notification (warning) to the store owner about the absence of active authentication providers
        public async Task<IActionResult> ForceMultifactorAuthenticationWarning(bool forceMultifactorAuthentication)
        {
            //ForceMultifactorAuthentication is set and the store haven't active Authentication provider , so display warning
            if (forceMultifactorAuthentication && !await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                return Json(new
                {
                    Result = await _localizationService
                        .GetResourceAsync("Admin.Configuration.Settings.UserUser.ForceMultifactorAuthentication.Warning")
                });
            }

            return Json(new { Result = string.Empty });
        }

        //Action that displays a notification (warning) to the store owner about the need to restart the application after changing the setting
        public async Task<IActionResult> SeoFriendlyUrlsForLanguagesEnabledWarning(bool seoFriendlyUrlsForLanguagesEnabled)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var localizationSettings = await _settingService.LoadSettingAsync<LocalizationSettings>(storeScope);

            if (seoFriendlyUrlsForLanguagesEnabled != localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                return Json(new
                {
                    Result = await _localizationService
                        .GetResourceAsync("Admin.Configuration.Settings.GeneralCommon.SeoFriendlyUrlsForLanguagesEnabled.Warning")
                });
            }

            return Json(new { Result = string.Empty });
        }

        #endregion
    }
}