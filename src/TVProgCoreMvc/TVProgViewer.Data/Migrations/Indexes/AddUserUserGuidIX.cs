using FluentMigrator;
using TVProgViewer.Core.Domain.Users;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2020/03/13 09:36:08:9037683")]
    public class AddUserUserGuidIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            Create.Index("IX_User_UserGuid").OnTable(nameof(User))
                .OnColumn(nameof(User.UserGuid)).Ascending()
                .WithOptions().NonClustered();
        }

        #endregion
    }
}