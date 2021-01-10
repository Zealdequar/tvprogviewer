using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037684")]
    public class AddUserSystemNameIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_User_SystemName", nameof(User), i => i.Ascending(), nameof(User.SystemName));
        }

        #endregion
    }
}