using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using TVProgViewer.Core.Domain.Payments;
using TVProgViewer.Services.Configuration;
using TVProgViewer.Services.Directory;
using TVProgViewer.Services.Events;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Payments;
using TVProgViewer.Services.Plugins;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Payments;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Core.Events;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class PaymentController : BaseAdminController
    {
        #region Fields

        private readonly ICountryService _countryService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPaymentModelFactory _paymentModelFactory;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly PaymentSettings _paymentSettings;

        #endregion

        #region Ctor

        public PaymentController(ICountryService countryService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPaymentModelFactory paymentModelFactory,
            IPaymentPluginManager paymentPluginManager,
            IPermissionService permissionService,
            ISettingService settingService,
            PaymentSettings paymentSettings)
        {
            _countryService = countryService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _paymentModelFactory = paymentModelFactory;
            _paymentPluginManager = paymentPluginManager;
            _permissionService = permissionService;
            _settingService = settingService;
            _paymentSettings = paymentSettings;
        }

        #endregion

        #region Methods  

        public virtual IActionResult PaymentMethods()
        {
            return RedirectToAction("Methods");
        }

        public virtual async Task<IActionResult> Methods()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Methods(PaymentMethodSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _paymentModelFactory.PreparePaymentMethodListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> MethodUpdate(PaymentMethodModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(model.SystemName);
            if (_paymentPluginManager.IsPluginActive(pm))
            {
                if (!model.IsActive)
                {
                    //mark as disabled
                    _paymentSettings.ActivePaymentMethodSystemNames.Remove(pm.PluginDescriptor.SystemName);
                    await _settingService.SaveSettingAsync(_paymentSettings);
                }
            }
            else
            {
                if (model.IsActive)
                {
                    //mark as active
                    _paymentSettings.ActivePaymentMethodSystemNames.Add(pm.PluginDescriptor.SystemName);
                    await _settingService.SaveSettingAsync(_paymentSettings);
                }
            }

            var pluginDescriptor = pm.PluginDescriptor;
            pluginDescriptor.FriendlyName = model.FriendlyName;
            pluginDescriptor.DisplayOrder = model.DisplayOrder;

            //update the description file
            pluginDescriptor.Save();

            //raise event
            await _eventPublisher.PublishAsync(new PluginUpdatedEvent(pluginDescriptor));

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> MethodRestrictions()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prepare model
            var model = await _paymentModelFactory.PreparePaymentMethodsModelAsync(new PaymentMethodsModel());

            return View(model);
        }

        //we ignore this filter for increase RequestFormLimits
        [IgnoreAntiforgeryToken]
        //we use 2048 value because in some cases default value (1024) is too small for this action
        [RequestFormLimits(ValueCountLimit = 2048)]
        [HttpPost, ActionName("MethodRestrictions")]
        public virtual async Task<IActionResult> MethodRestrictionsSave(PaymentMethodsModel model, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            var paymentMethods = await _paymentPluginManager.LoadAllPluginsAsync();
            var countries = await _countryService.GetAllCountriesAsync(showHidden: true);

            foreach (var pm in paymentMethods)
            {
                var formKey = "restrict_" + pm.PluginDescriptor.SystemName;
                var countryIdsToRestrict = (!StringValues.IsNullOrEmpty(form[formKey])
                        ? form[formKey].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList()
                        : new List<string>())
                    .Select(x => Convert.ToInt32(x)).ToList();

                var newCountryIds = new List<int>();
                foreach (var c in countries)
                {
                    if (countryIdsToRestrict.Contains(c.Id))
                    {
                        newCountryIds.Add(c.Id);
                    }
                }

                await _paymentPluginManager.SaveRestrictedCountriesAsync(pm, newCountryIds);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Payment.MethodRestrictions.Updated"));

            return RedirectToAction("MethodRestrictions");
        }

        #endregion
    }
}