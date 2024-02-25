using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TvProgViewer.Services.Users;
using TvProgViewer.Services.Helpers;
using TvProgViewer.Services.Logging;
using TvProgViewer.WebUI.Areas.Admin.Infrastructure.Mapper.Extensions;
using TvProgViewer.WebUI.Areas.Admin.Models.Logging;
using TvProgViewer.Web.Framework.Models.Extensions;

namespace TvProgViewer.WebUI.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the activity log model factory implementation
    /// </summary>
    public partial class ActivityLogModelFactory : IActivityLogModelFactory
    {
        #region Fields

        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly IUserActivityService _userActivityService;
        private readonly IUserService _userService;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Ctor

        public ActivityLogModelFactory(IBaseAdminModelFactory baseAdminModelFactory,
            IUserActivityService userActivityService,
            IUserService userService,
            IDateTimeHelper dateTimeHelper)
        {
            _baseAdminModelFactory = baseAdminModelFactory;
            _userActivityService = userActivityService;
            _userService = userService;
            _dateTimeHelper = dateTimeHelper;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare activity log type models
        /// </summary>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the list of activity log type models
        /// </returns>
        protected virtual async Task<IList<ActivityLogTypeModel>> PrepareActivityLogTypeModelsAsync()
        {
            //prepare available activity log types
            var availableActivityTypes = await _userActivityService.GetAllActivityTypesAsync();
            var models = availableActivityTypes.Select(activityType => activityType.ToModel<ActivityLogTypeModel>()).ToList();

            return models;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare activity log types search model
        /// </summary>
        /// <param name="searchModel">Activity log types search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log types search model
        /// </returns>
        public virtual async Task<ActivityLogTypeSearchModel> PrepareActivityLogTypeSearchModelAsync(ActivityLogTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.ActivityLogTypeListModel = await PrepareActivityLogTypeModelsAsync();

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare activity log search model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log search model
        /// </returns>
        public virtual async Task<ActivityLogSearchModel> PrepareActivityLogSearchModelAsync(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare available activity log types
            await _baseAdminModelFactory.PrepareActivityLogTypesAsync(searchModel.ActivityLogType);

            //prepare grid
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged activity log list model
        /// </summary>
        /// <param name="searchModel">Activity log search model</param>
        /// <returns>
        /// Задача представляет асинхронную операцию
        /// The task result contains the activity log list model
        /// </returns>
        public virtual async Task<ActivityLogListModel> PrepareActivityLogListModelAsync(ActivityLogSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter log
            var startDateValue = searchModel.CreatedOnFrom == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnFrom.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync());
            var endDateValue = searchModel.CreatedOnTo == null ? null
                : (DateTime?)_dateTimeHelper.ConvertToUtcTime(searchModel.CreatedOnTo.Value, await _dateTimeHelper.GetCurrentTimeZoneAsync()).AddDays(1);

            //get log
            var activityLog = await _userActivityService.GetAllActivitiesAsync(createdOnFrom: startDateValue,
                createdOnTo: endDateValue,
                activityLogTypeId: searchModel.ActivityLogTypeId,
                ipAddress: searchModel.IpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            if (activityLog is null)
                return new ActivityLogListModel();

            //prepare list model
            var userIds = activityLog.GroupBy(logItem => logItem.UserId).Select(logItem => logItem.Key);
            var activityLogUsers = await _userService.GetUsersByIdsAsync(userIds.ToArray());
            var model = await new ActivityLogListModel().PrepareToGridAsync(searchModel, activityLog, () =>
            {
                return activityLog.SelectAwait(async logItem =>
                {
                    //fill in model values from the entity
                    var logItemModel = logItem.ToModel<ActivityLogModel>();
                    logItemModel.ActivityLogTypeName = (await _userActivityService.GetActivityTypeByIdAsync(logItem.ActivityLogTypeId))?.Name;

                    logItemModel.UserEmail = activityLogUsers?.FirstOrDefault(x => x.Id == logItem.UserId)?.Email;

                    //convert dates to the user time
                    logItemModel.CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(logItem.CreatedOnUtc, DateTimeKind.Utc);

                    return logItemModel;
                });
            });

            return model;
        }

        #endregion
    }
}