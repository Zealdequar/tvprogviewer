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
        private readonly ITvChannelAttributeModelFactory _tvchannelAttributeModelFactory;
        private readonly ITvChannelAttributeService _tvchannelAttributeService;

        #endregion Fields

        #region Ctor

        public TvChannelAttributeController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITvChannelAttributeModelFactory tvchannelAttributeModelFactory,
            ITvChannelAttributeService tvchannelAttributeService)
        {
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _localizedEntityService = localizedEntityService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _tvchannelAttributeModelFactory = tvchannelAttributeModelFactory;
            _tvchannelAttributeService = tvchannelAttributeService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateLocalesAsync(TvChannelAttribute tvchannelAttribute, TvChannelAttributeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(tvchannelAttribute,
                    x => x.Name,
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(tvchannelAttribute,
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
            var model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeSearchModelAsync(new TvChannelAttributeSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TvChannelAttributeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(new TvChannelAttributeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(TvChannelAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var tvchannelAttribute = model.ToEntity<TvChannelAttribute>();
                await _tvchannelAttributeService.InsertTvChannelAttributeAsync(tvchannelAttribute);
                await UpdateLocalesAsync(tvchannelAttribute, model);

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTvChannelAttribute"), tvchannelAttribute.Name), tvchannelAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = tvchannelAttribute.Id });
            }

            //prepare model
            model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(id);
            if (tvchannelAttribute == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(null, tvchannelAttribute);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelAttributeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(model.Id);
            if (tvchannelAttribute == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                tvchannelAttribute = model.ToEntity(tvchannelAttribute);
                await _tvchannelAttributeService.UpdateTvChannelAttributeAsync(tvchannelAttribute);

                await UpdateLocalesAsync(tvchannelAttribute, model);

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannelAttribute"), tvchannelAttribute.Name), tvchannelAttribute);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Attributes.TvChannelAttributes.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");
                
                return RedirectToAction("Edit", new { id = tvchannelAttribute.Id });
            }

            //prepare model
            model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeModelAsync(model, tvchannelAttribute, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(id);
            if (tvchannelAttribute == null)
                return RedirectToAction("List");

            await _tvchannelAttributeService.DeleteTvChannelAttributeAsync(tvchannelAttribute);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannelAttribute",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelAttribute"), tvchannelAttribute.Name), tvchannelAttribute);

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

            var tvchannelAttributes = await _tvchannelAttributeService.GetTvChannelAttributeByIdsAsync(selectedIds.ToArray());
            await _tvchannelAttributeService.DeleteTvChannelAttributesAsync(tvchannelAttributes);

            foreach (var tvchannelAttribute in tvchannelAttributes)
            {
                await _userActivityService.InsertActivityAsync("DeleteTvChannelAttribute",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelAttribute"), tvchannelAttribute.Name), tvchannelAttribute);
            }

            return Json(new { Result = true });
        }

        #endregion

        #region Used by tvchannels

        [HttpPost]
        public virtual async Task<IActionResult> UsedByTvChannels(TvChannelAttributeTvChannelSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(searchModel.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id");

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PrepareTvChannelAttributeTvChannelListModelAsync(searchModel, tvchannelAttribute);

            return Json(model);
        }

        #endregion

        #region Predefined values

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueList(PredefinedTvChannelAttributeValueSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return await AccessDeniedDataTablesJson();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(searchModel.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id");

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueListModelAsync(searchModel, tvchannelAttribute);

            return Json(model);
        }

        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueCreatePopup(int tvchannelAttributeId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(tvchannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id", nameof(tvchannelAttributeId));

            //prepare model
            var model = await _tvchannelAttributeModelFactory
                .PreparePredefinedTvChannelAttributeValueModelAsync(new PredefinedTvChannelAttributeValueModel(), tvchannelAttribute, null);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueCreatePopup(PredefinedTvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(model.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id");

            if (ModelState.IsValid)
            {
                //fill entity from model
                var ppav = model.ToEntity<PredefinedTvChannelAttributeValue>();

                await _tvchannelAttributeService.InsertPredefinedTvChannelAttributeValueAsync(ppav);
                await UpdateLocalesAsync(ppav, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(model, tvchannelAttribute, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueEditPopup(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined tvchannel attribute value found with the specified id");

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(tvchannelAttributeValue.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id");

            //prepare model
            var model = await _tvchannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(null, tvchannelAttribute, tvchannelAttributeValue);

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueEditPopup(PredefinedTvChannelAttributeValueModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(model.Id)
                ?? throw new ArgumentException("No predefined tvchannel attribute value found with the specified id");

            //try to get a tvchannel attribute with the specified id
            var tvchannelAttribute = await _tvchannelAttributeService.GetTvChannelAttributeByIdAsync(tvchannelAttributeValue.TvChannelAttributeId)
                ?? throw new ArgumentException("No tvchannel attribute found with the specified id");

            if (ModelState.IsValid)
            {
                tvchannelAttributeValue = model.ToEntity(tvchannelAttributeValue);
                await _tvchannelAttributeService.UpdatePredefinedTvChannelAttributeValueAsync(tvchannelAttributeValue);

                await UpdateLocalesAsync(tvchannelAttributeValue, model);

                ViewBag.RefreshPage = true;

                return View(model);
            }

            //prepare model
            model = await _tvchannelAttributeModelFactory.PreparePredefinedTvChannelAttributeValueModelAsync(model, tvchannelAttribute, tvchannelAttributeValue, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> PredefinedTvChannelAttributeValueDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageAttributes))
                return AccessDeniedView();

            //try to get a predefined tvchannel attribute value with the specified id
            var tvchannelAttributeValue = await _tvchannelAttributeService.GetPredefinedTvChannelAttributeValueByIdAsync(id)
                ?? throw new ArgumentException("No predefined tvchannel attribute value found with the specified id", nameof(id));

            await _tvchannelAttributeService.DeletePredefinedTvChannelAttributeValueAsync(tvchannelAttributeValue);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}