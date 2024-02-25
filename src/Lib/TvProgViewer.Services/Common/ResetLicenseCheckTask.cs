﻿using System.Threading.Tasks;
using TvProgViewer.Core.Domain.Common;
using TvProgViewer.Services.Configuration;
using TvProgViewer.Services.ScheduleTasks;

namespace TvProgViewer.Services.Common
{
    /// <summary>
    /// Represents a task to reset license check
    /// </summary>
    public partial class ResetLicenseCheckTask : IScheduleTask
    {
        #region Fields

        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public ResetLicenseCheckTask(ISettingService settingService)
        {
            _settingService = settingService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Executes a task
        /// </summary>
        /// <returns>Задача представляет асинхронную операцию</returns>
        public async Task ExecuteAsync()
        {
            await _settingService.SetSettingAsync($"{nameof(AdminAreaSettings)}.{nameof(AdminAreaSettings.CheckLicense)}", true);
        }

        #endregion
    }
}