using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Core.Domain.Stores;
using TVProgViewer.Services.Configuration;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Stores;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Stores;
using TVProgViewer.Web.Framework.Controllers;
using TVProgViewer.Web.Framework.Mvc.Filters;
using System.Threading.Tasks;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
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

        #endregion

        #region Ctor

        public StoreController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IStoreModelFactory storeModelFactory,
            IStoreService storeService)
        {
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _storeModelFactory = storeModelFactory;
            _storeService = storeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateAttributeLocalesAsync(Store store, StoreModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(store,
                    x => x.Name,
                    localized.Name,
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
                await UpdateAttributeLocalesAsync(store, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Configuration.Stores.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = store.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _storeModelFactory.PrepareStoreModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            //try to get a store with the specified id
            var store = await _storeService.GetStoreByIdAsync(id);
            if (store == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _storeModelFactory.PrepareStoreModelAsync(null, store);

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
                await UpdateAttributeLocalesAsync(store, model);

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