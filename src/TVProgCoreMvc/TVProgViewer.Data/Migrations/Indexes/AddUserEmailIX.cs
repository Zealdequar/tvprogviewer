using FluentMigrator;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037681")]
    public class AddUserEmailIX : AutoReversingMigration
    {
        #region Methods   

        public override void Up()
        {
            Create.Index("IX_User_Email").OnTable(nameof(User))
                .OnColumn(nameof(User.Email)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}