using System;
using System.Collections.Generic;
using System.Linq;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Core.Html;
using TVProgViewer.Services.Users;
using TVProgViewer.Services.Helpers;
using TVProgViewer.Services.Localization;
using TVProgViewer.Services.Logging;
using TVProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TVProgViewer.WebUI.Areas.Admin.Models.Logging;
using TVProgViewer.Web.Framework.Models.DataTables;
using TVProgViewer.Web.Framework.Models.Extensions;

namespace TVProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the log model factory implementation
    /// </summary>
    public partial class LogModelFactory : ILogModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        #endregion

        #region Ctor

        public LogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IUserService userService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService,
            ILogger logger)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _dateTimeHelper = dateTimeHelper;
            _userService = userService;
            _localizationService = localizationService;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare log search model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>Log search model</returns>
        public virtual LogSearchModel PrepareLogSearchModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available log levels
            _baseAdminModelFactory.PrepareLogLevels(searchModel.AvailableLogLevels);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged log list model
        /// </summary>
        /// <param name="searchModel">Log search model</param>
        /// <returns>Log list model</returns>
        public virtual LogListModel PrepareLogListModel(LogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var createdOnFromValue = searchModel.CreatedOnFrom.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, _dateTimeHelper.CurrentTimeZone) : null;
            var createdToFromValue = searchModel.CreatedOnTo.HasValue
                ? (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1) : null;
            var logLevel = searchModel.LogLevelId > 0 ? (LogLevel?)searchModel.LogLevelId : null;

            //get log
            var logItems = _logger.GetAllLogs(message: searchModel.Message,
                fromUtc: createdOnFromValue,
                toUtc: createdToFromValue,
                logLevel: logLevel,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = new LogListModel().PrepareToGrid(searchModel, logItems, () =>
            {
                //fill in model values from the entity
                return logItems.Select(logItem =>
                {
                    //fill in model values from the entity
                    var logModel = logItem.ToModel<LogModel>();

                    //convert dates to the user time
                    logModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    logModel.LogLevel = _localizationService.GetLocalizedEnum(logItem.LogLevel);
                    logModel.ShortMessage = HtmlHelper.FormatText(logItem.ShortMessage, false, true, false, false, false, false);
                    logModel.FullMessage = string.Empty;
                    logModel.UserEmail = _userService.GetUserById(logItem.UserId ?? 0)?.Email ?? string.Empty;

                    return logModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare log model
        /// </summary>
        /// <param name="model">Log model</param>
        /// <param name="log">Log</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Log model</returns>
        public virtual LogModel PrepareLogModel(LogModel model, Log log, bool excludeProperties = false)
        {
            if (log != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = log.ToModel<LogModel>();

                    model.LogLevel = _localizationService.GetLocalizedEnum(log.LogLevel);
                    model.ShortMessage = HtmlHelper.FormatText(log.ShortMessage, false, true, false, false, false, false);
                    model.FullMessage = HtmlHelper.FormatText(log.FullMessage, false, true, false, false, false, false);
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(log.CreatedOnUtc, DateTimeKind.Utc);
                    model.UserEmail = log.UserId.HasValue ? _userService.GetUserById(log.UserId.Value)?.Email : string.Empty;
                }
            }
            return model;
        }

        #endregion
    }
}