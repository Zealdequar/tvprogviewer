using FluentMigrator.Builders.Create.Table;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Mapping.Builders.Logging
{
    /// <summary>
    /// Represents a activity log entity builder
    /// </summary>
    public partial class ActivityLogBuilder : TvProgEntityBuilder<ActivityLog>
    {
        #region Methods

        /// <summary>
        /// Apply entity configuration
        /// </summary>
        /// <param name="table">Create table expression builder</param>
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(ActivityLog.Comment)).AsString(int.MaxValue).NotNullable()
                .WithColumn(nameof(ActivityLog.IpAddress)).AsString(200).Nullable()
                .WithColumn(nameof(ActivityLog.EntityName)).AsString(400).Nullable()
                .WithColumn(nameof(ActivityLog.ActivityLogTypeId)).AsInt32().ForeignKey<ActivityLogType>()
                .WithColumn(nameof(ActivityLog.UserId)).AsInt32().ForeignKey<User>();
        }

        #endregion
    }
}