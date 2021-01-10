using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037685")]
    public class AddUserCreatedOnUtcIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_User_CreatedOnUtc", nameof(User), i => i.Descending(),
                nameof(User.CreatedOnUtc));
        }

        #endregion
    }
}