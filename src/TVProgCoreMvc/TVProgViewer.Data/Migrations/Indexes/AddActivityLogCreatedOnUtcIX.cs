using FluentMigrator;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 11:35:09:1647926")]
    public class AddActivityLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddIndex("IX_ActivityLog_CreatedOnUtc", nameof(ActivityLog), i => i.Descending(),
                nameof(ActivityLog.CreatedOnUtc));
        }

        #endregion
    }
}