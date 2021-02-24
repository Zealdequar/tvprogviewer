using System;
using Microsoft.AspNetCore.Mvc;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.Services.Messages;
using TVProgViewer.Services.Security;
using TVProgViewer.Services.Tasks;
using TVProgViewer.WebUI.Areas.Admin.Factories;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Tasks;
using TVProgViewer.Web.Framework.Mvc;
using TVProgViewer.Web.Framework.Mvc.ModelBinding;
using System.Threading.Tasks;
using Task = TVProgViewer.Services.Tasks.Task;

namespace TVProgViewer.WebUI.Areas.Admin.Controllers
{
    public partial class ScheduleTaskController : BaseAdminController
    {
        #region Fields

        private readonly IUserActivityService _userActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService;
        private readonly IScheduleTaskModelFactory _scheduleTaskModelFactory;
        private readonly IScheduleTaskService _scheduleTaskService;

        #endregion

        #region Ctor

        public ScheduleTaskController(IUserActivityService userActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,
            IScheduleTaskModelFactory scheduleTaskModelFactory,
            IScheduleTaskService scheduleTaskService)
        {
            _userActivityService = userActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService;
            _scheduleTaskModelFactory = scheduleTaskModelFactory;
            _scheduleTaskService = scheduleTaskService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            //prepare model
            var model = await _scheduleTaskModelFactory.PrepareScheduleTaskSearchModelAsync(new ScheduleTaskSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(ScheduleTaskSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _scheduleTaskModelFactory.PrepareScheduleTaskListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> TaskUpdate(ScheduleTaskModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return ErrorJson(ModelState.SerializeErrors());

            //try to get a schedule task with the specified id
            var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(model.Id)
                               ?? throw new ArgumentException("Schedule task cannot be loaded");

            scheduleTask = model.ToEntity(scheduleTask);

            await _scheduleTaskService.UpdateTaskAsync(scheduleTask);

            //activity log
            await _userActivityService.InsertActivityAsync("EditTask",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.EditTask"), scheduleTask.Id), scheduleTask);

            return new NullJsonResult();
        }

        public virtual async Task<IActionResult> RunNow(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageScheduleTasks))
                return AccessDeniedView();

            try
            {
                //try to get a schedule task with the specified id
                var scheduleTask = await _scheduleTaskService.GetTaskByIdAsync(id)
                                   ?? throw new ArgumentException("Schedule task cannot be loaded", nameof(id));

                //ensure that the task is enabled
                var task = new Task(scheduleTask) { Enabled = true };
                await task.ExecuteAsync(true, false);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.System.ScheduleTasks.RunNow.Done"));
            }
            catch (Exception exc)
            {
                await _notificationService.ErrorNotificationAsync(exc);
            }

            return RedirectToAction("List");
        }

        #endregion
    }
}