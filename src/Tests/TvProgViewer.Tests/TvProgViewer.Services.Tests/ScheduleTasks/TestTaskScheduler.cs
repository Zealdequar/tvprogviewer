using System.Linq;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using TvProgViewer.Core;
using TvProgViewer.Core.Configuration;
using TvProgViewer.Services.ScheduleTasks;


namespace TvProgViewer.Tests.TvProgViewer.Services.Tests.ScheduleTasks
{
    public class TestTaskScheduler: TaskScheduler
    {
        public TestTaskScheduler(AppSettings appSettings, IHttpClientFactory httpClientFactory, IScheduleTaskService scheduleTaskService, IServiceScopeFactory serviceScopeFactory, IStoreContext storeContext) : base(appSettings, httpClientFactory, scheduleTaskService, serviceScopeFactory, storeContext)
        {
        }

        public bool IsInit => _taskThreads.Any();

        public bool IsRun => _taskThreads.All(p => p.IsStarted && !p.IsDisposed);
    }
}
