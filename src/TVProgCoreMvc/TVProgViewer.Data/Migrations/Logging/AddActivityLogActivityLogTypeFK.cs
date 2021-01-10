using System.Data;
using FluentMigrator;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Logging
{
    [TvProgMigration("2019/11/19 04:57:30:8380329")]
    public class AddActivityLogActivityLogTypeFK : AutoReversingMigration
    {
        #region Methods

        public override void Up()
        {
            this.AddForeignKey(nameof(ActivityLog),
                nameof(ActivityLog.ActivityLogTypeId),
                nameof(ActivityLogType),
                nameof(ActivityLogType.Id),
                Rule.Cascade);
        }

        #endregion
    }
}