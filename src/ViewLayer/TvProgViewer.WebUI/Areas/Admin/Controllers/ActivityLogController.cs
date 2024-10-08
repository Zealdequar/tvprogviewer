﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TvProgViewer.Services.Localization;
using TvProgViewer.Services.Logging;
using TvProgViewer.Services.Messages;
using TvProgViewer.Services.Security;
using TvProgViewer.WebUI.Areas.Admin.Factories;
using TvProgViewer.WebUI.Areas.Admin.Models.Logging;
using TvProgViewer.Web.Framework.Mvc;

namespace TvProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ActivityLogController : BaseAdminController
    {
        #region Fields

        private readonly IActivityLogModelFactory _activityLogModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly INotificationService _notificationService;

        #endregion

        #region Ctor

        public ActivityLogController(IActivityLogModelFactory activityLogModelFactory,
            IUserActivityService userActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService)
        {
            _activityLogModelFactory = activityLogModelFactory;
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
        }

        #endregion

        #region Methods

        public virtual async Task<IActionResult> ActivityTypes()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogTypeSearchModelAsync(new ActivityLogTypeSearchModel());

            return View(model);
        }

        [HttpPost, ActionName("SaveTypes")]
        public virtual async Task<IActionResult> SaveTypes(IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //activity log
            await _userActivityService.InsertActivityAsync("EditActivityLogTypes", await _localizationService.GetResourceAsync("ActivityLog.EditActivityLogTypes"));

            //get identifiers of selected activity types
            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            //update activity types
            var activityTypes = await _userActivityService.GetAllActivityTypesAsync();
            foreach (var activityType in activityTypes)
            {
                activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
                await _userActivityService.UpdateActivityTypeAsync(activityType);
            }

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Users.ActivityLogType.Updated"));

            return RedirectToAction("ActivityTypes");
        }

        public virtual async Task<IActionResult> ActivityLogs()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogSearchModelAsync(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _activityLogModelFactory.PrepareActivityLogListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ActivityLogDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //try to get a log item with the specified id
            var logItem = await _userActivityService.GetActivityByIdAsync(id)
                ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

            await _userActivityService.DeleteActivityAsync(logItem);

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteActivityLog",
                await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"), logItem);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> ClearAll()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            await _userActivityService.ClearAllActivitiesAsync();

            //activity log
            await _userActivityService.InsertActivityAsync("DeleteActivityLog", await _localizationService.GetResourceAsync("ActivityLog.DeleteActivityLog"));

            return RedirectToAction("ActivityLogs");
        }

        #endregion
    }
}