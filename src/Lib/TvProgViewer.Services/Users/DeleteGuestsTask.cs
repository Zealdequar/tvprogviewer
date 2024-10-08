﻿using System;
using TvProgViewer.Core.Domain.Users;
using TvProgViewer.Services.ScheduleTasks;

namespace TvProgViewer.Services.Users
{
    /// <summary>
    /// Represents a task for deleting guest users
    /// </summary>
    public partial class DeleteGuestsTask : IScheduleTask
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public DeleteGuestsTask(UserSettings userSettings,
            IUserService userService)
        {
            _userSettings = userSettings;
            _userService = userService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public async System.Threading.Tasks.Task ExecuteAsync()
        {
            var olderThanMinutes = _userSettings.DeleteGuestTaskOlderThanMinutes;
            // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
            olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

            await _userService.DeleteGuestUsersAsync(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }

        #endregion
    }
}