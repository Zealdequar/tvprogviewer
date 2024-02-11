using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    /// <summary>
    /// Represent a review type controller
    /// </summary>
    public partial class ReviewTypeController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IReviewTypeModelFactory _reviewTypeModelFactory;
        private readonly IReviewTypeService _reviewTypeService;

        #endregion

        #region Ctor

        public ReviewTypeController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IReviewTypeModelFactory reviewTypeModelFactory,
            IReviewTypeService reviewTypeService)
        {
            _reviewTypeModelFactory = reviewTypeModelFactory;
            _reviewTypeService = reviewTypeService;
            _userActivityService = userActivityService;
            _localizedEntityService = localizedEntityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Utilities

        protected virtual async Task UpdateReviewTypeLocalesAsync(ReviewType reviewType, ReviewTypeModel model)
        {
            foreach (var localized in model.Locales)
            {
                await _localizedEntityService.SaveLocalizedValueAsync(reviewType,
                    x => x.Name,                    
                    localized.Name,
                    localized.LanguageId);

                await _localizedEntityService.SaveLocalizedValueAsync(reviewType,
                   x => x.Description,
                   localized.Description,
                   localized.LanguageId);
            }
        }

        #endregion

        #region Review type

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //select an appropriate card
            SaveSelectedCardName("catalogsettings-review-types");

            //we just redirect a user to the catalog settings page
            return RedirectToAction("Catalog", "Setting");
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ReviewTypeSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _reviewTypeModelFactory.PrepareReviewTypeListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //prepare model
            var model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(new ReviewTypeModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Create(ReviewTypeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var reviewType = model.ToEntity<ReviewType>();
                await _reviewTypeService.InsertReviewTypeAsync(reviewType);                

                //activity log
                await _userActivityService.InsertActivityAsync("AddNewReviewType",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewReviewType"), reviewType.Id), reviewType);

                //locales                
                await UpdateReviewTypeLocalesAsync(reviewType, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Added"));

                return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");                
            }

            //prepare model
            model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(model, null, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an tvchannel review type with the specified id
            var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
            if (reviewType == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(null, reviewType);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(ReviewTypeModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an review type with the specified id
            var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(model.Id);
            if (reviewType == null)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                reviewType = model.ToEntity(reviewType);
                await _reviewTypeService.UpdateReviewTypeAsync(reviewType);

                //activity log
                await _userActivityService.InsertActivityAsync("EditReviewType",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditReviewType"), reviewType.Id),
                    reviewType);

                //locales
                await UpdateReviewTypeLocalesAsync(reviewType, model);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = reviewType.Id }) : RedirectToAction("List");                
            }

            //prepare model
            model = await _reviewTypeModelFactory.PrepareReviewTypeModelAsync(model, reviewType, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageSettings))
                return AccessDeniedView();

            //try to get an review type with the specified id
            var reviewType = await _reviewTypeService.GetReviewTypeByIdAsync(id);
            if (reviewType == null)
                return RedirectToAction("List");

            try
            {
                await _reviewTypeService.DeleteReviewTypeAsync(reviewType);

                //activity log
                await _userActivityService.InsertActivityAsync("DeleteReviewType",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteReviewType"), reviewType),
                    reviewType);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Settings.ReviewType.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
                return RedirectToAction("Edit", new { id = reviewType.Id });
            }            
        }

        #endregion
    }
}
