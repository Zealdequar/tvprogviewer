using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Core;
using TvProgViewer.Core.Domain.Catalog;
using TvProgViewer.Core.Events;
using TvProgViewer.Services.Catalog;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Models.Catalog;
using TvProgViewer.Web.Framework.Mvc.Filters;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class TvChannelReviewController : BaseAdminController
    {
        #region Fields

        private readonly CatalogSettings _catalogSettings;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IEventPublisher _eventPublisher;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly ITvChannelReviewModelFactory _tvChannelReviewModelFactory;
        private readonly ITvChannelService _tvChannelService;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion Fields

        #region Ctor

        public TvChannelReviewController(CatalogSettings catalogSettings,
            IUserActivityService userActivityService,
            IUserService userService,
            IEventPublisher eventPublisher,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            ITvChannelReviewModelFactory tvChannelReviewModelFactory,
            ITvChannelService tvChannelService,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService)
        {
            _catalogSettings = catalogSettings;
            _userActivityService = userActivityService;
            _userService = userService;
            _eventPublisher = eventPublisher;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _tvChannelReviewModelFactory = tvChannelReviewModelFactory;
            _tvChannelService = tvChannelService;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //prepare model
            var model = await _tvChannelReviewModelFactory.PrepareTvChannelReviewSearchModelAsync(new TvChannelReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TvChannelReviewSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvChannelReviewModelFactory.PrepareTvChannelReviewListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvChannel review with the specified id
            var tvChannelReview = await _tvChannelService.GetTvChannelReviewByIdAsync(id);
            if (tvChannelReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvChannelReviewModelFactory.PrepareTvChannelReviewModelAsync(null, tvChannelReview);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelReviewModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvChannel review with the specified id
            var tvChannelReview = await _tvChannelService.GetTvChannelReviewByIdAsync(model.Id);
            if (tvChannelReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvChannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var previousIsApproved = tvChannelReview.IsApproved;

                //vendor can edit "Reply text" only
                var isLoggedInAsVendor = currentVendor != null;
                if (!isLoggedInAsVendor)
                {
                    tvChannelReview.Title = model.Title;
                    tvChannelReview.ReviewText = model.ReviewText;
                    tvChannelReview.IsApproved = model.IsApproved;
                }

                tvChannelReview.ReplyText = model.ReplyText;

                //notify user about reply
                if (tvChannelReview.IsApproved && !string.IsNullOrEmpty(tvChannelReview.ReplyText)
                    && _catalogSettings.NotifyUserAboutTvChannelReviewReply && !tvChannelReview.UserNotifiedOfReply)
                {
                    var user = await _userService.GetUserByIdAsync(tvChannelReview.UserId);
                    var userLanguageId = user?.LanguageId ?? 0;

                    var queuedEmailIds = await _workflowMessageService.SendTvChannelReviewReplyUserNotificationMessageAsync(tvChannelReview, userLanguageId);
                    if (queuedEmailIds.Any())
                        tvChannelReview.UserNotifiedOfReply = true;
                }

                await _tvChannelService.UpdateTvChannelReviewAsync(tvChannelReview);

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannelReview",
                   string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannelReview"), tvChannelReview.Id), tvChannelReview);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId);
                    //update tvChannel totals
                    await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && tvChannelReview.IsApproved)
                        await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvChannelReview));
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = tvChannelReview.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _tvChannelReviewModelFactory.PrepareTvChannelReviewModelAsync(model, tvChannelReview, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvChannel review with the specified id
            var tvChannelReview = await _tvChannelService.GetTvChannelReviewByIdAsync(id);
            if (tvChannelReview == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            await _tvChannelService.DeleteTvChannelReviewAsync(tvChannelReview);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannelReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelReview"), tvChannelReview.Id), tvChannelReview);

            var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId);

            //update tvChannel totals
            await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> ApproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            //filter not approved reviews
            var tvChannelReviews = (await _tvChannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);

            foreach (var tvChannelReview in tvChannelReviews)
            {
                tvChannelReview.IsApproved = true;
                await _tvChannelService.UpdateTvChannelReviewAsync(tvChannelReview);

                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId);

                //update tvChannel totals
                await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);

                //raise event 
                await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvChannelReview));
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DisapproveSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            //filter approved reviews
            var tvChannelReviews = (await _tvChannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);

            foreach (var tvChannelReview in tvChannelReviews)
            {
                tvChannelReview.IsApproved = false;
                await _tvChannelService.UpdateTvChannelReviewAsync(tvChannelReview);

                var tvChannel = await _tvChannelService.GetTvChannelByIdAsync(tvChannelReview.TvChannelId);

                //update tvChannel totals
                await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            if (selectedIds == null || selectedIds.Count == 0)
                return NoContent();

            var tvChannelReviews = await _tvChannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray());
            var tvChannels = await _tvChannelService.GetTvChannelsByIdsAsync(tvChannelReviews.Select(p => p.TvChannelId).Distinct().ToArray());

            await _tvChannelService.DeleteTvChannelReviewsAsync(tvChannelReviews);

            //update tvChannel totals
            foreach (var tvChannel in tvChannels)
            {
                await _tvChannelService.UpdateTvChannelReviewTotalsAsync(tvChannel);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelReviewReviewTypeMappingList(TvChannelReviewReviewTypeMappingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return await AccessDeniedDataTablesJson();
            var tvChannelReview = await _tvChannelService.GetTvChannelReviewByIdAsync(searchModel.TvChannelReviewId)
                ?? throw new ArgumentException("No tvChannel review found with the specified id");

            //prepare model
            var model = await _tvChannelReviewModelFactory.PrepareTvChannelReviewReviewTypeMappingListModelAsync(searchModel, tvChannelReview);

            return Json(model);
        }

        #endregion
    }
}