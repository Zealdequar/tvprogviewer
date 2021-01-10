using FluentMigrator;
using TVProgViewer.Core.Domain.Logging;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037680")]
    public class AddLogCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_Log_CreatedOnUtc", nameof(Log), i => i.Descending(), nameof(Log.CreatedOnUtc));
        }

        #endregion
    }
}