﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Core.Domain.Stores;
using TvProgViewer.Services.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.Services.Stores;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Stores;
using TvProgViewer.Web.Framework.Controllers;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class StoreController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IStoreModelFactory _storeModelFactory;
        private readonly IStoreService _storeService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public StoreController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreModelFactory storeModelFactory,
            IStoreService storeService,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper,
            IWorkContext workContext)
        {
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeModelFactory = storeModelFactory;
            _storeService = storeService;
            _genericAttributeService = genericAttributeService;
            _webHelper = webHelper;
            _workContext = workContext;

        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.DefaultTitle,
                    localized.DefaultTitle,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.DefaultMetaDescription,
                    localized.DefaultMetaDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.DefaultMetaKeywords,
                    localized.DefaultMetaKeywords,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.HomepageDescription,
                    localized.HomepageDescription,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.HomepageTitle,
                    localized.HomepageTitle,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreSearchModelAsync(new StoreSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(StoreSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //prepare model
            var model = await _storeModelFactory.PrepareStoreModelAsync(new StoreModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(StoreModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var store = model.ToEntity<Store>();

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await _storeService.InsertStoreAsync(store);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewStore",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewStore"), store.Id), store);

                //locales
                await UpdateLocalesAsync(store, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _storeModelFactory.PrepareStoreModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpsRequirement(ignore: true)]
        public virtual async Task<IActionResult> SetStoreSslByCurrentRequestScheme(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            var value = _webHelper.IsCurrentConnectionSecured();

            if (store.SslEnabled != value)
            {
                store.SslEnabled = value;
                await _storeService.UpdateStoreAsync(store);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Ssl.Updated"));
            }

            return RedirectToAction("Edit", new { id = id });
        }

        [HttpsRequirement(ignore: true)]
        public virtual async Task<IActionResult> Edit(int id, bool showtour = false)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _storeModelFactory.PrepareStoreModelAsync(null, store);

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

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Edit(StoreModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreByIdAsync(model.Id);
            if (store == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                store = model.ToEntity(store);

                //ensure we have "/" at the end
                if (!store.Url.EndsWith("/"))
                    store.Url += "/";

                await _storeService.UpdateStoreAsync(store);

                //activity log
                await _userActivityService.InsertActivityAsync("EditStore",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditStore"), store.Id), store);

                //locales
                await UpdateLocalesAsync(store, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _storeModelFactory.PrepareStoreModelAsync(model, store, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            try
            {
                await _storeService.DeleteStoreAsync(store);

                //activity log
                await _userActivityService.InsertActivityAsync("DeleteStore",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteStore"), store.Id), store);

                //when we delete a store we should also ensure that all "per store" settings will also be deleted
                var settingsToDelete = (await _settingService
                    .GetAllSettingsAsync())
                    .Where(s => s.StoreId == id)
                    .ToList();
                await _settingService.DeleteSettingsAsync(settingsToDelete);

                //when we had two stores and now have only one store, we also should delete all "per store" settings
                var allStores = await _storeService.GetAllStoresAsync();
                if (allStores.Count == 1)
                {
                    settingsToDelete = (await _settingService
                        .GetAllSettingsAsync())
                        .Where(s => s.StoreId == allStores[0].Id)
                        .ToList();
                    await _settingService.DeleteSettingsAsync(settingsToDelete);
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = store.Id });
            }
        }

        #endregion
    }
}