using System.Threading.Tasks;
using TvProgViewer.Services.ScheduleTasks;

namespace TvProgViewer.Plugin.Misc.Zettle.Services
{
    /// <summary>
    /// Represents a schedule task to synchronize tvChannels
    /// </summary>
    public class ZettleSyncTask : IScheduleTask
    {
        #region Fields

        private readonly ZettleService _zettleService;
        private readonly ZettleSettings _zettleSettings;

        #endregion

        #region Ctor

        public ZettleSyncTask(ZettleService zettleService,
            ZettleSettings zettleSettings)
        {
            _zettleService = zettleService;
            _zettleSettings = zettleSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Execute task
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task ExecuteAsync()
        {
            if (ZettleService.IsConfigured(_zettleSettings))
                await _zettleService.ImportAsync();
        }

        #endregion
    }
}