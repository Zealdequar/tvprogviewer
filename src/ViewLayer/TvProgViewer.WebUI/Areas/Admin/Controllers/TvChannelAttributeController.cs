using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Mvc;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class TvChannelAttributeController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ITvChannelAttributeModelFactory _tvChannelAttributeModelFactory;
        private readonly ITvChannelAttributeService _tvChannelAttributeService;

        #endregion Fields

        #region Ctor

        public TvChannelAttributeController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITvChannelAttributeModelFactory tvChannelAttributeModelFactory,
            ITvChannelAttributeService tvChannelAttributeService)
        {
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _tvChannelAttributeModelFactory = tvChannelAttributeModelFactory;
            _tvChannelAttributeService = tvChannelAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(TvChannelAttribute tvChannelAttribute, TvChannelAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvChannelAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(tvChannelAttribute,
                    x => x.Description,
                    localized.Description,
                    localized.LanguageId);
            }
        }

        protected virtual async Task UpdateLocalesAsync(PredefinedTvChannelAttributeValue ppav, PredefinedTvChannelAttributeValueModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(ppav,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);
            }
        }

        #endregion

        #region Methods

        #region Attribute list / create / edit / delete

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeSearchModelAsync(new TvChannelAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TvChannelAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(new TvChannelAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TvChannelAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var tvChannelAttribute = model.ToEntity<TvChannelAttribute>();
                await _tvChannelAttributeService.InsertTvChannelAttributeAsync(tvChannelAttribute);
                await UpdateLocalesAsync(tvChannelAttribute, model);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTvChannelAttribute"), tvChannelAttribute.Name), tvChannelAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = tvChannelAttribute.Id });
            }

            //prepare model
            model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(id);
            if (tvChannelAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(null, tvChannelAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(model.Id);
            if (tvChannelAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                tvChannelAttribute = model.ToEntity(tvChannelAttribute);
                await _tvChannelAttributeService.UpdateTvChannelAttributeAsync(tvChannelAttribute);

                await UpdateLocalesAsync(tvChannelAttribute, model);

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannelAttribute"), tvChannelAttribute.Name), tvChannelAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = tvChannelAttribute.Id });
            }

            //prepare model
            model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(model, tvChannelAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(id);
            if (tvChannelAttribute == null)
                return RedirectToAction("List");

            await _tvChannelAttributeService.DeleteTvChannelAttributeAsync(tvChannelAttribute);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannelAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelAttribute"), tvChannelAttribute.Name), tvChannelAttribute);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var tvChannelAttributes = await _tvChannelAttributeService.GetTvChannelAttributeByIdsAsync(selectedIds.ToArray());
            await _tvChannelAttributeService.DeleteTvChannelAttributesAsync(tvChannelAttributes);

            foreach (var tvChannelAttribute in tvChannelAttributes)
            {
                await _userActivityService.InsertActivityAsync("DeleteTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelAttribute"), tvChannelAttribute.Name), tvChannelAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Used by tvChannels

        [HttpPost]
        public virtual async Task<IActionResult> UsedByTvChannels(TvChannelAttributeTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(searchModel.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id");

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PrepareTvChannelAttributeTvChannelListModelAsync(searchModel, tvChannelAttribute);

            return Json(model);
        }

        #endregion

        #region Predefined values

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueList(PredefinedTvChannelAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(searchModel.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id");

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueListModelAsync(searchModel, tvChannelAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueCreatePopup(int tvChannelAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id", nameof(tvChannelAttributeId));

            //prepare model
            var model = await _tvChannelAttributeModelFactory
                .PreparePredefinedTvChannelAttributeValueModelAsync(new PredefinedTvChannelAttributeValueModel(), tvChannelAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueCreatePopup(PredefinedTvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(model.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var ppav = model.ToEntity<PredefinedTvChannelAttributeValue>();

                await _tvChannelAttributeService.InsertPredefinedTvChannelAttributeValueAsync(ppav);
                await UpdateLocalesAsync(ppav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(model, tvChannelAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined tvChannel attribute value found with the specified id");

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeValue.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id");

            //prepare model
            var model = await _tvChannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(null, tvChannelAttribute, tvChannelAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueEditPopup(PredefinedTvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(model.Id)
                ?? throw new ArgumentException("No predefined tvChannel attribute value found with the specified id");

            //try to get a tvChannel attribute with the specified id
            var tvChannelAttribute = await _tvChannelAttributeService.GetTvChannelAttributeByIdAsync(tvChannelAttributeValue.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvChannel attribute found with the specified id");

            if (ModelState.IsValid)
            {
                tvChannelAttributeValue = model.ToEntity(tvChannelAttributeValue);
                await _tvChannelAttributeService.UpdatePredefinedTvChannelAttributeValueAsync(tvChannelAttributeValue);

                await UpdateLocalesAsync(tvChannelAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvChannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(model, tvChannelAttribute, tvChannelAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvChannel attribute value with the specified id
            var tvChannelAttributeValue = await _tvChannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined tvChannel attribute value found with the specified id", nameof(id));

            await _tvChannelAttributeService.DeletePredefinedTvChannelAttributeValueAsync(tvChannelAttributeValue);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}