using LinqToDB.Mapping;
using TVProgViewer.Core.Domain.Tasks;

namespace TVProgViewer.Data.Mapping.Tasks
{
    /// <summary>
    /// Represents a schedule task mapping configuration
    /// </summary>
    public partial class ScheduleTaskMap : TvProgEntityTypeConfiguration<ScheduleTask>
    {
        #region Methods

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public override void Configure(EntityMappingBuilder<ScheduleTask> builder)
        {
            builder.HasTableName(nameof(ScheduleTask));

            builder.Property(task => task.Name).IsNullable(false);
            builder.Property(task => task.Type).IsNullable(false);
            builder.Property(task => task.Seconds);
            builder.Property(task => task.Enabled);
            builder.Property(task => task.StopOnError);
            builder.Property(task => task.LastStartUtc);
            builder.Property(task => task.LastEndUtc);
            builder.Property(task => task.LastSuccessUtc);
        }

        #endregion
    }
}