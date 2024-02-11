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
        private readonly ITvChannelReviewModelFactory _tvchannelReviewModelFactory;
        private readonly ITvChannelService _tvchannelService;
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
            ITvChannelReviewModelFactory tvchannelReviewModelFactory,
            ITvChannelService tvchannelService,
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
            _tvchannelReviewModelFactory = tvchannelReviewModelFactory;
            _tvchannelService = tvchannelService;
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
            var model = await _tvchannelReviewModelFactory.PrepareTvChannelReviewSearchModelAsync(new TvChannelReviewSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TvChannelReviewSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _tvchannelReviewModelFactory.PrepareTvChannelReviewListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvchannel review with the specified id
            var tvchannelReview = await _tvchannelService.GetTvChannelReviewByIdAsync(id);
            if (tvchannelReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            //prepare model
            var model = await _tvchannelReviewModelFactory.PrepareTvChannelReviewModelAsync(null, tvchannelReview);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TvChannelReviewModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvchannel review with the specified id
            var tvchannelReview = await _tvchannelService.GetTvChannelReviewByIdAsync(model.Id);
            if (tvchannelReview == null)
                return RedirectToAction("List");

            //a vendor should have access only to his tvchannels
            var currentVendor = await _workContext.GetCurrentVendorAsync();
            if (currentVendor != null && (await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId)).VendorId != currentVendor.Id)
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var previousIsApproved = tvchannelReview.IsApproved;

                //vendor can edit "Reply text" only
                var isLoggedInAsVendor = currentVendor != null;
                if (!isLoggedInAsVendor)
                {
                    tvchannelReview.Title = model.Title;
                    tvchannelReview.ReviewText = model.ReviewText;
                    tvchannelReview.IsApproved = model.IsApproved;
                }

                tvchannelReview.ReplyText = model.ReplyText;

                //notify user about reply
                if (tvchannelReview.IsApproved && !string.IsNullOrEmpty(tvchannelReview.ReplyText)
                    && _catalogSettings.NotifyUserAboutTvChannelReviewReply && !tvchannelReview.UserNotifiedOfReply)
                {
                    var user = await _userService.GetUserByIdAsync(tvchannelReview.UserId);
                    var userLanguageId = user?.LanguageId ?? 0;

                    var queuedEmailIds = await _workflowMessageService.SendTvChannelReviewReplyUserNotificationMessageAsync(tvchannelReview, userLanguageId);
                    if (queuedEmailIds.Any())
                        tvchannelReview.UserNotifiedOfReply = true;
                }

                await _tvchannelService.UpdateTvChannelReviewAsync(tvchannelReview);

                //activity log
                await _userActivityService.InsertActivityAsync("EditTvChannelReview",
                   string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTvChannelReview"), tvchannelReview.Id), tvchannelReview);

                //vendor can edit "Reply text" only
                if (!isLoggedInAsVendor)
                {
                    var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId);
                    //update tvchannel totals
                    await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);

                    //raise event (only if it wasn't approved before and is approved now)
                    if (!previousIsApproved && tvchannelReview.IsApproved)
                        await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvchannelReview));
                }

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.TvChannelReviews.Updated"));

                return continueEditing ? RedirectToAction("Edit", new { id = tvchannelReview.Id }) : RedirectToAction("List");
            }

            //prepare model
            model = await _tvchannelReviewModelFactory.PrepareTvChannelReviewModelAsync(model, tvchannelReview, true);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return AccessDeniedView();

            //try to get a tvchannel review with the specified id
            var tvchannelReview = await _tvchannelService.GetTvChannelReviewByIdAsync(id);
            if (tvchannelReview == null)
                return RedirectToAction("List");

            //a vendor does not have access to this functionality
            if (await _workContext.GetCurrentVendorAsync() != null)
                return RedirectToAction("List");

            await _tvchannelService.DeleteTvChannelReviewAsync(tvchannelReview);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteTvChannelReview",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTvChannelReview"), tvchannelReview.Id), tvchannelReview);

            var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId);

            //update tvchannel totals
            await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);

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
            var tvchannelReviews = (await _tvchannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray())).Where(review => !review.IsApproved);

            foreach (var tvchannelReview in tvchannelReviews)
            {
                tvchannelReview.IsApproved = true;
                await _tvchannelService.UpdateTvChannelReviewAsync(tvchannelReview);

                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId);

                //update tvchannel totals
                await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);

                //raise event 
                await _eventPublisher.PublishAsync(new TvChannelReviewApprovedEvent(tvchannelReview));
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
            var tvchannelReviews = (await _tvchannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray())).Where(review => review.IsApproved);

            foreach (var tvchannelReview in tvchannelReviews)
            {
                tvchannelReview.IsApproved = false;
                await _tvchannelService.UpdateTvChannelReviewAsync(tvchannelReview);

                var tvchannel = await _tvchannelService.GetTvChannelByIdAsync(tvchannelReview.TvChannelId);

                //update tvchannel totals
                await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);
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

            var tvchannelReviews = await _tvchannelService.GetTvChannelReviewsByIdsAsync(selectedIds.ToArray());
            var tvchannels = await _tvchannelService.GetTvChannelsByIdsAsync(tvchannelReviews.Select(p => p.TvChannelId).Distinct().ToArray());

            await _tvchannelService.DeleteTvChannelReviewsAsync(tvchannelReviews);

            //update tvchannel totals
            foreach (var tvchannel in tvchannels)
            {
                await _tvchannelService.UpdateTvChannelReviewTotalsAsync(tvchannel);
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public virtual async Task<IActionResult> TvChannelReviewReviewTypeMappingList(TvChannelReviewReviewTypeMappingSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTvChannelReviews))
                return await AccessDeniedDataTablesJson();
            var tvchannelReview = await _tvchannelService.GetTvChannelReviewByIdAsync(searchModel.TvChannelReviewId)
                ?? throw new ArgumentException("No tvchannel review found with the specified id");

            //prepare model
            var model = await _tvchannelReviewModelFactory.PrepareTvChannelReviewReviewTypeMappingListModelAsync(searchModel, tvchannelReview);

            return Json(model);
        }

        #endregion
    }
}