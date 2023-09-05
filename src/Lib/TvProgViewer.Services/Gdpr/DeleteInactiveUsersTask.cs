using System;
using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Gdpr;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.ScheduleTasks;

namespace TvProgViewer.Services.Gdpr
{
    /// <summary>
    /// Represents a task for deleting inactive users
    /// </summary>
    public partial class DeleteInactiveUsersTask : IScheduleTask
    {
        #region Fields

        private readonly IUserService _userService;
        private readonly IGdprService _gdprService;
        private readonly GdprSettings _gdprSettings;

        #endregion

        #region Ctor

        public DeleteInactiveUsersTask(IUserService userService,
            IGdprService gdprService,
            GdprSettings gdprSettings)
        {
            _userService = userService;
            _gdprService = gdprService;
            _gdprSettings = gdprSettings;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async Task ExecuteAsync()
        {
            if (!_gdprSettings.GdprEnabled)
                return;

            var lastActivityToUtc = DateTime.UtcNow.AddMonths(-_gdprSettings.DeleteInactiveUsersAfterMonths);
            var inactiveUsers = await _userService.GetAllUsersAsync(lastActivityToUtc : lastActivityToUtc);

            foreach (var user in inactiveUsers)
                await _gdprService.PermanentDeleteUserAsync(user);
        }

        #endregion
    }
}
