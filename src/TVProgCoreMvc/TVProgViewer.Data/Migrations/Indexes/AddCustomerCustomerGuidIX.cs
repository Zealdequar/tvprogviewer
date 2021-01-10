using FluentMigrator;
using TVProgViewer.Core.Domain.Users;
using TVProgViewer.Data.Extensions;

namespace TVProgViewer.Data.Migrations.Indexes
{
    [TvProgMigration("2019/12/19 09:36:08:9037683")]
    public class AddUserUserGuidIX : AutoReversingMigration
    {
        #region Methods          

        public override void Up()
        {
            this.AddIndex("IX_User_UserGuid", nameof(User), i => i.Ascending(),
                nameof(User.UserGuid));
        }

        #endregion
    }
}