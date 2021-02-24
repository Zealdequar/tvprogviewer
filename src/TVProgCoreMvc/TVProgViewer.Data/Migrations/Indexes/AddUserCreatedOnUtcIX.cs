using FluentMigrator;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037685")]
    public class AddUserCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_User_CreatedOnUtc").OnTable(nameof(User))
                .OnColumn(nameof(User.CreatedOnUtc)).Descending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}