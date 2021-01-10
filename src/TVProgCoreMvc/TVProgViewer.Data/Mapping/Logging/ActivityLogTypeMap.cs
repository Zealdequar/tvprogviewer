using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Logging;

namespace TVProgViewer.Data.Mapping.Logging
{
    /// <summary>
    /// Represents an activity log type mapping configuration
    /// </summary>
    public partial class ActivityLogTypeMap : TvProgEntityTypeConfiguration<ActivityLogType>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ActivityLogType> builder)
        {
            builder.HasTableName(nameof(ActivityLogType));

            builder.Property(logType => logType.SystemKeyword).HasLength(100).IsNullable(false);
            builder.Property(logType => logType.Name).HasLength(200).IsNullable(false);
            builder.Property(activitylogtype => activitylogtype.Enabled);
        }

        #endregion
    }
}