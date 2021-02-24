using FluentMigrator;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037684")]
    public class AddUserSystemNameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_User_SystemName").OnTable(nameof(User))
                .OnColumn(nameof(User.SystemName)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}