using System;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Services.Tasks;

namespace TVProgViewer.Services.Users
{
    /// <summary>
    /// Represents a task for deleting guest Users
    /// </summary>
    public partial class DeleteGuestsTask : IScheduleTask
    {
        #region Fields

        private readonly UserSettings _userSettings;
        private readonly IUserService _userService;

        #endregion

        #region Ctor

        public DeleteGuestsTask(UserSettings UserSettings,
            IUserService UserService)
        {
            _userSettings = UserSettings;
            _userService = UserService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        public void Execute()
        {
            var olderThanMinutes = _userSettings.DeleteGuestTaskOlderThanMinutes;
            // Default value in case 0 is returned.  0 would effectively disable this service and harm performance.
            olderThanMinutes = olderThanMinutes == 0 ? 1440 : olderThanMinutes;

            _userService.DeleteGuestUsers(null, DateTime.UtcNow.AddMinutes(-olderThanMinutes), true);
        }

        #endregion
    }
}