using FluentMigrator;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037682")]
    public class AddUserUsernameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_User_Username").OnTable(nameof(User))
                .OnColumn(nameof(User.Username)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}