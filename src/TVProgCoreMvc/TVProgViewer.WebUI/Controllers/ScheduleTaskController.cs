using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TVProgViewer.Services.Tasks;
using Task = TVProgViewer.Services.Tasks.Task;

namespace TVProgViewer.WebUI.Controllers
{
    //do not inherit it from BasePublicController. otherwise a lot of extra action filters will be called
    //they can create guest account(s), etc
    public partial class ScheduleTaskController : Controller
    {
        private readonly IScheduleTaskService _scheduleTaskService;

        public ScheduleTaskController(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual async Task<IActionResult> RunTask(string taskType)
        {
            var scheduleTask = await _scheduleTaskService.GetTaskByTypeAsync(taskType);
            if (scheduleTask == null)
                //schedule task cannot be loaded
                return NoContent();

            var task = new Task(scheduleTask);
            await task.ExecuteAsync();

            return NoContent();
        }
    }
}