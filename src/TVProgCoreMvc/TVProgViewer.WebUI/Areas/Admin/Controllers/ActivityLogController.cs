using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Models.Logging;
using TVProgViewer.Web.Framework.Mvc;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
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

        public virtual IActionResult ActivityTypes()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogTypeSearchModel(new ActivityLogTypeSearchModel());

            return View(model);
        }

        [HttpPost, ActionName("SaveTypes")]
        public virtual IActionResult SaveTypes(IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //activity log
            _userActivityService.InsertActivity("EditActivityLogTypes", _localizationService.GetResource("ActivityLog.EditActivityLogTypes"));

            //get identifiers of selected activity types
            var selectedActivityTypesIds = form["checkbox_activity_types"]
                .SelectMany(value => value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(idString => int.TryParse(idString, out var id) ? id : 0)
                .Distinct().ToList();

            //update activity types
            var activityTypes = _userActivityService.GetAllActivityTypes();
            foreach (var activityType in activityTypes)
            {
                activityType.Enabled = selectedActivityTypesIds.Contains(activityType.Id);
                _userActivityService.UpdateActivityType(activityType);
            }

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Configuration.ActivityLog.ActivityLogType.Updated"));

            return RedirectToAction("ActivityTypes");
        }

        public virtual IActionResult ActivityLogs()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogSearchModel(new ActivityLogSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ListLogs(ActivityLogSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _activityLogModelFactory.PrepareActivityLogListModel(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual IActionResult ActivityLogDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            //try to get a log item with the specified id
            var logItem = _userActivityService.GetActivityById(id)
                ?? throw new ArgumentException("No activity log found with the specified id", nameof(id));

            _userActivityService.DeleteActivity(logItem);

            //activity log
            _userActivityService.InsertActivity("DeleteActivityLog",
                _localizationService.GetResource("ActivityLog.DeleteActivityLog"), logItem);

            return new NullJsonResult();
        }

        public virtual IActionResult ClearAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageActivityLog))
                return AccessDeniedView();

            _userActivityService.ClearAllActivities();

            //activity log
            _userActivityService.InsertActivity("DeleteActivityLog", _localizationService.GetResource("ActivityLog.DeleteActivityLog"));

            return RedirectToAction("ActivityLogs");
        }

        #endregion
    }
}