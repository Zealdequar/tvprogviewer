using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037681")]
    public class AddUserEmailIX : AutoReversingMigration
    {
        #region Methods   

        public override void Up()
        {
            this.AddIndex("IX_User_Email", nameof(User), i => i.Ascending(), nameof(User.Email));
        }

        #endregion
    }
}