﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Plugin.Payments.CheckMoneyOrder.Models;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Web.Framework;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.Plugin.Payments.CheckMoneyOrder.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class PaymentCheckMoneyOrderController : BasePaymentController
    {
        #region Fields

        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        #endregion

        #region Ctor

        public PaymentCheckMoneyOrderController(ILanguageService languageService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _languageService = languageService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<CheckMoneyOrderPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                DescriptionText = checkMoneyOrderPaymentSettings.DescriptionText
            };

            //locales
            await AddLocalesAsync(_languageService, model.Locales, async (locale, languageId) =>
            {
                locale.DescriptionText = await _localizationService
                    .GetLocalizedSettingAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, languageId, 0, false, false);
            });
            model.AdditionalFee = checkMoneyOrderPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = checkMoneyOrderPaymentSettings.AdditionalFeePercentage;
            model.ShippableTvChannelRequired = checkMoneyOrderPaymentSettings.ShippableTvChannelRequired;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableTvChannelRequired_OverrideForStore = await _settingService.SettingExistsAsync(checkMoneyOrderPaymentSettings, x => x.ShippableTvChannelRequired, storeScope);
            }

            return View("~/Plugins/Payments.CheckMoneyOrder/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var checkMoneyOrderPaymentSettings = await _settingService.LoadSettingAsync<CheckMoneyOrderPaymentSettings>(storeScope);

            //save settings
            checkMoneyOrderPaymentSettings.DescriptionText = model.DescriptionText;
            checkMoneyOrderPaymentSettings.AdditionalFee = model.AdditionalFee;
            checkMoneyOrderPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            checkMoneyOrderPaymentSettings.ShippableTvChannelRequired = model.ShippableTvChannelRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(checkMoneyOrderPaymentSettings, x => x.ShippableTvChannelRequired, model.ShippableTvChannelRequired_OverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                await _localizationService.SaveLocalizedSettingAsync(checkMoneyOrderPaymentSettings,
                    x => x.DescriptionText, localized.LanguageId, localized.DescriptionText);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}