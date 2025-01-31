﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Orders;
using TvProgViewer.Plugin.Payments.PayPalViewer.Domain;
using TvProgViewer.Plugin.Payments.PayPalViewer.Models;
using TvProgViewer.Plugin.Payments.PayPalViewer.Services;
using TvProgViewer.Services;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.Plugin.Payments.PayPalViewer.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    [ValidateIpAddress]
    [AuthorizeAdmin]
    public class PayPalViewerController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ServiceManager _serviceManager;
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PayPalViewerController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext,
            ServiceManager serviceManager,
            ShoppingCartSettings shoppingCartSettings)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
            _serviceManager = serviceManager;
            _shoppingCartSettings = shoppingCartSettings;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare credentials and onboarding model properties
        /// </summary>
        /// <param name="model">Comfiguration model</param>
        /// <param name="settings">Plugin settings</param>
        /// <param name="storeId">Store id</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PrepareCredentialsAsync(ConfigurationModel model, PayPalViewerSettings settings, int storeId)
        {
            model.OnboardingModel.MerchantGuid = settings.MerchantGuid;
            model.OnboardingModel.SignUpUrl = settings.SignUpUrl;

            //no need to check credentials if the plugin is already configured or credentials were manually set
            if (settings.SetCredentialsManually || ServiceManager.IsConfigured(settings))
                return;

            //no need to check credentials until the merchant has been onboarded and signed up
            if (string.IsNullOrEmpty(settings.MerchantGuid) || !string.IsNullOrEmpty(settings.SignUpUrl))
                return;

            var (merchant, error) = await _serviceManager.GetMerchantAsync(settings.MerchantGuid);
            if (merchant is null || !string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Configuration.Error");
                var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
                return;
            }

            model.OnboardingModel.AccountCreated = !string.IsNullOrEmpty(merchant.MerchantId);
            model.OnboardingModel.EmailConfirmed = merchant.EmailConfirmed;
            model.OnboardingModel.PaymentsReceivable = merchant.PaymentsReceivable;
            model.OnboardingModel.PermissionGranted = merchant.PermissionGranted;
            model.OnboardingModel.DisplayStatus = true;

            if (!merchant.EmailConfirmed || !merchant.PaymentsReceivable || !merchant.PermissionGranted)
            {
                var onboardingNotCompleted = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Onboarding.InProcess");
                _notificationService.WarningNotification(onboardingNotCompleted);
                return;
            }

            if (string.IsNullOrEmpty(merchant.ClientId) || string.IsNullOrEmpty(merchant.ClientSecret))
            {
                var onboardingError = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Onboarding.Error");
                _notificationService.ErrorNotification(onboardingError);
                return;
            }

            var onboardingCompleted = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Onboarding.Completed");
            _notificationService.SuccessNotification(onboardingCompleted);

            //first delete the unused webhook on a previous client, if changed
            if ((!merchant.ClientId?.Equals(settings.ClientId) ?? true) &&
                !string.IsNullOrEmpty(settings.WebhookUrl) &&
                !string.IsNullOrEmpty(settings.ClientId) &&
                !string.IsNullOrEmpty(settings.SecretKey))
            {
                await _serviceManager.DeleteWebhookAsync(settings);
            }

            //set new settings values
            settings.ClientId = merchant.ClientId;
            settings.SecretKey = merchant.ClientSecret;
            model.IsConfigured = ServiceManager.IsConfigured(settings);

            //ensure that webhook created, display warning in case of fail
            if (!string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
            {
                var (webhook, _) = await _serviceManager.CreateWebhookAsync(settings, storeId);
                settings.WebhookUrl = webhook?.Url;
                if (string.IsNullOrEmpty(settings.WebhookUrl))
                {
                    var url = Url.Action("List", "Log");
                    var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.WebhookWarning"), url);
                    _notificationService.WarningNotification(warning, false);
                }
            }

            if (!string.IsNullOrEmpty(merchant.Email) && !merchant.Email.Equals(settings.Email, StringComparison.InvariantCultureIgnoreCase))
            {
                settings.Email = merchant.Email;
                model.Email = merchant.Email;
            }

            var overrideSettings = storeId > 0;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Email, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.WebhookUrl, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, overrideSettings, storeId, false);
            await _settingService.ClearCacheAsync();
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PayPalViewerSettings>(storeId);

            //we don't need some of the shared settings that loaded above, so load them separately for chosen store
            if (storeId > 0)
            {
                settings.WebhookUrl = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.WebhookUrl)}", storeId: storeId);
                settings.UseSandbox = await _settingService
                    .GetSettingByKeyAsync<bool>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.UseSandbox)}", storeId: storeId);
                settings.ClientId = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.ClientId)}", storeId: storeId);
                settings.SecretKey = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.SecretKey)}", storeId: storeId);
                settings.Email = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.Email)}", storeId: storeId);
                settings.MerchantGuid = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.MerchantGuid)}", storeId: storeId);
                settings.SignUpUrl = await _settingService
                    .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.SignUpUrl)}", storeId: storeId);
            }

            var model = new ConfigurationModel
            {
                Email = settings.Email,
                SetCredentialsManually = settings.SetCredentialsManually,
                UseSandbox = settings.UseSandbox,
                ClientId = settings.SetCredentialsManually ? settings.ClientId : string.Empty,
                SecretKey = settings.SetCredentialsManually ? settings.SecretKey : string.Empty,
                PaymentTypeId = (int)settings.PaymentType,
                DisplayButtonsOnShoppingCart = settings.DisplayButtonsOnShoppingCart,
                DisplayButtonsOnTvChannelDetails = settings.DisplayButtonsOnTvChannelDetails,
                DisplayLogoInHeaderLinks = settings.DisplayLogoInHeaderLinks,
                LogoInHeaderLinks = settings.LogoInHeaderLinks,
                DisplayLogoInFooter = settings.DisplayLogoInFooter,
                DisplayPayLaterMessages = settings.DisplayPayLaterMessages,
                LogoInFooter = settings.LogoInFooter,
                ActiveStoreScopeConfiguration = storeId,
                IsConfigured = ServiceManager.IsConfigured(settings)
            };

            await PrepareCredentialsAsync(model, settings, storeId);

            if (storeId > 0)
            {
                model.Email_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.Email, storeId);
                model.SetCredentialsManually_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SetCredentialsManually, storeId);
                model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.UseSandbox, storeId);
                model.ClientId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.ClientId, storeId);
                model.SecretKey_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.SecretKey, storeId);
                model.PaymentTypeId_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.PaymentType, storeId);
                model.DisplayButtonsOnShoppingCart_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayButtonsOnShoppingCart, storeId);
                model.DisplayButtonsOnTvChannelDetails_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayButtonsOnTvChannelDetails, storeId);
                model.DisplayLogoInHeaderLinks_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayLogoInHeaderLinks, storeId);
                model.LogoInHeaderLinks_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LogoInHeaderLinks, storeId);
                model.DisplayLogoInFooter_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayLogoInFooter, storeId);
                model.DisplayPayLaterMessages_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.DisplayPayLaterMessages, storeId);
                model.LogoInFooter_OverrideForStore = await _settingService.SettingExistsAsync(settings, setting => setting.LogoInFooter, storeId);
            }

            model.PaymentTypes = (await PaymentType.Capture.ToSelectListAsync(false))
                .Select(item => new SelectListItem(item.Text, item.Value))
                .ToList();

            //prices and total aren't rounded, so display warning
            if (model.IsConfigured && !_shoppingCartSettings.RoundPricesDuringCalculation)
            {
                var url = Url.Action("AllSettings", "Setting", new { settingName = nameof(ShoppingCartSettings.RoundPricesDuringCalculation) });
                var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.RoundingWarning"), url);
                _notificationService.WarningNotification(warning, false);
            }

            //ensure credentials are valid
            if (!string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
            {
                var (_, credentialsError) = await _serviceManager.GetAccessTokenAsync(settings);
                if (!string.IsNullOrEmpty(credentialsError))
                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Credentials.Invalid"));
                else
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Credentials.Valid"));
            }

            return View("~/Plugins/Payments.PayPalViewer/Views/Configure.cshtml", model);
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PayPalViewerSettings>(storeId);

            //set new settings values
            settings.SetCredentialsManually = model.SetCredentialsManually;
            settings.PaymentType = (PaymentType)model.PaymentTypeId;
            settings.DisplayButtonsOnShoppingCart = model.DisplayButtonsOnShoppingCart;
            settings.DisplayButtonsOnTvChannelDetails = model.DisplayButtonsOnTvChannelDetails;
            settings.DisplayLogoInHeaderLinks = model.DisplayLogoInHeaderLinks;
            settings.LogoInHeaderLinks = model.LogoInHeaderLinks;
            settings.DisplayLogoInFooter = model.DisplayLogoInFooter;
            settings.DisplayPayLaterMessages = model.DisplayPayLaterMessages;
            settings.LogoInFooter = model.LogoInFooter;

            if (model.SetCredentialsManually)
            {
                //we don't need some of the shared settings that loaded above, so load them separately for chosen store
                if (storeId > 0)
                {
                    settings.WebhookUrl = await _settingService
                        .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.WebhookUrl)}", storeId: storeId);
                    settings.UseSandbox = await _settingService
                        .GetSettingByKeyAsync<bool>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.UseSandbox)}", storeId: storeId);
                    settings.ClientId = await _settingService
                        .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.ClientId)}", storeId: storeId);
                    settings.SecretKey = await _settingService
                        .GetSettingByKeyAsync<string>($"{nameof(PayPalViewerSettings)}.{nameof(PayPalViewerSettings.SecretKey)}", storeId: storeId);
                }

                //first delete the unused webhook on a previous client, if changed
                if ((!model.ClientId?.Equals(settings.ClientId) ?? true) &&
                    !string.IsNullOrEmpty(settings.WebhookUrl) &&
                    !string.IsNullOrEmpty(settings.ClientId) &&
                    !string.IsNullOrEmpty(settings.SecretKey))
                {
                    await _serviceManager.DeleteWebhookAsync(settings);
                }

                settings.UseSandbox = model.UseSandbox;
                settings.ClientId = model.ClientId;
                settings.SecretKey = model.SecretKey;

                //ensure that webhook created, display warning in case of fail
                if (!string.IsNullOrEmpty(settings.ClientId) && !string.IsNullOrEmpty(settings.SecretKey))
                {
                    var (webhook, _) = await _serviceManager.CreateWebhookAsync(settings, storeId);
                    settings.WebhookUrl = webhook?.Url;
                    if (string.IsNullOrEmpty(settings.WebhookUrl))
                    {
                        var url = Url.Action("List", "Log");
                        var warning = string.Format(await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.WebhookWarning"), url);
                        _notificationService.WarningNotification(warning, false);
                    }
                }

                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.WebhookUrl, model.ClientId_OverrideForStore, storeId, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseSandbox, model.UseSandbox_OverrideForStore, storeId, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, model.ClientId_OverrideForStore, storeId, false);
                await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, model.SecretKey_OverrideForStore, storeId, false);
            }

            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SetCredentialsManually, model.SetCredentialsManually_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.PaymentType, model.PaymentTypeId_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DisplayButtonsOnShoppingCart, model.DisplayButtonsOnShoppingCart_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DisplayButtonsOnTvChannelDetails, model.DisplayButtonsOnTvChannelDetails_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DisplayLogoInHeaderLinks, model.DisplayLogoInHeaderLinks_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.LogoInHeaderLinks, model.LogoInHeaderLinks_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DisplayLogoInFooter, model.DisplayLogoInFooter_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.DisplayPayLaterMessages, model.DisplayPayLaterMessages_OverrideForStore, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.LogoInFooter, model.LogoInFooter_OverrideForStore, storeId, false);
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("onboarding")]
        public async Task<IActionResult> Onboarding(OnboardingModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PayPalViewerSettings>(storeId);

            //try to onboard merchant with the passed email
            var (merchant, error) = await _serviceManager.OnboardAsync(model.Email);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Configuration.Error");
                var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
                return await Configure();
            }

            settings.SetCredentialsManually = false;
            settings.UseSandbox = false;
            settings.ClientId = string.Empty;
            settings.SecretKey = string.Empty;
            settings.Email = merchant.Email;
            settings.SignUpUrl = merchant.SignUpUrl;
            settings.MerchantGuid = merchant.MerchantGuid;
            var overrideSettings = model.Email_OverrideForStore;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SetCredentialsManually, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.UseSandbox, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Email, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SignUpUrl, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.MerchantGuid, overrideSettings, storeId, false);
            await _settingService.ClearCacheAsync();

            var emailSet = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Onboarding.EmailSet");
            _notificationService.SuccessNotification(emailSet);

            return await Configure();
        }

        [HttpPost, ActionName("Configure")]
        [FormValueRequired("revoke")]
        public async Task<IActionResult> RevokeAccess()
        {
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PayPalViewerSettings>(storeId);

            var (_, error) = await _serviceManager.RevokeAccessAsync(settings.MerchantGuid);
            if (!string.IsNullOrEmpty(error))
            {
                var locale = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Configuration.Error");
                var errorMessage = string.Format(locale, error, Url.Action("List", "Log"));
                _notificationService.ErrorNotification(errorMessage, false);
                return await Configure();
            }

            settings.Email = string.Empty;
            settings.SignUpUrl = string.Empty;
            settings.MerchantGuid = string.Empty;
            settings.ClientId = string.Empty;
            settings.SecretKey = string.Empty;
            settings.WebhookUrl = string.Empty;
            var overrideSettings = storeId > 0;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.Email, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SignUpUrl, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.MerchantGuid, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.ClientId, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SecretKey, overrideSettings, storeId, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.WebhookUrl, overrideSettings, storeId, false);
            await _settingService.ClearCacheAsync();

            var accessRevoked = await _localizationService.GetResourceAsync("Plugins.Payments.PayPalViewer.Onboarding.AccessRevoked");
            _notificationService.SuccessNotification(accessRevoked);

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(OnboardingModel model)
        {
            await _serviceManager.SignUpAsync(model.MerchantGuid, model.AuthCode, model.SharedId);

            //clear URL since the merchant is already signed up
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<PayPalViewerSettings>(storeId);
            settings.SignUpUrl = null;
            await _settingService.SaveSettingOverridablePerStoreAsync(settings, setting => setting.SignUpUrl, model.Email_OverrideForStore, storeId, false);
            await _settingService.ClearCacheAsync();

            return Ok();
        }

        #endregion
    }
}